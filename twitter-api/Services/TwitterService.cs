using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using twitter_api.Models;

namespace twitter_api.Services
{
   
    public class TwitterService : ITwitterService
    {
        private readonly RestClient _client;

        public TwitterService(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            Uri baseUrl = new Uri("https://api.twitter.com/1.1");
            _client = new RestClient(baseUrl)
            {
                Authenticator = OAuth1Authenticator.ForProtectedResource(consumerKey, consumerSecret, accessToken, accessTokenSecret)
            };
            RestRequest request = new RestRequest("account/verify_credentials.json");

            request.AddParameter("include_entities", "true", ParameterType.QueryString);

            IRestResponse response = _client.Execute(request);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        }
        public ITwitterResponse GetTweets(IList<string> accounts, DateTime from, DateTime to)
        {
            RestRequest request = new RestRequest("/search/tweets.json", Method.GET);
            const int timeout = 15 * 60 / 180; // To avoid twitter rate limit

            string q = "";

            foreach (string account in accounts)
            {
                q+= "from:" + account + " OR ";
            }

            q = q.Remove(q.LastIndexOf("OR"), 3);
            q += "since:" + from.ToString("yyyy-MM-d") + " until:" + to.ToString("yyyy-MM-d");

            request.AddQueryParameter("q", q);

            var response = _client.Execute(request);
            
            dynamic data = JsonConvert.DeserializeObject(response.Content);
            bool flag = true;

            if (data.search_metadata.next_results != null)
            {
                String max_id = (data.statuses[14].id.Value - 1).ToString();
                request = new RestRequest("/search/tweets.json", Method.GET);
                request.AddQueryParameter("q", q);
                request.AddQueryParameter("max_id", max_id);
            }
            else
            {
                flag = false;
            }

            do
            {
                Thread.Sleep(timeout);

                response = _client.Execute(request);
                data = JsonConvert.DeserializeObject(response.Content);

                if (data.search_metadata.next_results == null)
                {
                    flag = false;
                }
                else
                {
                    String max_id = (data.statuses[14].id.Value - 1).ToString();
                    request = new RestRequest("/search/tweets.json", Method.GET);
                    request.AddQueryParameter("q", q);
                    request.AddQueryParameter("max_id", max_id);
                }

              

            } while (flag);


            var tweets = new List<ITweet>();
            foreach (var status in data.statuses)
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

            return new TwitterResponse()
            {
                Tweets = tweets,
                AccountStats =  accountStats
            };
        }
    }
}