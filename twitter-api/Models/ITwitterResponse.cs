using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twitter_api.Models
{
    public interface ITwitterResponse
    {
        IList<ITweet> Tweets { get; set; }

        List<AccountStats> AccountStats { get; set; } 

    }
}
