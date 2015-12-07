namespace twitter_api.Models
{
    public class Tweet : ITweet
    {
        public string Account { get; set; }
        public string Text { get; set; }
        public int UserMentions { get; set; }
        public string DateTime { get; set; }
    }
}