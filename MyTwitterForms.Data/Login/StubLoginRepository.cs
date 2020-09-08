using System;
using System.Threading;
using System.Threading.Tasks;
using MyTwitterForms.Application.Login;
using MyTwitterForms.Model;
using static MyTwitterForms.Application.Login.ILoginRepository;

namespace MyTwitterForms.Data.Login
{
    internal class StubLoginRepository : ILoginRepository
    {
        public async Task<Result<LoginSession>> BeginLoginSession(CancellationToken token)
        {
            try
            {
                await Task.Delay(3000, token);

                if (new Random().Next() % 3 == 0) throw new Exception();

                var session = new LoginSession(sessionId: "ID", authorizeUrl: "https://twitter.com");
                return new Result<LoginSession>.Success<LoginSession>(session);
            }

            catch (OperationCanceledException)
            {
                return new Result<LoginSession>.Cancelled<LoginSession>();
            }

            catch (Exception e)
            {
                return new Result<LoginSession>.Failure<LoginSession>(e);
            }
        }

        public async Task<Result<AccessTokens>> ObtainAccessTokens(string sessionId, string pinCode, CancellationToken token)
        {
            try
            {
                await Task.Delay(3000, token);

                if (new Random().Next() % 3 == 0) throw new Exception();

                var accessTokens = new AccessTokens(accessToken: "ACCESS_TOKEN", accessTokenSecret: "SECRET");
                return new Result<AccessTokens>.Success<AccessTokens>(accessTokens);
            }

            catch (OperationCanceledException)
            {
                return new Result<AccessTokens>.Cancelled<AccessTokens>();
            }

            catch (Exception e)
            {
                return new Result<AccessTokens>.Failure<AccessTokens>(e);
            }
        }
    }
}
