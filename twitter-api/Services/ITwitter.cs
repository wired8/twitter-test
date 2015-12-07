using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using twitter_api.Models;

namespace twitter_api.Services
{
    public interface ITwitter
    {
        IList<ITweet> GetTweets(IList<string> accounts, DateTime from, DateTime to);
    }
}