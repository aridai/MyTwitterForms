using System;
using System.Threading.Tasks;
using MyTwitterForms.Model;
using static MyTwitterForms.Application.Login.ILoginRepository;
using static MyTwitterForms.Application.Login.Tokens.IAccessTokensObtainUseCase;

namespace MyTwitterForms.Application.Login.Tokens
{
    internal class AccessTokensObtainInteractor : IAccessTokensObtainUseCase
    {
        private readonly ILoginRepository repository;

        public AccessTokensObtainInteractor(ILoginRepository repository)
        {
            this.repository = repository;
        }

        async Task<Response> IAccessTokensObtainUseCase.Execute(Request request)
        {
            var result = await this.repository.ObtainAccessTokens(
                sessionId: request.SessionId,
                pinCode: request.PinCode,
                cancellation: request.Cancellation
            );

            switch (result)
            {
                case Result<AccessTokens>.Success<AccessTokens> success:
                    this.repository.SaveAccessTokens(accessTokens: success.Value);
                    return new Response.Success();

                case Result<AccessTokens>.Cancelled<AccessTokens> _:
                    return new Response.Cancelled();

                case Result<AccessTokens>.Failure<AccessTokens> failure:
                    return new Response.Failure(cause: failure.Cause);

                default: throw new InvalidOperationException();
            }
        }
    }
}
