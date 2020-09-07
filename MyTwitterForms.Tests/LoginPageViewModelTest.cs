using System;
using System.Linq;
using System.Reactive.Concurrency;
using Moq;
using MyTwitterForms.Application.Login;
using MyTwitterForms.Application.Login.Session;
using MyTwitterForms.Application.Login.Tokens;
using MyTwitterForms.UI.Login;
using Prism.Navigation;
using Reactive.Bindings;
using Xunit;

namespace MyTwitterForms.Tests
{
    public class LoginPageViewModelTest : IDisposable
    {
        public LoginPageViewModelTest()
        {
            ReactivePropertyScheduler.SetDefault(CurrentThreadScheduler.Instance);
        }

        private readonly Mock<INavigationService> navigationServiceMock = new Mock<INavigationService>();

        private readonly Mock<IObserver<bool>> isWebViewVisibleObserver = new Mock<IObserver<bool>>();
        private readonly Mock<IObserver<bool>> isProgressBarVisibleObserver = new Mock<IObserver<bool>>();
        private readonly Mock<IObserver<bool>> isRetryButtonVisibleObserver = new Mock<IObserver<bool>>();
        private readonly Mock<IObserver<string?>> authorizeUrlObserver = new Mock<IObserver<string?>>();
        private readonly Mock<IObserver<bool>> isPinCodeEntryEnabledObserver = new Mock<IObserver<bool>>();
        private readonly Mock<IObserver<string?>> pinCodeObserver = new Mock<IObserver<string?>>();
        private readonly Mock<EventHandler> retryButtonCommandHandler = new Mock<EventHandler>();
        private readonly Mock<EventHandler> loginButtonCommandHandler = new Mock<EventHandler>();

        [Fact]
        public void ログインセッションの開始もトークンの取得も成功するケース()
        {
            var sessionId = "ID";
            var authorizeUrl = "URL";
            var session = new LoginSession(sessionId, authorizeUrl);

            //  ログインセッションの開始に成功するように設定する。
            var loginSessionBeginUseCaseMock = new Mock<ILoginSessionBeginUseCase>();
            loginSessionBeginUseCaseMock.Setup(uc => uc.Execute(It.IsAny<ILoginSessionBeginUseCase.Request>()))
                .ReturnsAsync(new ILoginSessionBeginUseCase.Response.Success(session));

            //  アクセストークンの取得に成功するように設定する。
            var accessTokensObtainUseCaseMock = new Mock<IAccessTokensObtainUseCase>();
            accessTokensObtainUseCaseMock.Setup(uc => uc.Execute(It.IsAny<IAccessTokensObtainUseCase.Request>()))
                .ReturnsAsync(new IAccessTokensObtainUseCase.Response.Success());

            //  ViewModelを生成してプロパティを購読する。
            var viewModel = new LoginPageViewModel(
                this.navigationServiceMock.Object,
                loginSessionBeginUseCaseMock.Object,
                accessTokensObtainUseCaseMock.Object
            );
            this.SubscribeProperties(viewModel);

            //  はじめはWebViewが表示されていないはず。
            Assert.False(viewModel.IsWebViewVisible.Value);

            this.ClearCallCounts();

            //  初期化を行うと、自動でセッションの開始が行われるはず。
            (viewModel as IInitialize).Initialize(new NavigationParameters());

            //  ユースケースのコールが行われているはず。
            loginSessionBeginUseCaseMock.Verify(
                uc => uc.Execute(It.IsAny<ILoginSessionBeginUseCase.Request>()),
                Times.Once
            );

            //  WebViewが表示されているはず。
            Assert.True(viewModel.IsWebViewVisible.Value);

            //  プログレスバーが処理開始前に表示されており、処理が終わると非表示になったはず。
            Assert.Equal(
                new[] { true, false },
                this.isProgressBarVisibleObserver.Invocations.TakeLast(2).Select(i => (bool)i.Arguments.First())
            );

            //  再試行ボタンが非表示なはず。
            Assert.False(viewModel.IsRetryButtonVisible.Value);

            //  認証用URLが設定されているはず。
            Assert.Equal(authorizeUrl, viewModel.AuthorizeUrl.Value);

            //  PINコードの入力が有効なはず。
            Assert.True(viewModel.IsPinCodeEntryEnabled.Value);

            //  まだこの段階ではログインボタンは無効なはず。
            Assert.False(viewModel.LoginButtonCommand.CanExecute(new object()));

            //  PINコードを指定桁数より1桁少なく入力してみるが、まだログインボタンは無効なはず。
            viewModel.PinCode.Value = this.GeneratePinCode(length: LoginPageViewModel.PinCodeLength - 1);
            Assert.False(viewModel.LoginButtonCommand.CanExecute(new object()));

            //  PINコードを指定桁数より1桁多く入力してみるが、まだログインボタンは無効なはず。
            viewModel.PinCode.Value = this.GeneratePinCode(length: LoginPageViewModel.PinCodeLength + 1);
            Assert.False(viewModel.LoginButtonCommand.CanExecute(new object()));

            //  PINコードを指定桁数だけ入力すると、ログインボタンが有効になるはず。
            var pinCode = this.GeneratePinCode(length: LoginPageViewModel.PinCodeLength);
            viewModel.PinCode.Value = pinCode;
            Assert.True(viewModel.LoginButtonCommand.CanExecute(new object()));

            this.ClearCallCounts();

            //  アクセストークンの取得処理を行う。
            viewModel.LoginButtonCommand.Execute(new object());

            //  ユースケースのコールが行われているはず。
            accessTokensObtainUseCaseMock.Verify(
                uc => uc.Execute(
                    It.Is<IAccessTokensObtainUseCase.Request>(r => r.SessionId == sessionId && r.PinCode == pinCode)
                ),
                Times.Once
            );

            //  プログレスバーが再び表示されたはず。
            this.isProgressBarVisibleObserver.Verify(it => it.OnNext(true), Times.Once);

            //  PINコードの入力が無効なはず。
            Assert.False(viewModel.IsPinCodeEntryEnabled.Value);

            //  ログインボタンが無効なはず。
            Assert.False(viewModel.LoginButtonCommand.CanExecute(new object()));

            //  画面遷移が行われたはず。
            this.navigationServiceMock.Verify(it => it.GoBackAsync(), Times.Once);
        }

