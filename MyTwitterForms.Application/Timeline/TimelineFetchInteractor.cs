using System;
using System.Threading.Tasks;
using MyTwitterForms.Application.Login;
using MyTwitterForms.Model;
using static MyTwitterForms.Application.Timeline.ITimelineFetchUseCase;
using static MyTwitterForms.Application.Timeline.ITimelineRepository;

namespace MyTwitterForms.Application.Timeline
{
    internal class TimelineFetchInteractor : ITimelineFetchUseCase
    {
        private readonly ApiKeys apiKeys;
        private readonly ILoginRepository loginRepository;
        private readonly ITimelineRepository timelineRepository;

        public TimelineFetchInteractor(
            ApiKeys apiKeys,
            ILoginRepository loginRepository,
            ITimelineRepository timelineRepository
        )
        {
            this.apiKeys = apiKeys;
            this.loginRepository = loginRepository;
            this.timelineRepository = timelineRepository;
        }

        async Task<Response> ITimelineFetchUseCase.Execute(Request request)
        {
            var tokens = this.loginRepository.GetAccessTokens();
            if (tokens == null) return new Response.InvalidTokenError();

            var result = await this.timelineRepository.FetchTimeline(this.apiKeys, tokens, request.CancellationToken);

            return result switch
            {
                Result<Timeline>.Success<Timeline> success => new Response.Success(success.Value),
                Result<Timeline>.Cancelled<Timeline> _ => new Response.Cancelled(),
                Result<Timeline>.InvalidToken<Timeline> _ => new Response.InvalidTokenError(),
                Result<Timeline>.ServerError<Timeline> error => new Response.ServerError(error.Cause),
                Result<Timeline>.NetworkError<Timeline> error => new Response.NetworkError(error.Cause),
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
