using System;
using System.Threading.Tasks;
using static MyTwitterForms.Application.Login.Tokens.IAccessTokensObtainUseCase;

namespace MyTwitterForms.Application.Login.Tokens
{
    internal class StubAccessTokensObtainInteractor : IAccessTokensObtainUseCase
    {
        async Task<Response> IAccessTokensObtainUseCase.Execute(Request request)
        {
            try
            {
                await Task.Delay(3000, request.Cancellation);

                if (new Random().Next() % 3 == 0) throw new Exception("ダミー例外");

                return new Response.Success();
            }

            catch (OperationCanceledException)
            {
                return new Response.Cancelled();
            }

            catch (Exception e)
            {
                return new Response.Failure(e);
            }
        }
    }
}
