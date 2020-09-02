namespace MyTwitterForms.UI.Timeline
{
    internal static class TweetConverter
    {
        public static Tweet ToUiDto(this Application.Timeline.Tweet source) =>
            new Tweet(
                id: source.Id,
                tweetUrl: source.TweetUrl,
                userName: source.User.UserName,
                screenName: source.User.ScreenName,
                userIconUrl: source.User.UserIconUrl,
                postedAt: source.PostedAt,
                body: source.Body,
                imageUrls: source.ImageUrls
            );
    }
}