        [Fact]
        public void ログインセッションの開始に失敗するケース()
        {
            var sessionId = "ID";
            var authorizeUrl = "URL";
            var session = new LoginSession(sessionId, authorizeUrl);

            //  最初はログインセッションの開始に失敗し、次に試行すると成功するように設定する。
            var loginSessionBeginUseCaseMock = new Mock<ILoginSessionBeginUseCase>();
            loginSessionBeginUseCaseMock.SetupSequence(uc => uc.Execute(It.IsAny<ILoginSessionBeginUseCase.Request>()))
                .ReturnsAsync(new ILoginSessionBeginUseCase.Response.Failure(new Exception()))
                .ReturnsAsync(new ILoginSessionBeginUseCase.Response.Success(session));

            var accessTokensObtainUseCaseMock = new Mock<IAccessTokensObtainUseCase>();

            //  ViewModelを生成してプロパティを購読する。
            var viewModel = new LoginPageViewModel(
                this.navigationServiceMock.Object,
                loginSessionBeginUseCaseMock.Object,
                accessTokensObtainUseCaseMock.Object
            );
            this.SubscribeProperties(viewModel);

            this.ClearCallCounts();

            //  初期化を行うと、自動でセッションの開始が行われるはず。
            (viewModel as IInitialize).Initialize(new NavigationParameters());

            //  ユースケースのコールが行われているはず。
            loginSessionBeginUseCaseMock.Verify(
                uc => uc.Execute(It.IsAny<ILoginSessionBeginUseCase.Request>()),
                Times.Once
            );

            //  プログレスバーが処理開始前に表示されており、処理が終わると非表示になったはず。
            Assert.Equal(
                new[] { true, false },
                this.isProgressBarVisibleObserver.Invocations.TakeLast(2).Select(i => (bool)i.Arguments.First())
            );

            //  失敗したため、再試行ボタンが表示されているはず。
            Assert.True(viewModel.IsRetryButtonVisible.Value);

            //  認証用URLが設定されていていないはず。
            Assert.NotEqual(authorizeUrl, viewModel.AuthorizeUrl.Value);

            //  PINコードの入力が無効なはず。
            Assert.False(viewModel.IsPinCodeEntryEnabled.Value);

            //  再試行を行う。
            viewModel.RetryButtonCommand.Execute(new object());

            //  ユースケースのコールが再び行われているはず。
            loginSessionBeginUseCaseMock.Verify(
                uc => uc.Execute(It.IsAny<ILoginSessionBeginUseCase.Request>()),
                Times.Exactly(2)
            );

            //  プログレスバーが表示されていたはず。
            this.isProgressBarVisibleObserver.Verify(it => it.OnNext(true));

            //  成功したため、再試行ボタンが非表示なはず。
            Assert.False(viewModel.IsRetryButtonVisible.Value);

            //  今度こそ、認証用URLが設定されているはず。
            Assert.Equal(authorizeUrl, viewModel.AuthorizeUrl.Value);

            //  PINコードの入力が有効なはず。
            Assert.True(viewModel.IsPinCodeEntryEnabled.Value);
        }

