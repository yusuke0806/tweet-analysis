using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweet.Analysis.Entities;

namespace Tweet.Analysis;

public class StoreTweet
{
    private const int MaxCount = 5;

    private static readonly string BearerToken = Environment.GetEnvironmentVariable("BearerToken");
    private static readonly string ApiKey = Environment.GetEnvironmentVariable("ApiKey");
    private static readonly string ApiSecret = Environment.GetEnvironmentVariable("ApiSecret");

    [FunctionName("StoreTweetData")]
    public async Task RunAsync([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
    {
        // Aim to get from the table in the future
        const string filteredKeywords = "";
        const int tweetStreamCounter = 0;
        await StartFilteredStream(filteredKeywords, tweetStreamCounter);
    }

    private static async Task StartFilteredStream(string filteredKeywords, int tweetStreamCounter)
    {
        var entity = new TweetEntity();
        Tweetinvi.Streaming.IFilteredStream stream = null;

        while (true)
        {
            try
            {
                var client = new TwitterClient(ApiKey, ApiSecret, BearerToken);
                stream = client.Streams.CreateFilteredStream();
            
                var keywords = filteredKeywords.Split(' ', '　');
                foreach (var keyword in keywords)
                {
                    stream.AddTrack(keyword);
                }
        
                // Read stream
                stream.MatchingTweetReceived += (sender, args) =>
                {
                    var lang = args.Tweet.Language;
                    if (lang == Tweetinvi.Models.Language.Japanese && args.Tweet.Source.Contains(">Twitter "))
                    {
                        entity.CreatedAt.Add(args.Tweet.CreatedAt);
                        entity.CreatedBy.Add(args.Tweet.CreatedBy.ToString());
                        entity.Source.Add(args.Tweet.Source);
                        entity.Text.Add(args.Tweet.Text);
                    }

                    ++tweetStreamCounter;
                    if (tweetStreamCounter > MaxCount)
                    {
                        stream?.Stop();
                    }
                };
            
                await stream.StartMatchingAllConditionsAsync();
                break;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("The response ended prematurely."))
                {
                    stream?.Stop();
                    await Task.Delay(1000);
                }
                else
                {
                    throw;
                }
            }
        }
        
        await CreateParquetFile(entity);
    }

    private static async Task CreateParquetFile(TweetEntity entity)
    {
        
    }
}
