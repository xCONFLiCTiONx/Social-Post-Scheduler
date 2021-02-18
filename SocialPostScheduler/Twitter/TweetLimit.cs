using System;

namespace SocialPostScheduler.Twitter
{
    internal class TweetLimit
    {
        /// <summary>
        /// Cuts the tweet text to fit the limit
        /// </summary>
        /// <returns>Cutted tweet text</returns>
        /// <param name="tweet">Uncutted tweet text</param>
        internal static string CutTweetToLimit(string tweet)
        {
            while (tweet.Length >= 280)
            {
                tweet = tweet.Substring(0, tweet.LastIndexOf(" ", StringComparison.Ordinal));
            }
            return tweet;
        }
    }
}
