using MyTwitterForms.Application;
using MyTwitterForms.UI;
using Prism.Ioc;
using Prism.Unity;

namespace MyTwitterForms
{
    public partial class App : PrismApplication
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnInitialized() { }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            MyTwitterFormsUI.Register(containerRegistry);
            MyTwitterFormsApplication.Register(containerRegistry);
        }
    }
}
