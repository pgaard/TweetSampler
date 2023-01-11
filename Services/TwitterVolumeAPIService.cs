using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using TweetSampler.Services.Models;

namespace TweetSampler.Services
{
    public partial class TwitterVolumeAPIService : ITwitterVolumeAPIService
    {
        private const string TwitterVolumeAPIUrl = "https://api.twitter.com/2/tweets/sample/stream";
        private const string BearerTokenConfig = "TwitterBearerToken";

        private readonly string? TwitterBearerToken;        
        private readonly ConcurrentDictionary<string, int> hashTagCounts;
        private readonly ILogger logger;
        private readonly Regex hashTagRegEx;

        private int tweetCount;
        private int tweetsPerSecond;
        private int maxTweetsPerSecond = 0;

        public TwitterVolumeAPIService(IConfiguration configuration, ILogger<TwitterVolumeAPIService> logger)
        {
            this.logger = logger;
            if (configuration == null || configuration[BearerTokenConfig] == null || configuration[BearerTokenConfig] == "")
            {
                throw (new Exception("you must set the BearerTokenConfig setting in appsettings.json"));
            }

            if (configuration != null)
            {
                TwitterBearerToken = configuration[BearerTokenConfig];
            }

            hashTagCounts = new ConcurrentDictionary<string, int>();
            tweetCount = 0;
            hashTagRegEx = new Regex("#[a-zA-Z0-9_]+");
        }

        /// <summary>
        /// Get the current statistics about the tweet stream
        /// </summary>
        /// <param name="numberOfHashTags">Number of top hash tags to return (defaul 10)</param>
        /// <returns>Tweet Stastistcs</returns>
        public TweetStatistics GetCurrentStatistics(int numberOfHashTags)
        {
            var digest = new TweetStatistics {
                TweetCount = tweetCount,
                UniqueHashTagCount = hashTagCounts.Count,
                TweetsPerSecond = tweetsPerSecond,
                MaxTweetsPerSecond = maxTweetsPerSecond
            };

            // leave out tags with one hit to optimize sorting
            var hashTagList = hashTagCounts.Where(x => x.Value > 1).ToList();

            hashTagList.Sort((x, y) => { return y.Value - x.Value; }); // descending on count sort
            digest.TopHashTags = hashTagList
                .Take(Math.Min(numberOfHashTags, 500))
                .Select(x => new HasTagCount { Tag = x.Key, Count = x.Value } )
                .ToList();
            return digest;
        }

        /// <summary>
        /// Start a background thread that continuously ingests the stream of tweets
        /// </summary>
        public void StartListening()
        {
            Task.Run(() => this.Reader());
        }

        private async void Reader()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TwitterBearerToken);    

            try
            {
                var stream = await httpClient.GetStreamAsync(TwitterVolumeAPIUrl);
                var reader = new StreamReader(stream, System.Text.Encoding.ASCII, false);
                
                logger.LogInformation("Started to read twitter feed");

                var tweetsThisSecond = 0;
                var currentSecond = DateTime.Now.Second;

                while (!reader.EndOfStream)
                {
                    tweetCount++;
                    tweetsThisSecond++;

                    var tweet = reader.ReadLine();
                    if (tweet != null)
                    {
                        ProcessHashTags(tweet);
                    }

                    // track some rate information
                    var second = DateTime.Now.Second;
                    if (currentSecond != second) {
                        tweetsPerSecond = tweetsThisSecond;
                        currentSecond = second;
                        tweetsThisSecond = 0;
                        if (tweetsPerSecond > maxTweetsPerSecond)
                        {
                            maxTweetsPerSecond = tweetsPerSecond;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                logger.LogError("There was an error reading the twitter volume API", ex);
            }
        }

        public void ProcessHashTags(string tweet)
        {            
            var hashTags = hashTagRegEx.Matches(tweet);
            foreach (Match tag in hashTags)
            {
                var hashText = tag.Value.Substring(1);
                if (hashTagCounts.ContainsKey(hashText))
                {
                    hashTagCounts[hashText]++;
                }
                else
                {
                    hashTagCounts[hashText] = 1;
                }
            }
        }
    }
}
