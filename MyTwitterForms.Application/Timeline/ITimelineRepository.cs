using System;
using System.Threading;
using System.Threading.Tasks;
using MyTwitterForms.Model;

namespace MyTwitterForms.Application.Timeline
{
    public interface ITimelineRepository
    {
        //  タイムラインを取得する。
        Task<Result<Timeline>> FetchTimeline(ApiKeys keys, AccessTokens tokens, CancellationToken cancellation);

        public abstract class Result<T>
        {
            private Result() { }

            //  成功
            public class Success<TResult> : Result<TResult>
            {
                public TResult Value { get; }

                public Success(TResult value) : base()
                {
                    this.Value = value;
                }
            }

            //  キャンセル
            public class Cancelled<TResult> : Result<TResult>
            {
                public Cancelled() : base() { }
            }

            //  無効なトークン
            public class InvalidToken<TResult> : Result<TResult>
            {
                public InvalidToken() : base() { }
            }

            // サーバ側のエラー
            public class ServerError<TResult> : Result<TResult>
            {
                public Exception Cause { get; }

                public ServerError(Exception cause) : base()
                {
                    this.Cause = cause;
                }
            }

            //  端末のネットワークエラー
            public class NetworkError<TResult> : Result<TResult>
            {
                public Exception Cause { get; }

                public NetworkError(Exception cause) : base()
                {
                    this.Cause = cause;
                }
            }
        }
    }
}
