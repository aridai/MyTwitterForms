using MyTwitterForms.UI.Timeline;
using Prism.Mvvm;
using Prism.Navigation;

namespace MyTwitterForms.UI
{
    internal class MainPageViewModel : BindableBase
    {
        private readonly INavigationService navigationService;

        public MainPageViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;

            this.ToTimelinePage();
        }

        //  タイムライン画面に遷移する。
        private async void ToTimelinePage()
        {
            await this.navigationService.NavigateAsync(nameof(TimelinePage));
        }
    }
}
