using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitterClient;
using TwitterEntities;
using UserGraphClient;

public class TwitterExplorer
{
    private TwitterHttpClient twitterClient;
    private Client neo4jClient;

    public TwitterExplorer()
    {
            twitterClient = new TwitterHttpClient();
            neo4jClient = new Client();
    }

    public async Task Start(string rootUser)
    {
        neo4jClient.Initialise().GetAwaiter().GetResult();
        var user = twitterClient.GetUser(rootUser).GetAwaiter().GetResult();
        var searcher = new Searcher<User, string>(this.GetFollowing, 
            this.Filter, 
            this.WriteUser,
            this.Link);
        var cancellationToken = new CancellationTokenSource();
        cancellationToken.CancelAfter(12000_000);
        await searcher.Start(user.Data, cancellationToken.Token);
    }

    public async Task<IEnumerable<User>> GetFollowing(string userId)
    {
        var following = await twitterClient.GetFriends(userId);
        return following.Data;
    }

    public async Task Link(User followingUser, User targetUser)
    {
        await neo4jClient.SetFollows(followingUser.Id, targetUser.Id);
    }

    public IEnumerable<User> Filter(IEnumerable<User> users)
    {
        var count = users.Count();
        var byFollowers = users
            .OrderByDescending(s => s.PublicMetrics.FollowersCount);
        var byFollowing = users
            .OrderByDescending(s => s.PublicMetrics.FollowingCount)
            .Select(s => s.Id)
            .ToList();
        var found = 0;
        foreach (var user in byFollowers)
        {
            if (byFollowing.IndexOf(user.Id) < count / 2)
            {
                found++;
                yield return user;
            }
            if (found > 10)
                break;
        } 
    }

    public async Task WriteUser(User user)
    {
        await neo4jClient.WriteUser(user);
    }
}