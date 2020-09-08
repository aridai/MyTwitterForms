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

            return result switch
            {
                //  TODO: アクセストークンの保存処理を追加
                Result<AccessTokens>.Success<AccessTokens> _ => new Response.Success(),
                Result<AccessTokens>.Cancelled<AccessTokens> _ => new Response.Cancelled(),
                Result<AccessTokens>.Failure<AccessTokens> failure => new Response.Failure(cause: failure.Cause),
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
