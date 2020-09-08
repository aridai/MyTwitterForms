using MyTwitterForms.Application.Login;
using MyTwitterForms.Data.Login;
using Prism.Ioc;

namespace MyTwitterForms.Data
{
    public static class MyTwitterFormsData
    {
        public static void Register(IContainerRegistry registry)
        {
            registry.RegisterInstance<ILoginRepository>(new StubLoginRepository());
        }
    }
}
