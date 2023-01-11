# TweetSampler

This .NET 7 project demonstrates how to read a streaming http GET call, specifically the Twitter V2 Volume API api.twitter.com/2/tweets/sample/stream. This Twitter API returns 1% of the tweets being posted.

The application tracks the most frequent hash tags and returns them along with other stats in a REST API at

```
/api/TweetStatistics
```

There is a swagger endpoint at 

```
/swagger/index.html
```

In order to run this service, you must put your Twitter bearer token in **appsettings.json** under the key TwitterBearerToken.

```
{
  ...
  "TwitterBearerToken": "ADD_YOUR_BEARER_TOKEN_HERE"
}
```

![image](https://user-images.githubusercontent.com/10542297/211940854-540b53a3-d7ad-4756-96ac-ab31b6e45ce6.png)
