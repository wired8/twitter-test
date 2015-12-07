using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using twitter_api.Models;

namespace twitter_api.Services
{
   
    public class TwitterService : ITwitterService
    {
        private readonly IRestClient _client;

        /// <summary>
        /// Twitter api search service 
        /// </summary>
        /// <param name="restClient"></param>
        /// <param name="consumerKey"></param>
        /// <param name="consumerSecret"></param>
        /// <param name="accessToken"></param>
        /// <param name="accessTokenSecret"></param>
        public TwitterService(IRestClient restClient, string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            Uri baseUrl = new Uri("https://api.twitter.com/1.1");
            _client = restClient;
            _client.BaseUrl = baseUrl;
            _client.Authenticator = OAuth1Authenticator.ForProtectedResource(consumerKey, consumerSecret, accessToken,
                accessTokenSecret);

            RestRequest request = new RestRequest("account/verify_credentials.json");
            request.AddParameter("include_entities", "true", ParameterType.QueryString);

            IRestResponse response = _client.Execute(request);

            if (response != null && response.StatusCode == HttpStatusCode.OK) return;
            var errorMessage = response != null ? response.ErrorMessage : "";
            throw new Exception($"Could not validate credientals with Twitter API {errorMessage}");
        }

        /// <summary>
        /// Get all tweets from twitter search api for specific accounts and a date range
        /// </summary>
        /// <param name="accounts">A list of twitter accounts</param>
        /// <param name="from">Starting date</param>
        /// <param name="to">Ending date</param>
        /// <returns></returns>
        public ITwitterResponse GetTweets(IList<string> accounts, DateTime from, DateTime to)
        {
            RestRequest request = new RestRequest("/search/tweets.json", Method.GET);
            var tweets = new List<ITweet>();
            var q = accounts.Aggregate("", (current, account) => current + ("from:" + account + " OR "));

            q = q.Remove(q.LastIndexOf("OR", StringComparison.Ordinal), 3);
            q += "since:" + from.ToString("yyyy-MM-d") + " until:" + to.ToString("yyyy-MM-d");
            request.AddQueryParameter("q", q);

            var response = _client.Execute(request);
            dynamic twitterResponse = JsonConvert.DeserializeObject(response.Content);
            var statuses = PaginateTweets(q, twitterResponse);

            foreach (var status in statuses)
            {
                tweets.Add(new Tweet()
                {
                    Account = status.user.name,
                    DateTime = status.created_at,
                    Text = status.text,
                    UserMentions = status.entities.user_mentions.Count
                });
            }

            var accountStats = tweets.GroupBy(t => t.Account).Select(
                               tw => new AccountStats {
                                   AccountName = tw.Key,
                                   TotalTweets = tw.Count(),
                                   TotalMentions = tw.Sum(m => m.UserMentions)
                               }
                              ).ToList();

            return new TwitterResponse
            {
                Tweets = tweets,
                AccountStats =  accountStats
            };
        }


        private dynamic PaginateTweets(string query, dynamic twitterResponse)
        {
            RestRequest request = new RestRequest("/search/tweets.json", Method.GET);
            var statuses = twitterResponse.statuses;
            bool flag = true;
            const int timeout = 15 * 60 / 180; // To avoid twitter rate limit
            var mergeSettings = new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            };
           
            if (twitterResponse.search_metadata.next_results != null)
            {
                var len = twitterResponse.statuses.Count -1;
                string maxId = (twitterResponse.statuses[len].id.Value - 1).ToString();
                request = new RestRequest("/search/tweets.json", Method.GET);
                request.AddQueryParameter("q", query);
                request.AddQueryParameter("max_id", maxId);
            }
            else
            {
                flag = false;
            }

            do
            {
                // Throttle
                Thread.Sleep(timeout);
                var response = _client.Execute(request);
                twitterResponse = JsonConvert.DeserializeObject(response.Content);
                statuses.Merge(twitterResponse.statuses, mergeSettings);

                if (twitterResponse.search_metadata.next_results == null)
                {
                    flag = false;
                }
                else
                {
                    var len = twitterResponse.statuses.Count - 1;
                    string maxId = (twitterResponse.statuses[len].id.Value - 1).ToString();
                    request = new RestRequest("/search/tweets.json", Method.GET);
                    request.AddQueryParameter("q", query);
                    request.AddQueryParameter("max_id", maxId);
                }
            } while (flag);

            return statuses;
        }


    }

}