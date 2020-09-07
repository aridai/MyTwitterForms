using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;
using MyTwitterForms.Application.Login;
using MyTwitterForms.Application.Login.Session;
using MyTwitterForms.Application.Login.Tokens;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MyTwitterForms.UI.Login
{
    internal class LoginPageViewModel : BindableBase, IInitialize, IDestructible
    {
        public const int PinCodeLength = 7;

        private readonly INavigationService navigationService;
        private readonly ILoginSessionBeginUseCase loginSessionBeginUseCase;
        private readonly IAccessTokensObtainUseCase accessTokensObtainUseCase;

        private CancellationTokenSource? cancellation = null;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        //  現在試行中のセッション
        private readonly ReactiveProperty<LoginSession?> session =
            new ReactiveProperty<LoginSession?>(initialValue: null);

        //  ログインセッションを開始中かどうか
        private readonly ReactiveProperty<bool> isBeginningSession = new ReactiveProperty<bool>(initialValue: false);

        //  トークンの取得処理を実行中かどうか
        private readonly ReactiveProperty<bool> isObtainingTokens = new ReactiveProperty<bool>(initialValue: false);

        //  WebViewを表示するかどうか
        public IReadOnlyReactiveProperty<bool> IsWebViewVisible { get; }

        //  プログレスバーを表示するかどうか
        public IReadOnlyReactiveProperty<bool> IsProgressBarVisible { get; }

        //  再試行ボタンを表示するかどうか
        public IReadOnlyReactiveProperty<bool> IsRetryButtonVisible { get; }

        //  認証用URL
        public ReactiveProperty<string?> AuthorizeUrl { get; }

        //  PINコードの入力が有効かどうか
        public IReadOnlyReactiveProperty<bool> IsPinCodeEntryEnabled { get; }

        //  PINコード
        public ReactiveProperty<string?> PinCode { get; } = new ReactiveProperty<string?>(initialValue: "");

        //  再試行ボタンのコマンド
        public ICommand RetryButtonCommand { get; }

        //  ログインボタンのコマンド
        public ICommand LoginButtonCommand { get; }

        public LoginPageViewModel(
            INavigationService navigationService,
            ILoginSessionBeginUseCase loginSessionBeginUseCase,
            IAccessTokensObtainUseCase accessTokensObtainUseCase
        )
        {
            this.navigationService = navigationService;
            this.loginSessionBeginUseCase = loginSessionBeginUseCase;
            this.accessTokensObtainUseCase = accessTokensObtainUseCase;

            //  セッションが存在している間のみWebViewを表示する。
            this.IsWebViewVisible =
                this.session.Select(session => session != null).ToReadOnlyReactiveProperty().AddTo(this.disposables);

            //  セッション開始中かトークン取得中ならばプログレスバーを表示する。
            this.IsProgressBarVisible =
                Observable.CombineLatest(
                    this.isBeginningSession,
                    this.isObtainingTokens,
                    (busy1, busy2) => busy1 || busy2
                ).ToReadOnlyReactiveProperty().AddTo(this.disposables);

            //  セッションが存在せず、処理が行われていないときのみ、再試行ボタンを表示する。
            this.IsRetryButtonVisible =
                Observable.CombineLatest(
                    this.session,
                    this.IsProgressBarVisible,
                    (session, busy) => session == null && !busy
                ).ToReadOnlyReactiveProperty().AddTo(this.disposables);

            //  試行中のセッションから認証用URLを設定する。
            this.AuthorizeUrl =
                this.session.Select(session => session?.AuthorizeUrl).ToReactiveProperty().AddTo(this.disposables);

            //  セッションが存在し、処理を行っていないならば、PIN入力を受け付ける。
            this.IsPinCodeEntryEnabled =
                Observable.CombineLatest(
                    this.session,
                    this.IsProgressBarVisible,
                    (session, busy) => session != null && !busy
                ).ToReadOnlyReactiveProperty().AddTo(this.disposables);

            //  セッションが存在し、処理が行われておらず、指定桁数のPINが入力されているときのみ、ログインボタンを有効にする。
            var isLoginButtonEnabled = Observable.CombineLatest(
                this.session,
                this.IsProgressBarVisible,
                this.PinCode,
                (session, busy, pin) => session != null && !busy && pin?.Length == PinCodeLength
            );

            this.LoginButtonCommand =
                isLoginButtonEnabled.ToReactiveCommand<object?>(initialValue: false)
                .AddTo(this.disposables)
                .WithSubscribe(_ => this.ObtainTokens());

            this.RetryButtonCommand = new DelegateCommand(this.BeginLoginSession);
        }

        //  ログインセッションを開始する。
        private async void BeginLoginSession()
        {
            try
            {
                this.isBeginningSession.Value = true;

                var cancellation = new CancellationTokenSource();
                this.cancellation = cancellation;

                var request = new ILoginSessionBeginUseCase.Request(cancellation.Token);
                var response = await this.loginSessionBeginUseCase.Execute(request);

                switch (response)
                {
                    //  成功したらセッションを保持する。
                    case ILoginSessionBeginUseCase.Response.Success success:
                        this.session.Value = success.Session;
                        break;

                    //  失敗したらエラー表示を行う。
                    case ILoginSessionBeginUseCase.Response.Failure _:
                        break;
                }
            }
            finally
            {
                this.cancellation?.Dispose();
                this.cancellation = null;
                this.isBeginningSession.Value = false;
            }
        }

        //  トークンを取得する。
        private async void ObtainTokens()
        {
            try
            {
                this.isObtainingTokens.Value = true;

                var cancellation = new CancellationTokenSource();
                this.cancellation = cancellation;

                var sessionId = this.session.Value!.SessionId;
                var pinCode = this.PinCode.Value!;

                var request = new IAccessTokensObtainUseCase.Request(sessionId, pinCode, cancellation.Token);
                var response = await this.accessTokensObtainUseCase.Execute(request);

                switch (response)
                {
                    //  成功したらログイン画面を自動終了する。
                    case IAccessTokensObtainUseCase.Response.Success _:
                        this.session.Value = null;
                        await this.navigationService.GoBackAsync();
                        break;

                    //  失敗したらセッションをやり直させる。
                    case IAccessTokensObtainUseCase.Response.Failure _:
                        this.session.Value = null;
                        this.PinCode.Value = "";
                        break;
                }
            }
            finally
            {
                this.cancellation?.Dispose();
                this.cancellation = null;
                this.isObtainingTokens.Value = false;
            }
        }

        void IInitialize.Initialize(INavigationParameters parameters)
        {
            //  画面遷移時に自動でログインセッションを開始する。
            this.BeginLoginSession();
        }

        void IDestructible.Destroy()
        {
            if (this.cancellation is CancellationTokenSource cancellation)
            {
                cancellation.Cancel();
                cancellation.Dispose();
                this.cancellation = null;
            }
            this.disposables.Dispose();
        }
    }
}
