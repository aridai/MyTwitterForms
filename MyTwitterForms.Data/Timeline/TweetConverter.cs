using System.Linq;
using MyTwitterForms.Application.Timeline;

namespace MyTwitterForms.Data.Timeline
{
    internal static class TweetConverter
    {
        public static Tweet Convert(CoreTweet.Status source) =>
            new Tweet(
                id: source.Id,
                tweetUrl: $"https://twitter.com/{source.User.ScreenName}/status/{source.Id}",
                user: ConvertUser(source.User),
                postedAt: source.CreatedAt.LocalDateTime,
                body: source.Text,
                imageUrls: source.Entities.Urls.Select(e => e.ExpandedUrl).ToList()
            );

        private static User ConvertUser(CoreTweet.User source) =>
            new User(
                userId: source.Id ?? -1L,
                userName: source.Name,
                screenName: source.ScreenName,
                userIconUrl: source.ProfileImageUrlHttps
            );
    }
}
