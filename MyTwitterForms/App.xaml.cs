using Prism.Ioc;

namespace MyTwitterForms
{
    public partial class App
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnInitialized() { }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainPage>(nameof(MainPage));
            containerRegistry.RegisterForNavigation<LoginPage>(nameof(LoginPage));
        }
    }
}
