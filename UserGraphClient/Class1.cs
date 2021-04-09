using System;
using System.Threading.Tasks;
using Neo4jClient;
using TwitterEntities;

namespace UserGraphClient
{
    public class Client
    {
        private GraphClient client;

        public async Task Initialise()
        {
            client = new GraphClient(new Uri("http://localhost:7474/"), "neo4j", "pass");
            await client.ConnectAsync();
        }

        public async Task WriteUser(User user)
        {
            var newUser = new NeoUser
            {
                Description = user.Description,
                UserName = user.Username,
                Id = user.Id,
                FollowersCount = user.PublicMetrics.FollowersCount,
                FollowingCount = user.PublicMetrics.FollowingCount
            };
            await client.Cypher
                .Merge("(user:User { Id: $id })")
                .OnCreate()
                .Set("user = $newUser")
                .WithParams(new {
                    id = newUser.Id,
                    newUser
                })
                .ExecuteWithoutResultsAsync();
        }

        public async Task SetFollows(string userId, string followingId)
        {
            await client.Cypher
                .Match("(user1:User)", "(user2:User)")
                .Where((User user1) => user1.Id == userId)
                .AndWhere((User user2) => user2.Id == followingId)
                .Merge("(user1)-[:FOLLOWS]->(user2)")
                .ExecuteWithoutResultsAsync();
        }
    }

    public class NeoUser
    {
        public string UserName {get; set;}
        public string Id {get; set;}

        public string Description {get; set;}
        public int FollowersCount {get; set;}

        public int FollowingCount {get; set;}
    }
}
