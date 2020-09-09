using System;
using static MyTwitterForms.Application.Login.Status.ILoginStatusGetUseCase;

namespace MyTwitterForms.Application.Login.Status
{
    internal class StubLoginStatusGetInteractor : ILoginStatusGetUseCase
    {
        Response ILoginStatusGetUseCase.Execute(Request request)
        {
            var isLoggedIn = (new Random().Next() % 3 != 0);
            System.Diagnostics.Debug.WriteLine($"isLoggedIn: {isLoggedIn}");

            return new Response(isLoggedIn);
        }
    }
}
