using System;
using System.Collections.Generic;

namespace MyTwitterForms.Application.Timeline
{
    public class Tweet
    {
        //  ツイートのID
        public long Id { get; }

        //  ツイートのURL
        public string TweetUrl { get; }

        //  ユーザ
        public User User { get; }

        //  ツイートの投稿日時
        public DateTime PostedAt { get; }

        //  ツイートの本文
        public string Body { get; }

        //  ツイートの画像のURLのリスト
        public IReadOnlyList<string> ImageUrls { get; }

        public Tweet(
            long id,
            string tweetUrl,
            User user,
            DateTime postedAt,
            string body,
            IReadOnlyList<string> imageUrls
        )
        {
            this.Id = id;
            this.TweetUrl = tweetUrl;
            this.User = user;
            this.PostedAt = postedAt;
            this.Body = body;
            this.ImageUrls = imageUrls;
        }
    }
}
