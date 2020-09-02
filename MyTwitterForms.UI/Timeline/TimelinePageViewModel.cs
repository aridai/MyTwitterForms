using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using MyTwitterForms.Application.Timeline;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using static MyTwitterForms.Application.Timeline.ITimelineFetchUseCase;

namespace MyTwitterForms.UI.Timeline
{
    internal class TimelinePageViewModel : BindableBase, IDestructible
    {
        private readonly INavigationService navigationService;
        private readonly ITimelineFetchUseCase timelineFetchUseCase;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private CancellationTokenSource? cancellation = null;

        private readonly ObservableCollection<Tweet> tweets = new ObservableCollection<Tweet>();
        public ReadOnlyObservableCollection<Tweet> Tweets { get; }

        public ReactiveProperty<bool> IsRefreshing { get; } = new ReactiveProperty<bool>(initialValue: false);

        public TimelinePageViewModel(INavigationService navigationService, ITimelineFetchUseCase timelineFetchUseCase)
        {
            this.navigationService = navigationService;
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
