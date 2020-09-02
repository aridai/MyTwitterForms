using MyTwitterForms.UI.Timeline;
using Prism.Ioc;

namespace MyTwitterForms.UI
{
    public static class MyTwitterFormsUI
    {
        public static void Register(IContainerRegistry registry)
        {
            registry.RegisterForNavigation<MainPage>(nameof(MainPage));
            registry.RegisterForNavigation<TimelinePage>(nameof(TimelinePage));
        }
    }
}
