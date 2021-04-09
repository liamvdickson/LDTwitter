using System; 
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TwitterEntities;
using Polly;
using Polly.Retry;

namespace TwitterClient
{
    public class TwitterHttpClient
    {
        private const string route = "https://api.twitter.com/2";
        private readonly AsyncRetryPolicy policy;
        private HttpClient restClient;
        public TwitterHttpClient()
        {
            policy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(10, attempt => TimeSpan.FromSeconds(10 * Math.Pow(2, attempt)));
            
            
            restClient = new HttpClient();
            restClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
                "");
        }

        public async Task<DataWrapp<User>> GetUser(string username)
        {
            var result = await restClient.GetAsync($"{route}/users/by/username/{username}?user.fields=public_metrics");
            result.EnsureSuccessStatusCode();
            var content = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DataWrapp<User>>(content);
        }

        public async Task<DataWrapp<List<User>>> GetFriends(string userId)
        {
            var query = "?max_results=1000&user.fields=public_metrics,description";
            var result = await policy.ExecuteAsync(async () => 
                {
                    var res = await restClient.GetAsync($"{route}/users/{userId}/following{query}"); 
                    res.EnsureSuccessStatusCode(); 
                    return res;
                });
            var content = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DataWrapp<List<User>>>(content);
        }
    }


    public class DataWrapp<T>
    {
        public T Data {get; set;}
    }


}
