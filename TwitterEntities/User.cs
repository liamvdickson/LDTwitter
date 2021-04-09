using System;
using Newtonsoft.Json;

namespace TwitterEntities
{
    public class User: IEntity<string>
    {
        public string Id {get; set;}
        public string Name {get; set;}
        public string Username {get; set;}
        public string Description {get; set;}

        [JsonProperty("public_metrics")]
        public PublicMetrics PublicMetrics {get; set;}
    }

    public class PublicMetrics
    {
        [JsonProperty("followers_count")]
        public int FollowersCount {get; set;}

        [JsonProperty("following_count")]
        public int FollowingCount {get; set;}
        [JsonProperty("tweet_count")]
        public int TweetCount {get; set;}
        [JsonProperty("listed_count")]
        public int ListedCount {get; set;}
    }
}
