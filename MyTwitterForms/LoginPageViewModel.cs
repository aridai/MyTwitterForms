using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;
using CoreTweet;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MyTwitterForms
{
    internal class LoginPageViewModel : BindableBase, IDestructible
    {
        private const string ConsumerKey = Env.ApiKey;
        private const string ConsumerSecret = Env.ApiSecretKey;
        private const int PinCodeLength = 7;

        private readonly INavigationService navigationService;
        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

        private CancellationTokenSource? cancellation = null;

        //  現在試行中のセッション
        private readonly ReactiveProperty<OAuth.OAuthSession?> session =
            new ReactiveProperty<OAuth.OAuthSession?>(initialValue: null);

        //  認証用URL
        public ReactiveProperty<string?> AuthorizeUri { get; }

        //  PINコードの入力が有効かどうか
        public IReadOnlyReactiveProperty<bool> IsPinCodeEntryEnabled { get; }

        //  PINコード
        public ReactiveProperty<string?> PinCode { get; } = new ReactiveProperty<string?>(initialValue: "");

        //  戻るボタンのコマンド
        public ICommand BackButtonCommand { get; }

        public LoginPageViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            this.BackButtonCommand = new DelegateCommand(this.Back);

            //  セッションから認証用URLを設定する。
            this.AuthorizeUri = this.session.Select(session => session?.AuthorizeUri?.AbsoluteUri)
                .ToReactiveProperty()
                .AddTo(this.compositeDisposable);

            //  セッションが存在するときのみPIN入力を受け付ける。
            this.IsPinCodeEntryEnabled = this.session.Select(session => session != null)
                .ToReadOnlyReactiveProperty()
                .AddTo(this.compositeDisposable);

            //  PINコードを検査する。
            this.PinCode.OfType<string>()
                .Where(pin => this.IsPinCodeEntryEnabled.Value && pin.Length == PinCodeLength && pin.All(char.IsNumber))
                .Subscribe(this.ObtainTokens)
                .AddTo(this.compositeDisposable);

            //  デバッグ用
            this.AuthorizeUri.Subscribe(uri => System.Diagnostics.Debug.WriteLine($"URL: {uri}"));
            this.PinCode.Subscribe(pin => System.Diagnostics.Debug.WriteLine($"PINコード: {pin}"));

            this.BeginSession();
        }

        private async void Back()
        {
            await this.navigationService.GoBackAsync();
        }

        //  認証セッションを開始する。
        private async void BeginSession()
        {
            try
            {
                var cancellation = new CancellationTokenSource();
                this.cancellation = cancellation;

                var session = await OAuth.AuthorizeAsync(
                    consumerKey: ConsumerKey,
                    consumerSecret: ConsumerSecret,
                    cancellationToken: cancellation.Token
                );

                this.session.Value = session;
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine($"セッション開始キャンセル");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"セッション開始エラー: {e}");
            }
            finally
            {
                this.cancellation?.Dispose();
                this.cancellation = null;
            }
        }

        //  トークンを取得する。
        private async void ObtainTokens(string pin)
        {
            try
            {
                var cancellation = new CancellationTokenSource();
                this.cancellation = cancellation;

                var tokens = await OAuth.GetTokensAsync(this.session.Value!, pin, cancellation.Token);

                System.Diagnostics.Debug.WriteLine($"トークン取得成功: {tokens.AccessToken}, {tokens.AccessTokenSecret}");

                await this.navigationService.GoBackAsync(("TOKENS", tokens));
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine($"トークン取得キャンセル");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"トークン取得エラー: {e}");
            }
            finally
            {
                this.cancellation?.Dispose();
                this.cancellation = null;
            }
        }

        void IDestructible.Destroy()
        {
            this.compositeDisposable.Dispose();

            if (this.cancellation is CancellationTokenSource cancellation)
            {
                cancellation.Dispose();
                this.cancellation = null;
            }
        }
    }
}
