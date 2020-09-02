using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyTwitterForms.Application.Timeline
{
    //  タイムラインを取得する。
    public interface ITimelineFetchUseCase
    {
        Task<Response> Execute(Request request);

        public struct Request
        {
            public CancellationToken CancellationToken { get; }

            public Request(CancellationToken token)
            {
                this.CancellationToken = token;
            }
        }

        public abstract class Response
        {
            private Response() { }

            //  成功
            public class Success : Response
            {
                public Timeline Timeline { get; }

                public Success(Timeline timeline) : base()
                {
                    this.Timeline = timeline;
                }
            }

            //  キャンセル
            public class Cancelled : Response
            {
                public Cancelled() : base() { }
            }

            //  ネットワークエラー (端末側)
            public class NetworkError : Response
            {
                public Exception Cause { get; }

                public NetworkError(Exception cause) : base()
                {
                    this.Cause = cause;
                }
            }

            //  サーバエラー (Twitter側)
            public class ServerError : Response
            {
                public Exception Cause { get; }

                public ServerError(Exception cause) : base()
                {
                    this.Cause = cause;
                }
            }

            //  無効トークンエラー
            public class InvalidTokenError : Response
            {
                public InvalidTokenError() : base() { }
            }
        }
    }
}
