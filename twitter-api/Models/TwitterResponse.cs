using System.Collections.Generic;

namespace twitter_api.Models
{
    public class TwitterResponse : ITwitterResponse
    {
        public IList<ITweet> Tweets { get; set; }
        public List<AccountStats> AccountStats { get; set; }
    }
}