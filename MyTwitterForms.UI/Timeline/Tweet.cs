using System;
using System.Collections.Generic;
using Prism.Mvvm;

namespace MyTwitterForms.UI.Timeline
{
    //  ツイートのUI用モデル
    internal class Tweet : BindableBase
    {
        //  ツイートのID
        public long Id { get; }

        //  ツイートのURL
        public string TweetUrl { get; }

        //  ユーザ名
        public string UserName { get; }

        //  スクリーンネーム
        public string ScreenName { get; }

        //  ユーザのアイコン画像のURL
        public string UserIconUrl { get; }

        //  ツイートの投稿日時
        public DateTime PostedAt { get; }

        //  ツイートの本文
        public string Body { get; }

        //  ツイートの画像のURLのリスト
        public IReadOnlyList<string> ImageUrls { get; }

        public Tweet(
            long id,
            string tweetUrl,
            string userName,
            string screenName,
            string userIconUrl,
            DateTime postedAt,
            string body,
            IReadOnlyList<string> imageUrls
        )
        {
            this.Id = id;
            this.TweetUrl = tweetUrl;
            this.UserName = userName;
            this.ScreenName = screenName;
            this.UserIconUrl = userIconUrl;
            this.PostedAt = postedAt;
            this.Body = body;
            this.ImageUrls = imageUrls;
        }
    }
}
