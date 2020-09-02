using System;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Mvvm;
using Prism.Navigation;

namespace MyTwitterForms.UI.Timeline
{
    internal class TimelinePageViewModel : BindableBase
    {
        private readonly INavigationService navigationService;

        private readonly ObservableCollection<Tweet> tweets;
        public ReadOnlyObservableCollection<Tweet> Tweets { get; }

        public TimelinePageViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;

            //  ダミーデータ
            var dummyTweets = Enumerable.Range(1, 10).Select(_ =>
                new Tweet(
                    id: 1L,
                    tweetUrl: "https://twitter.com/aridai_net/status/1275051468005900290",
                    userName: "aridai",
                    screenName: "aridai_net",
                    userIconUrl: "https://pbs.twimg.com/profile_images/1035113789601923072/BKF60R8m_400x400.jpg",
                    postedAt: DateTime.Now,
                    body: "テスト",
                    imageUrls: new string[0]
                )
            ).ToList();

            this.tweets = new ObservableCollection<Tweet>(dummyTweets);
            this.Tweets = new ReadOnlyObservableCollection<Tweet>(this.tweets);
        }
    }
}
