using static MyTwitterForms.Application.Login.Status.ILoginStatusGetUseCase;

namespace MyTwitterForms.Application.Login.Status
{
    internal class LoginStatusGetInteractor : ILoginStatusGetUseCase
    {
        private readonly ILoginRepository repository;

        public LoginStatusGetInteractor(ILoginRepository repository)
        {
            this.repository = repository;
        }

        Response ILoginStatusGetUseCase.Execute(Request request)
        {
            var tokens = this.repository.GetAccessTokens();
            var isLoggedIn = (tokens != null);

            return new Response(isLoggedIn);
        }
    }
}
