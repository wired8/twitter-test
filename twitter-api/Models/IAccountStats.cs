namespace twitter_api.Models
{
    public interface IAccountStats
    {
        string AccountName { get; set; }
        int TotalTweets { get; set; }
        int TotalMentions { get; set; }
    }
}