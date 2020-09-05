using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyTwitterForms.Application.Login.Tokens
{
    //  トークンを取得する。
    public interface IAccessTokensObtainUseCase
    {
        Task<Response> Execute(Request request);

        public class Request
        {
            public string SessionId { get; }

            public CancellationToken Cancellation { get; }

            public Request(string sessionId, CancellationToken cancellation)
            {
                this.SessionId = sessionId;
                this.Cancellation = cancellation;
            }

            public Request(string sessionId) : this(sessionId, CancellationToken.None) { }
        }

        public abstract class Response
        {
            private Response() { }

            public class Success : Response
            {
                public Success() : base() { }
            }

            public class Failure : Response
            {
                public Exception Cause { get; }

                public Failure(Exception cause) : base()
                {
                    this.Cause = cause;
                }
            }

            public class Cancelled : Response
            {
                public Cancelled() : base() { }
            }
        }
    }
}
