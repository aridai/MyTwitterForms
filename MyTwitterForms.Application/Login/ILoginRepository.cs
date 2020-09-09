using System;
using System.Threading;
using System.Threading.Tasks;
using MyTwitterForms.Model;

namespace MyTwitterForms.Application.Login
{
    public interface ILoginRepository
    {
        //  ログインセッションを開始する。
        Task<Result<LoginSession>> BeginLoginSession(CancellationToken cancellation);

        //  アクセストークンを取得する。
        Task<Result<AccessTokens>> ObtainAccessTokens(string sessionId, string pinCode, CancellationToken cancellation);

        //  アクセストークンを保存する。
        void SaveAccessTokens(AccessTokens accessTokens);

        //  保存されているアクセストークンを取得する。
        AccessTokens? GetAccessTokens();

        public abstract class Result<T>
        {
            private Result() { }

            public class Success<TResult> : Result<TResult>
            {
                public TResult Value { get; }

                public Success(TResult value) : base()
                {
                    this.Value = value;
                }
            }

            public class Cancelled<TResult> : Result<TResult>
            {
                public Cancelled() : base() { }
            }

            public class Failure<TResult> : Result<TResult>
            {
                public Exception Cause { get; }

                public Failure(Exception cause) : base()
                {
                    this.Cause = cause;
                }
            }
        }
    }
}
