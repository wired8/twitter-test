using System;
using System.Collections.Generic;
using twitter_api.Models;

namespace twitter_api.Services
{
    public interface ITwitterService
    {
        ITwitterResponse GetTweets(IList<string> accounts, DateTime from, DateTime to);
    }
}