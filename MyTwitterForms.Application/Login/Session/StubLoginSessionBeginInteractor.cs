using System;
using System.Threading.Tasks;
using static MyTwitterForms.Application.Login.Session.ILoginSessionBeginUseCase;

namespace MyTwitterForms.Application.Login.Session
{
    internal class StubLoginSessionBeginInteractor : ILoginSessionBeginUseCase
    {
        async Task<Response> ILoginSessionBeginUseCase.Execute(Request request)
        {
            try
            {
                await Task.Delay(3000, request.Cancellation);

                if (new Random().Next() % 3 == 0) throw new Exception("ダミー例外");

                var dummySession = new LoginSession(sessionId: "ID", authorizeUrl: "https://twitter.com");
                return new Response.Success(dummySession);
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
