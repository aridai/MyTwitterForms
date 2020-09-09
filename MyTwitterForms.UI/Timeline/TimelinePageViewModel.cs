using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyTwitterForms.Application.Login.Status;
using MyTwitterForms.Application.Timeline;
using MyTwitterForms.UI.Login;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using static MyTwitterForms.Application.Timeline.ITimelineFetchUseCase;

namespace MyTwitterForms.UI.Timeline
{
    internal class TimelinePageViewModel : BindableBase, IInitializeAsync, IDestructible
    {
        private readonly INavigationService navigationService;
        private readonly ILoginStatusGetUseCase loginStatusGetUseCase;
        private readonly ITimelineFetchUseCase timelineFetchUseCase;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private CancellationTokenSource? cancellation = null;

        private readonly ObservableCollection<Tweet> tweets = new ObservableCollection<Tweet>();
        public ReadOnlyObservableCollection<Tweet> Tweets { get; }

        public ReactiveProperty<bool> IsRefreshing { get; } = new ReactiveProperty<bool>(initialValue: false);

        public TimelinePageViewModel(
            INavigationService navigationService,
            ILoginStatusGetUseCase loginStatusGetUseCase,
            ITimelineFetchUseCase timelineFetchUseCase
        )
        {
            this.navigationService = navigationService;
            this.loginStatusGetUseCase = loginStatusGetUseCase;
            this.timelineFetchUseCase = timelineFetchUseCase;

            this.Tweets = new ReadOnlyObservableCollection<Tweet>(this.tweets);

            this.IsRefreshing.DistinctUntilChanged()
                .Where(value => value)
                .Subscribe(_ => this.FetchTimeline())
                .AddTo(this.disposables);
        }

        //  タイムラインを取得する。
        private async void FetchTimeline()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("タイムライン取得開始");

                var cancellation = new CancellationTokenSource();
                this.cancellation = cancellation;

                var request = new Request(cancellation.Token);
                var response = await this.timelineFetchUseCase.Execute(request);

                switch (response)
                {
                    //  成功
                    case Response.Success success:
                        System.Diagnostics.Debug.WriteLine($"タイムライン取得成功: {success.Timeline.Count}件");
                        this.tweets.Clear();
                        foreach (var tweet in success.Timeline) this.tweets.Add(tweet.ToUiDto());
                        break;

                    //  その他 (未実装)
                    default:
                        System.Diagnostics.Debug.WriteLine("タイムライン取得失敗");
                        break;
                }

                this.IsRefreshing.Value = false;
            }
            finally
            {
                this.cancellation?.Dispose();
                this.cancellation = null;
            }
        }

        async Task IInitializeAsync.InitializeAsync(INavigationParameters parameters)
        {
            var request = new ILoginStatusGetUseCase.Request();
            var response = this.loginStatusGetUseCase.Execute(request);

            //  未ログインならばログイン画面に飛ばす。
            if (!response.IsLoggedIn) await this.navigationService.NavigateAsync(nameof(LoginPage));
        }

        void IDestructible.Destroy()
        {
            if (this.cancellation is CancellationTokenSource cancellation)
            {
                System.Diagnostics.Debug.WriteLine("キャンセル要求");

                cancellation.Cancel();
                cancellation.Dispose();
                this.cancellation = null;
            }
        }
    }
}
