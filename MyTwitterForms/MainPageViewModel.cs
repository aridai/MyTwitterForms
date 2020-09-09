using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CoreTweet;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace MyTwitterForms
{
    internal class MainPageViewModel : BindableBase, INavigationAware
    {
        private readonly INavigationService navigationService;

        private readonly ObservableCollection<Status> tweets = new ObservableCollection<Status>();
        public ReadOnlyObservableCollection<Status> Tweets { get; }

        public ICommand BackButtonCommand { get; }
        public ICommand LoginButtonCommand { get; }

        public MainPageViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            this.Tweets = new ReadOnlyObservableCollection<Status>(this.tweets);
            this.BackButtonCommand = new DelegateCommand(this.Back);
            this.LoginButtonCommand = new DelegateCommand(this.ToLoginPage);

            this.navigationService.NavigateAsync(nameof(LoginPage)).Wait();
        }

        private async void Back()
        {
            await this.navigationService.GoBackAsync();
        }

        private async void ToLoginPage()
        {
            await this.navigationService.NavigateAsync("TwitterLoginPage");
        }

        private async void FetchTimeline(Tokens tokens)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"タイムライン取得開始");
                var response = (await tokens.Statuses.HomeTimelineAsync(count: 100))!;

                this.tweets.Clear();
                foreach (var tweet in response) this.tweets.Add(tweet);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"タイムライン取得エラー: {e}");
            }
        }

        void INavigatedAware.OnNavigatedFrom(INavigationParameters parameters) { }

        void INavigatedAware.OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters["TOKENS"] is Tokens tokens) this.FetchTimeline(tokens);
        }
    }
}
