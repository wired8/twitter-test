using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twitter_api.Models
{
    public interface ITweet
    {
        string Account { get; set; }
        string Text { get; set; }
        int UserMentions { get; set; }
        string DateTime { get; set; }
    }
}
