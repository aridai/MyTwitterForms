namespace MyTwitterForms.Application.Login.Status
{
    //  ログイン状況を取得する。
    public interface ILoginStatusGetUseCase
    {
        Response Execute(Request request);

        public class Request { }
        public class Response
        {
            public bool IsLoggedIn { get; }

            public Response(bool isLoggedIn)
            {
                this.IsLoggedIn = isLoggedIn;
            }
        }
    }
}
