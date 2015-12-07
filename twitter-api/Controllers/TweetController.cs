using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using RestSharp;
using twitter_api.Services;

namespace twitter_api.Controllers
{
    public class TweetController : ApiController
    {
        private static readonly string ConsumerKey = ConfigurationManager.AppSettings["TwitterConsumerKey"];
        private static readonly string ConsumerSecret = ConfigurationManager.AppSettings["TwitterConsumerSecret"];
        private static readonly string TwitterAccessToken = ConfigurationManager.AppSettings["TwitterAccessToken"];
        private static readonly string TwitterAccessTokenSecret = ConfigurationManager.AppSettings["TwitterAccessTokenSecret"];
        private readonly IUnityContainer _container;
        private ITwitterService _twitterService;

        public TweetController(IUnityContainer container)
        {
            this._container = container;
        }

        public IHttpActionResult GetAllTweets()
        {
            var restClient = _container.Resolve<IRestClient>();
            _twitterService = _container.Resolve<ITwitterService>(new ResolverOverride[]
                                   {
                new ParameterOverride("restClient", restClient),
                new ParameterOverride("consumerKey", ConsumerKey),
                new ParameterOverride("consumerSecret", ConsumerSecret),
                new ParameterOverride("accessToken", TwitterAccessToken),
                new ParameterOverride("accessTokenSecret", TwitterAccessTokenSecret)});

            var accounts = new List<string>(ConfigurationManager.AppSettings["TwitterAccounts"].Split(','));
            var numberDays = Convert.ToInt32(ConfigurationManager.AppSettings["NumberDays"]);
            var tweets = _twitterService.GetTweets(accounts, DateTime.Now.AddDays(-numberDays), DateTime.Now);
            return Json(tweets);
        }
    }
}
