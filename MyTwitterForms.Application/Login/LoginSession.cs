namespace MyTwitterForms.Application.Login
{
    //  ログインのセッション
    public class LoginSession
    {
        public string SessionId { get; }

        public string AuthorizeUrl { get; }

        public LoginSession(string sessionId, string authorizeUrl)
        {
            this.SessionId = sessionId;
            this.AuthorizeUrl = authorizeUrl;
        }
    }
}
