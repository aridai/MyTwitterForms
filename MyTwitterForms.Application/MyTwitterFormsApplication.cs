using MyTwitterForms.Application.Timeline;
using Prism.Ioc;

namespace MyTwitterForms.Application
{
    public static class MyTwitterFormsApplication
    {
        public static void Register(IContainerRegistry registry)
        {
            registry.Register<ITimelineFetchUseCase, StubTimelineFetchInteractor>();
        }
    }
}
