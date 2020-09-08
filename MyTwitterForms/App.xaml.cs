using MyTwitterForms.Application;
using MyTwitterForms.Data;
using MyTwitterForms.Model;
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
            containerRegistry.RegisterInstance(new ApiKeys(Env.ApiKey, Env.ApiSecretKey));

            MyTwitterFormsUI.Register(containerRegistry);
            MyTwitterFormsApplication.Register(containerRegistry);
            MyTwitterFormsModel.Register(containerRegistry);
            MyTwitterFormsData.Register(containerRegistry);
        }
    }
}
