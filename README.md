# TweetSampler

This .NET 7 project demonstrates how to read a streaming http GET call, specifically the Twitter V2 Volume API api.twitter.com/2/tweets/sample/stream. This Twitter API returns 1% of the tweets being posted.

The application tracks the most frequent hash tags and returns them along with other stats in a REST API at

```
/api/TweetStatistics
```
The endpoint returns the following data:

* tweetCount - total tweets processed
* uniqueHashTagCount - total number of unique hash tags found
* tweetsPerSecond - number of tweets received in the last second
* maxTweetsPerSecond - the highest tweetsPerSecond observed
* topHashTags - a list of the most frequent hash tags and their occurance count

There is one optional parameter hashTagNumber that can be used to change the number of topHashTags returned. The default is 10. e.g.

```
/api/TweetStatistics?hashTagNumber=100
```


There is a swagger endpoint at 

```
/swagger/index.html
```

In order to run this service, you must put your Twitter bearer token in **appsettings.json** under the key **TwitterBearerToken**.

```
{
  ...
  "TwitterBearerToken": "ADD_YOUR_BEARER_TOKEN_HERE"
}
```

![image](https://user-images.githubusercontent.com/10542297/211940854-540b53a3-d7ad-4756-96ac-ab31b6e45ce6.png)
