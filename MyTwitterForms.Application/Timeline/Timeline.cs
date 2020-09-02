using System;
using System.Collections;
using System.Collections.Generic;

namespace MyTwitterForms.Application.Timeline
{
    //  取得したタイムライン
    public class Timeline : IReadOnlyList<Tweet>
    {
        private readonly IReadOnlyList<Tweet> tweets;

        public Tweet this[int index] => this.tweets[index];

        public DateTime FetchedAt { get; }

        public int Count => this.tweets.Count;

        public Timeline(DateTime fetchedAt, IReadOnlyList<Tweet> tweets)
        {
            this.FetchedAt = fetchedAt;
            this.tweets = tweets;
        }

        public IEnumerator<Tweet> GetEnumerator() => this.tweets.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.tweets.GetEnumerator();
    }
}
