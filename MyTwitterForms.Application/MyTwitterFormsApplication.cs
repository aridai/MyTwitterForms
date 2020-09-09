using MyTwitterForms.Application.Login.Session;
using MyTwitterForms.Application.Login.Status;
using MyTwitterForms.Application.Login.Tokens;
using MyTwitterForms.Application.Timeline;
using Prism.Ioc;

namespace MyTwitterForms.Application
{
    public static class MyTwitterFormsApplication
    {
        public static void Register(IContainerRegistry registry)
        {
            registry.Register<ITimelineFetchUseCase, TimelineFetchInteractor>();
            registry.Register<ILoginSessionBeginUseCase, LoginSessionBeginInteractor>();
            registry.Register<IAccessTokensObtainUseCase, AccessTokensObtainInteractor>();
            registry.Register<ILoginStatusGetUseCase, LoginStatusGetInteractor>();
        }
    }
}
