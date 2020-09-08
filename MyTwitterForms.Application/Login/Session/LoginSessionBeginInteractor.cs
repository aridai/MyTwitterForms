using System;
using System.Threading.Tasks;
using static MyTwitterForms.Application.Login.ILoginRepository;
using static MyTwitterForms.Application.Login.Session.ILoginSessionBeginUseCase;

namespace MyTwitterForms.Application.Login.Session
{
    internal class LoginSessionBeginInteractor : ILoginSessionBeginUseCase
    {
        private readonly ILoginRepository repository;

        public LoginSessionBeginInteractor(ILoginRepository repository)
        {
            this.repository = repository;
        }

        async Task<Response> ILoginSessionBeginUseCase.Execute(Request request)
        {
            var result = await this.repository.BeginLoginSession(request.Cancellation);

            return result switch
            {
                Result<LoginSession>.Success<LoginSession> success => new Response.Success(session: success.Value),
                Result<LoginSession>.Cancelled<LoginSession> _ => new Response.Cancelled(),
                Result<LoginSession>.Failure<LoginSession> failure => new Response.Failure(cause: failure.Cause),
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
