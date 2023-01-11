namespace TweetSampler.Services.Models
{
    public class HasTagCount
    {
        public string Tag { get; set; }
        public int Count { get; set; }
    }

    public class TweetStatistics
    {
        public long TweetCount { get; set; }
        public long UniqueHashTagCount { get; set; }
        public int TweetsPerSecond { get; set; }
        public int MaxTweetsPerSecond { get; set; }
        public List<HasTagCount> TopHashTags { get; set; }
    }
}
