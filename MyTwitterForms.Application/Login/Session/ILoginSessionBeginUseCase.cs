using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyTwitterForms.Application.Login.Session
{
    //  ログインセッションを開始する。
    public interface ILoginSessionBeginUseCase
    {
        Task<Response> Execute(Request request);

        public sealed class Request
        {
            public CancellationToken Cancellation { get; }

            public Request(CancellationToken cancellation)
            {
                this.Cancellation = cancellation;
            }

            public Request() : this(CancellationToken.None) { }
        }

        public abstract class Response
        {
            private Response() { }

            public class Success : Response
            {
                public LoginSession Session { get; }

                public Success(LoginSession session) : base()
                {
                    this.Session = session;
                }
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
