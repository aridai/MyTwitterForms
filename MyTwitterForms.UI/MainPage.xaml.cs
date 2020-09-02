using System;
using System.Linq;
using MyTwitterForms.UI.Timeline;
using Xamarin.Forms;

namespace MyTwitterForms.UI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.BindingContext = Enumerable.Range(1, 10).Select(_ =>
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
        }
    }
}
