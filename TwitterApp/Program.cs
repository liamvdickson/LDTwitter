using System;
using TwitterClient;
using UserGraphClient;

namespace TwitterApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var twitterExplorer = new TwitterExplorer();
            twitterExplorer.Start("cheongmoves").GetAwaiter().GetResult();
        }
    }
}
