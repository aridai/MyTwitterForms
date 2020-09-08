using MyTwitterForms.Application.Login;
using MyTwitterForms.Data.Login;
using MyTwitterForms.Model;
using Prism.Ioc;

namespace MyTwitterForms.Data
{
    public static class MyTwitterFormsData
    {
        public static void Register(IContainerRegistry registry)
        {
            //  TODO: ApiKeysを外部の環境変数から設定できるように修正
            registry.RegisterInstance(new ApiKeys(apiKey: "API_KEY", apiSecretKey: "SECRET"));

            registry.RegisterSingleton<ILoginRepository, LoginRepository>();
        }
    }
}
