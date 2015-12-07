using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using RestSharp.Authenticators;
using twitter_api.Models;

namespace twitter_api.Services
{
   
    public class Twitter : ITwitter
    {
        private readonly RestClient _client;

        public Twitter(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
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
        public IList<ITweet> GetTweets(IList<string> accounts, DateTime from, DateTime to)
        {
            RestRequest request = new RestRequest("/search/tweets.json", Method.GET);

            string q = "";

            foreach (string account in accounts)
            {
                q+= "from:" + account + " OR ";
            }

            q = q.Remove(q.LastIndexOf("OR"), 3);
            q += "since:" + from.ToString("yyyy-MM-d") + " until:" + to.ToString("yyyy-MM-d");

            request.AddQueryParameter("q", q);

            _client.ExecuteAsync<TwitterSearchResponse>(request, response =>
            {
                Console.WriteLine("Query complete, let's see what you've won:");
                Console.WriteLine("Status Code: " + response.StatusCode);
       

                ITwitterSearchResponse tweets = response.Data;

                foreach (Tweet tweet in tweets.results)
                {
                    Console.WriteLine(String.Format("Tweet from {0}: {1} - {2}", tweet.from_user, tweet.created_at, tweet.source));
                }
            });

            return null;
        }
    }
}