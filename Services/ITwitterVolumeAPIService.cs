using TweetSampler.Services.Models;

namespace TweetSampler.Services
{
    public interface ITwitterVolumeAPIService
    {
        TweetStatistics GetCurrentStatistics(int numberOfHashTags);
        void StartListening();
    }
}
