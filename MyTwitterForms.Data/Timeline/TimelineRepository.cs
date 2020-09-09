using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoreTweet;
using MyTwitterForms.Application.Timeline;
using MyTwitterForms.Model;
using static MyTwitterForms.Application.Timeline.ITimelineRepository;
using TL = MyTwitterForms.Application.Timeline.Timeline;

namespace MyTwitterForms.Data.Timeline
{
    internal class TimelineRepository : ITimelineRepository
    {
        private const int Count = 100;

        public async Task<Result<TL>> FetchTimeline(ApiKeys keys, AccessTokens tokens, CancellationToken cancellation)
        {
            try
            {
                var twitterTokens = Tokens.Create(
                    consumerKey: keys.ApiKey,
                    consumerSecret: keys.ApiSecretKey,
                    accessToken: tokens.AccessToken,
                    accessSecret: tokens.AccessTokenSecret
                );

                var response = await twitterTokens.Statuses.HomeTimelineAsync(
                    count: Count,
                    cancellationToken: cancellation
                );

                var tweets = response.Select(TweetConverter.Convert).ToList();
                var timeline = new TL(fetchedAt: DateTime.Now, tweets: tweets);

                return new Result<TL>.Success<TL>(timeline);
            }

            catch (OperationCanceledException)
            {
                return new Result<TL>.Cancelled<TL>();
            }

            catch (TwitterException e)
            {
                if (e.Status == HttpStatusCode.Unauthorized) return new Result<TL>.InvalidToken<TL>();
                else return new Result<TL>.ServerError<TL>(e);
            }

            catch (HttpRequestException e)
            {
                return new Result<TL>.NetworkError<TL>(e);
            }
        }
    }
}
