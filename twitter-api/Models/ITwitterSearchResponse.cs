using System.Collections.Generic;

namespace twitter_api.Models
{
    public interface ITwitterSearchResponse
    {
         double completed_in { get; set; }
         long max_id { get; set; }
         string max_id_str { get; set; }
         string next_page { get; set; }
         int page { get; set; }
         string query { get; set; }
         string refresh_url { get; set; }
         List<Tweet> results { get; set; }
         int results_per_page { get; set; }
         int since_id { get; set; }
         string since_id_str { get; set; }
    }
}