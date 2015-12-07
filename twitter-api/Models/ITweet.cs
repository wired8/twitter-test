namespace twitter_api.Models
{
    public interface ITweet
    {
         string created_at { get; set; }
         string from_user { get; set; }
         int from_user_id { get; set; }
         string from_user_id_str { get; set; }
         string from_user_name { get; set; }
         object geo { get; set; }
         string id { get; set; }
         string id_str { get; set; }
         string iso_language_code { get; set; }
         MetaData metadata { get; set; }
         string profile_image_url { get; set; }
         string profile_image_url_https { get; set; }
         string source { get; set; }
         string text { get; set; }
         string to_user { get; set; }
         int to_user_id { get; set; }
         string to_user_id_str { get; set; }
         string to_user_name { get; set; }
         long? in_reply_to_status_id { get; set; }
         string in_reply_to_status_id_str { get; set; }
    }
}