        [Fact]
        public void トークンの取得に失敗するケース()
        {
            var sessionId = "ID";
            var authorizeUrl = "URL";
            var session = new LoginSession(sessionId, authorizeUrl);

            //  ログインセッションの開始に成功するように設定する。
            var loginSessionBeginUseCaseMock = new Mock<ILoginSessionBeginUseCase>();
            loginSessionBeginUseCaseMock.Setup(uc => uc.Execute(It.IsAny<ILoginSessionBeginUseCase.Request>()))
                .ReturnsAsync(new ILoginSessionBeginUseCase.Response.Success(session));

            //  アクセストークンの取得に失敗するように設定する。
            var accessTokensObtainUseCaseMock = new Mock<IAccessTokensObtainUseCase>();
            accessTokensObtainUseCaseMock.Setup(uc => uc.Execute(It.IsAny<IAccessTokensObtainUseCase.Request>()))
                .ReturnsAsync(new IAccessTokensObtainUseCase.Response.Failure(new Exception()));

            //  ViewModelを生成してプロパティを購読する。
            var viewModel = new LoginPageViewModel(
                this.navigationServiceMock.Object,
                loginSessionBeginUseCaseMock.Object,
                accessTokensObtainUseCaseMock.Object
            );
            this.SubscribeProperties(viewModel);

            this.ClearCallCounts();

            //  セッションの開始を行い、成功するはず。
            (viewModel as IInitialize).Initialize(new NavigationParameters());

            //  指定桁数のPINコードを入力する。
            viewModel.PinCode.Value = this.GeneratePinCode(length: LoginPageViewModel.PinCodeLength);

            this.ClearCallCounts();

            //  アクセストークンの取得処理を行う。
            viewModel.LoginButtonCommand.Execute(new object());

            //  ユースケースのコールが行われているはず。
            accessTokensObtainUseCaseMock.Verify(uc => uc.Execute(It.IsAny<IAccessTokensObtainUseCase.Request>()));

            //  プログレスバーが再び表示されたはず。
            this.isProgressBarVisibleObserver.Verify(it => it.OnNext(true), Times.Once);

            //  PINコードの入力が無効なはず。
            Assert.False(viewModel.IsPinCodeEntryEnabled.Value);

            //  ログインボタンが無効なはず。
            Assert.False(viewModel.LoginButtonCommand.CanExecute(new object()));

            //  画面遷移が行われていないはず。
            this.navigationServiceMock.Verify(it => it.GoBackAsync(), Times.Never);

            //  失敗したため、WebViewが非表示なったはず。
            Assert.False(viewModel.IsWebViewVisible.Value);

            //  プログレスバーが非表示なはず。
            Assert.False(viewModel.IsProgressBarVisible.Value);

            //  再試行ボタンが表示されているはず。
            Assert.True(viewModel.IsRetryButtonVisible.Value);

            this.ClearCallCounts();

            //  再試行を行う。

            //  ログインセッションを開始するユースケースが再びコールされたはず。
            loginSessionBeginUseCaseMock.Verify(uc => uc.Execute(It.IsAny<ILoginSessionBeginUseCase.Request>()));
        }

        void IDisposable.Dispose()
        {
            ReactivePropertyScheduler.SetDefault(DefaultScheduler.Instance);
        }

        private void SubscribeProperties(LoginPageViewModel viewModel)
        {
            viewModel.IsWebViewVisible.Subscribe(this.isWebViewVisibleObserver.Object);
            viewModel.IsProgressBarVisible.Subscribe(this.isProgressBarVisibleObserver.Object);
            viewModel.IsRetryButtonVisible.Subscribe(this.isRetryButtonVisibleObserver.Object);
            viewModel.AuthorizeUrl.Subscribe(this.authorizeUrlObserver.Object);
            viewModel.IsPinCodeEntryEnabled.Subscribe(this.isPinCodeEntryEnabledObserver.Object);
            viewModel.PinCode.Subscribe(this.pinCodeObserver.Object);
            viewModel.RetryButtonCommand.CanExecuteChanged += this.retryButtonCommandHandler.Object;
            viewModel.LoginButtonCommand.CanExecuteChanged += this.loginButtonCommandHandler.Object;
        }

        private void ClearCallCounts()
        {
            this.isWebViewVisibleObserver.Invocations.Clear();
            this.isProgressBarVisibleObserver.Invocations.Clear();
            this.isRetryButtonVisibleObserver.Invocations.Clear();
            this.authorizeUrlObserver.Invocations.Clear();
            this.isPinCodeEntryEnabledObserver.Invocations.Clear();
            this.pinCodeObserver.Invocations.Clear();
            this.retryButtonCommandHandler.Invocations.Clear();
            this.loginButtonCommandHandler.Invocations.Clear();
        }

        private string GeneratePinCode(int length) =>
            string.Join("", Enumerable.Range(1, length).Select(i => i % 10));
    }
}
