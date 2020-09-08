namespace MyTwitterForms.Model
{
    //  アクセストークン
    public class AccessTokens
    {
        public string AccessToken { get; }

        public string AccessTokenSecret { get; }

        public AccessTokens(string accessToken, string accessTokenSecret)
        {
            this.AccessToken = accessToken;
            this.AccessTokenSecret = accessTokenSecret;
        }
    }
}
