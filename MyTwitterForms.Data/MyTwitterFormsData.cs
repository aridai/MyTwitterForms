using MyTwitterForms.Application.Login;
using MyTwitterForms.Application.Timeline;
using MyTwitterForms.Data.Login;
using MyTwitterForms.Data.Timeline;
using Prism.Ioc;

namespace MyTwitterForms.Data
{
    public static class MyTwitterFormsData
    {
        public static void Register(IContainerRegistry registry)
        {
            registry.RegisterSingleton<ILoginRepository, LoginRepository>();
            registry.RegisterSingleton<ITimelineRepository, TimelineRepository>();
        }
    }
}
