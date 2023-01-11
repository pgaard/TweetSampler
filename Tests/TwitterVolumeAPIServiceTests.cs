using Moq;
using NUnit.Framework;
using TweetSampler.Services;

namespace TweetSampler.Tests
{
    [TestFixture]
    public class TwitterVolumeAPIServiceTests
    {
        private TwitterVolumeAPIService service;

        [SetUp]
        public void SetUp()
        {
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["TwitterBearerToken"]).Returns("test");

            var loggerMock = new Mock<ILogger<TwitterVolumeAPIService>>();
            service = new TwitterVolumeAPIService(configurationMock.Object, loggerMock.Object);
        }
        
        [Test]
        public void ShouldThrowIfTwitterBearerTokenNotConfigured()
        {
            var configurationMock = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<TwitterVolumeAPIService>>();

            Assert.Throws<Exception>(() =>
            {
                var throwService = new TwitterVolumeAPIService(configurationMock.Object, loggerMock.Object);
            });
        }

        [Test]
        public void ShouldSetCorrectUniqueHashCodeCount()
        {
            var testTweet = "this is #ahashtag and here is #anotherhashtag";
            service.ProcessHashTags(testTweet);

            var result = service.GetCurrentStatistics(10);

            Assert.AreEqual(2, result.UniqueHashTagCount);
        }

        [Test]
        public void ShouldNotReturnSingleHitHashTags()
        {
            var testTweet = "this is #ahashtag and here is #anotherhashtag";
            service.ProcessHashTags(testTweet);

            var result = service.GetCurrentStatistics(10);

            Assert.AreEqual(0, result.TopHashTags.Count);
        }

        [Test]
        public void ShouldFindMultipleTopHashTagsWithMultipleHits()
        {
            var testTweet = "this is #ahashtag and here is #anotherhashtag";
            service.ProcessHashTags(testTweet);
            service.ProcessHashTags(testTweet);

            var result = service.GetCurrentStatistics(10);

            Assert.AreEqual(2, result.TopHashTags.Count);
            Assert.IsTrue(result.TopHashTags.Any(x => x.Tag == "ahashtag"));
            Assert.IsTrue(result.TopHashTags.Any(x => x.Tag == "anotherhashtag"));
            Assert.IsTrue(result.TopHashTags.All(x => x.Count == 2));
        }

        [Test]
        public void ShouldLimitTopHashTagsToParameter()
        {
            var testTweet = "this is #ahashtag and here is #anotherhashtag";
            service.ProcessHashTags(testTweet);
            service.ProcessHashTags(testTweet);

            var result = service.GetCurrentStatistics(1);

            Assert.AreEqual(1, result.TopHashTags.Count);
        }

        [Test]
        public void ShouldFindHashTagsWithUnderscores()
        {
            var testTweet = "this is #a_hash_tag";
            service.ProcessHashTags(testTweet);
            service.ProcessHashTags(testTweet);

            var result = service.GetCurrentStatistics(10);

            Assert.AreEqual(1, result.UniqueHashTagCount);
            Assert.IsTrue(result.TopHashTags.Any(x => x.Tag == "a_hash_tag"));
        }

        [Test]
        public void ShouldFindHashTagsWithNumbers()
        {
            var testTweet = "this is #a2hash23tag";
            service.ProcessHashTags(testTweet);
            service.ProcessHashTags(testTweet);

            var result = service.GetCurrentStatistics(10);

            Assert.AreEqual(1, result.UniqueHashTagCount);
            Assert.IsTrue(result.TopHashTags.Any(x => x.Tag == "a2hash23tag"));
        }
    }
}
