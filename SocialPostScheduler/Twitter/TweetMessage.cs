using SocialPostScheduler.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SocialPostScheduler
{
    internal class TweetMessage : IDisposable
    {
        private const string TwitterApiBaseUrl = "https://api.twitter.com/1.1/";
        private readonly HMACSHA1 sigHasher;
        private readonly DateTime epochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static string _API_KEY;
        private static string _ACCESS_TOKEN;

        /// <summary>
        /// Creates an object for sending tweets to Twitter using Single-user OAuth.
        /// 
        /// Get your access keys by creating an app at apps.twitter.com then visiting the
        /// "Keys and Access Tokens" section for your app. They can be found under the
        /// "Your Access Token" heading.
        /// </summary>
        internal TweetMessage(string API_KEY, string ACCESS_TOKEN, string API_SECRET_KEY, string ACCESS_TOKEN_SECRET)
        {
            _API_KEY = API_KEY;
            _ACCESS_TOKEN = ACCESS_TOKEN;
            sigHasher = new HMACSHA1(new ASCIIEncoding().GetBytes(string.Format("{0}&{1}", API_SECRET_KEY, ACCESS_TOKEN_SECRET)));
        }

        /// <summary>
        /// Publish a post with image
        /// </summary>
        /// <returns>result</returns>
        /// <param name="message">message to publish</param>
        /// <param name="pathToImage">image to attach</param>
        internal string PublishTweet(string message)
        {
            try
            {
                Task<string> rezText = Task.Run(async () =>
                {
                    string response = await Tweet(TweetLimit.CutTweetToLimit(message));
                    return response;
                });

                if (rezText.Result != "OK")
                {
                    EasyLogger.Warning(rezText.Result);
                }

                return "OK";
            }
            catch (Exception ex)
            {
                EasyLogger.Error("TweetMessage - @PublishTweet(1): " + ex);
                return ex.Message;
            }
        }

        /// <summary>
        /// Sends a tweet with the supplied text and returns the response from the Twitter API.
        /// </summary>
        internal Task<string> Tweet(string message)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "status", message },
                { "trim_user", "1" }
            };

            return SendRequest("statuses/update.json", data);
        }

        private Task<string> SendRequest(string url, Dictionary<string, string> data)
        {
            string fullUrl = TwitterApiBaseUrl + url;

            // Timestamps are in seconds since 1/1/1970.
            int timestamp = (int)(DateTime.UtcNow - epochUtc).TotalSeconds;

            // Add all the OAuth headers we'll need to use when constructing the hash.
            data.Add("oauth_consumer_key", _API_KEY);
            data.Add("oauth_signature_method", "HMAC-SHA1");
            data.Add("oauth_timestamp", timestamp.ToString());
            data.Add("oauth_nonce", "a"); // Required, but Twitter doesn't appear to use it, so "a" will do.
            data.Add("oauth_token", _ACCESS_TOKEN);
            data.Add("oauth_version", "1.0");

            // Generate the OAuth signature and add it to our payload.
            data.Add("oauth_signature", GenerateSignature(fullUrl, data));

            // Build the OAuth HTTP Header from the data.
            string oAuthHeader = GenerateOAuthHeader(data);

            // Build the form data (exclude OAuth stuff that's already in the header).
            FormUrlEncodedContent formData = new FormUrlEncodedContent(data.Where(kvp => !kvp.Key.StartsWith("oauth_")));

            return SendRequest(fullUrl, oAuthHeader, formData);
        }

        /// <summary>
        /// Generate an OAuth signature from OAuth header values.
        /// </summary>
        private string GenerateSignature(string url, Dictionary<string, string> data)
        {
            string sigString = string.Join(
                "&",
                data
                    .Union(data)
                    .Select(kvp => string.Format("{0}={1}", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))
                    .OrderBy(s => s)
            );

            string fullSigData = string.Format(
                "{0}&{1}&{2}",
                "POST",
                Uri.EscapeDataString(url),
                Uri.EscapeDataString(sigString.ToString())
            );

            return Convert.ToBase64String(sigHasher.ComputeHash(new ASCIIEncoding().GetBytes(fullSigData.ToString())));
        }

        /// <summary>
        /// Generate the raw OAuth HTML header from the values (including signature).
        /// </summary>
        private static string GenerateOAuthHeader(Dictionary<string, string> data)
        {
            return "OAuth " + string.Join(
                ", ",
                data
                    .Where(kvp => kvp.Key.StartsWith("oauth_"))
                    .Select(kvp => string.Format("{0}=\"{1}\"", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))
                    .OrderBy(s => s)
            );
        }

        /// <summary>
        /// Send HTTP Request and return the response.
        /// </summary>
        private static async Task<string> SendRequest(string fullUrl, string oAuthHeader, FormUrlEncodedContent formData)
        {
            using (HttpClient http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("Authorization", oAuthHeader);

                HttpResponseMessage httpResp = await http.PostAsync(fullUrl, formData);
                string respBody = await httpResp.Content.ReadAsStringAsync();

                return "OK";
            }
        }

        public void Dispose()
        {
            if (sigHasher != null)
            {
                sigHasher.Dispose();
            }
        }
    }
}
