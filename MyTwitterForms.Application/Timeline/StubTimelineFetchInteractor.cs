using System;
using System.Linq;
using System.Threading.Tasks;
using static MyTwitterForms.Application.Timeline.ITimelineFetchUseCase;

namespace MyTwitterForms.Application.Timeline
{
    internal class StubTimelineFetchInteractor : ITimelineFetchUseCase
    {
        async Task<Response> ITimelineFetchUseCase.Execute(Request request)
        {
            try
            {
                await Task.Delay(3000, request.CancellationToken);

                var tweets = Enumerable.Range(1, 10).Select(_ =>
                    new Tweet(
                        id: 1L,
                        tweetUrl: "https://twitter.com/aridai_net/status/1275051468005900290",
                        user: new User(
                            userId: 1L,
                            userName: "aridai",
                            screenName: "aridai_net",
                            userIconUrl: "https://pbs.twimg.com/profile_images/1035113789601923072/BKF60R8m_400x400.jpg"
                        ),
                        postedAt: DateTime.Now,
                        body: "テスト",
                        imageUrls: new string[0]
                    )
                ).ToList();
                var timeline = new Timeline(fetchedAt: DateTime.Now, tweets: tweets);
                return new Response.Success(timeline);
            }
            catch (OperationCanceledException)
            {
                return new Response.Cancelled();
            }
        }
    }
}
