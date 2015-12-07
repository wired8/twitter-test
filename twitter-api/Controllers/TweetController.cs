using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using twitter_api.Services;

namespace twitter_api.Controllers
{
    public class TweetController : ApiController
    {
        private static readonly string ConsumerKey = ConfigurationManager.AppSettings["TwitterConsumerKey"];
        private static readonly string ConsumerSecret = ConfigurationManager.AppSettings["TwitterConsumerSecret"];
        private static readonly string TwitterAccessToken = ConfigurationManager.AppSettings["TwitterAccessToken"];
        private static readonly string TwitterAccessTokenSecret = ConfigurationManager.AppSettings["TwitterAccessTokenSecret"];
        private readonly ITwitter _twitter = new Twitter(ConsumerKey, ConsumerSecret, TwitterAccessToken, TwitterAccessTokenSecret);

        public IHttpActionResult GetAllTweets()
        {
            var accounts = new List<string>(ConfigurationManager.AppSettings["TwitterAccounts"].Split(','));
            var tweets = _twitter.GetTweets(accounts, DateTime.Now.AddDays(-14), DateTime.Now);
            return Ok(tweets);
        }
    }
}
