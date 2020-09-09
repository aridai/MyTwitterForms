using System;
using System.Threading;
using System.Threading.Tasks;
using MyTwitterForms.Application.Login;
using MyTwitterForms.Model;
using static MyTwitterForms.Application.Login.ILoginRepository;

namespace MyTwitterForms.Data.Login
{
    internal class LoginRepository : ILoginRepository
    {
        private const string AccessTokenKey = "ACCESS_TOKEN";
        private const string AccessTokenSecretKey = "ACCESS_TOKEN_SECRET";

        private readonly ApiKeys apiKeys;

        private (string, CoreTweet.OAuth.OAuthSession)? currentSession = null;

        public LoginRepository(ApiKeys apiKeys)
        {
            this.apiKeys = apiKeys;
        }

        public async Task<Result<LoginSession>> BeginLoginSession(CancellationToken cancellation)
        {
            try
            {
                var session = await CoreTweet.OAuth.AuthorizeAsync(
                    consumerKey: this.apiKeys.ApiKey,
                    consumerSecret: this.apiKeys.ApiSecretKey,
                    cancellationToken: cancellation
                );
                var sessionId = Guid.NewGuid().ToString();

                this.currentSession = (sessionId, session);
                var loginSession = new LoginSession(
                    sessionId: sessionId,
                    authorizeUrl: session.AuthorizeUri.AbsoluteUri
                );

                return new Result<LoginSession>.Success<LoginSession>(loginSession);
            }

            catch (OperationCanceledException)
            {
                return new Result<LoginSession>.Cancelled<LoginSession>();
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"ログインセッション開始失敗: {e}{Environment.NewLine}{e.StackTrace}");

                return new Result<LoginSession>.Failure<LoginSession>(e);
            }
        }

        public async Task<Result<AccessTokens>> ObtainAccessTokens(
            string sessionId,
            string pinCode,
            CancellationToken cancellation
        )
        {
            try
            {
                var current = this.currentSession;
                if (current == null)
                {
                    return new Result<AccessTokens>.Failure<AccessTokens>(
                        new InvalidOperationException("ログインセッションが存在しません。")
                    );
                }

                var (id, session) = current ?? default;
                if (id != sessionId)
                {
                    return new Result<AccessTokens>.Failure<AccessTokens>(
                        new InvalidOperationException("セッションの状態が不正です。")
                    );
                }

                var tokens = await CoreTweet.OAuth.GetTokensAsync(session, pinCode, cancellation);
                var accessTokens = new AccessTokens(tokens.AccessToken, tokens.AccessTokenSecret);

                return new Result<AccessTokens>.Success<AccessTokens>(accessTokens);
            }

            catch (OperationCanceledException)
            {
                return new Result<AccessTokens>.Cancelled<AccessTokens>();
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"トークン取得失敗: {e}{Environment.NewLine}{e.StackTrace}");

                return new Result<AccessTokens>.Failure<AccessTokens>(e);
            }
        }

        public void SaveAccessTokens(AccessTokens accessTokens)
        {
            Xamarin.Essentials.Preferences.Set(AccessTokenKey, accessTokens.AccessToken);
            Xamarin.Essentials.Preferences.Set(AccessTokenSecretKey, accessTokens.AccessTokenSecret);
        }

        public AccessTokens? GetAccessTokens()
        {
            var token = Xamarin.Essentials.Preferences.Get(AccessTokenKey, null);
            var secret = Xamarin.Essentials.Preferences.Get(AccessTokenSecretKey, null);

            if (token == null || secret == null) return null;
            else return new AccessTokens(token, secret);
        }
    }
}
