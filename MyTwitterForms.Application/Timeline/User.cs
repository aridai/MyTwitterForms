namespace MyTwitterForms.Application.Timeline
{
    public class User
    {
        //  ユーザID
        public long UserId { get; }

        //  ユーザ名
        public string UserName { get; }

        //  スクリーンネーム
        public string ScreenName { get; }

        //  ユーザのアイコン画像のURL
        public string UserIconUrl { get; }

        public User(
            long userId,
            string userName,
            string screenName,
            string userIconUrl
        )
        {
            this.UserId = userId;
            this.UserName = userName;
            this.ScreenName = screenName;
            this.UserIconUrl = userIconUrl;
        }
    }
}
