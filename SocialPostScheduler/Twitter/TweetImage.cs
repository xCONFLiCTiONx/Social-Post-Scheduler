using Newtonsoft.Json.Linq;
using SocialPostScheduler.Twitter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SocialPostScheduler
{
    internal class TweetImage : IDisposable
    {
        private readonly HMACSHA1 _sigHasher;
        private readonly DateTime _epochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static string _API_KEY;
        private static string _ACCESS_TOKEN;

        /// <summary>
        /// Twitter endpoint for sending tweets
        /// </summary>
        private readonly string _TwitterTextAPI;

        /// <summary>
        /// Twitter endpoint for uploading images
        /// </summary>
        private readonly string _TwitterImageAPI;

        internal TweetImage(string API_KEY, string ACCESS_TOKEN, string API_SECRET_KEY, string ACCESS_TOKEN_SECRET)
        {
            _API_KEY = API_KEY;
            _ACCESS_TOKEN = ACCESS_TOKEN;

            _TwitterTextAPI = "https://api.twitter.com/1.1/statuses/update.json";
            _TwitterImageAPI = "https://upload.twitter.com/1.1/media/upload.json";

            _sigHasher = new HMACSHA1(new ASCIIEncoding().GetBytes($"{API_SECRET_KEY}&{ACCESS_TOKEN_SECRET}"));
        }

        /// <summary>
        /// Publish a post with image
        /// </summary>
        /// <returns>result</returns>
        /// <param name="message">message to publish</param>
        /// <param name="image">image to attach</param>
        internal string PublishTweet(string message, string image = null)
        {
            try
            {
                string mediaID = string.Empty;
                Task<Tuple<int, string>> rezImage = Task.Run(async () =>
                {
                    Tuple<int, string> response = await Tweet(image);
                    return response;
                });
                JObject rezImageJson = JObject.Parse(rezImage.Result.Item2);

                if (rezImage.Result.Item1 != 200)
                {
                    try
                    {
                        return $"Error uploading image to Twitter. {rezImageJson["errors"][0]["message"].Value<string>()}";
                    }
                    catch
                    {
                        return "Unknown error uploading image to Twitter";
                    }
                }
                mediaID = rezImageJson["media_id_string"].Value<string>();

                Task<Tuple<int, string>> rezText = Task.Run(async () =>
                {
                    Tuple<int, string> response = await TweetText(TweetLimit.CutTweetToLimit(message), mediaID);
                    return response;
                });
                JObject rezTextJson = JObject.Parse(rezText.Result.Item2);

                if (rezText.Result.Item1 != 200)
                {
                    try
                    {
                        return $"Error sending image to Twitter. {rezTextJson["errors"][0]["message"].Value<string>()} - Response Code: " + rezText.Result.Item1;
                    }
                    catch
                    {
                        return "Unknown error sending image to Twitter - Response Code: " + rezText.Result.Item1;
                    }
                }

                try
                {
                    if (File.Exists(image))
                    {
                        File.Delete(image);
                    }
                }
                catch (Exception ex)
                {
                    EasyLogger.Error("TweetImage - @PublishTweet(1): " + ex);
                }

                return "OK";
            }
            catch (Exception ex)
            {
                EasyLogger.Error("TweetImage - @PublishTweet(2): " + ex);
                return ex.Message;
            }
        }

        /// <summary>
        /// Send a tweet with some image attached
        /// </summary>
        /// <returns>HTTP StatusCode and response</returns>
        /// <param name="text">Text</param>
        /// <param name="mediaID">Media ID for the uploaded image. Pass empty string, if you want to send just text</param>
        internal Task<Tuple<int, string>> TweetText(string text, string mediaID = null)
        {
            Dictionary<string, string> data;
            if (mediaID.Length > 0)
            {
                Dictionary<string, string> imageData = new Dictionary<string, string> {
                    { "status", text },
                    { "trim_user", "1" },
                    { "media_ids", mediaID}
                };

                data = imageData;
            }
            else
            {
                Dictionary<string, string> messageData = new Dictionary<string, string> {
                    { "status", text },
                    { "trim_user", "1" }
                };

                data = messageData;
            }

            return SendText(_TwitterTextAPI, data);
        }

        /// <summary>
        /// Upload some image to Twitter
        /// </summary>
        /// <returns>HTTP StatusCode and response</returns>
        /// <param name="pathToImage">Path to the image to send</param>
        internal Task<Tuple<int, string>> Tweet(string pathToImage)
        {
            byte[] imgdata = System.IO.File.ReadAllBytes(pathToImage);
            ByteArrayContent imageContent = new ByteArrayContent(imgdata);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

            MultipartFormDataContent multipartContent = new MultipartFormDataContent
            {
                { imageContent, "media" }
            };

            return SendImage(_TwitterImageAPI, multipartContent);
        }

        private async Task<Tuple<int, string>> SendText(string URL, Dictionary<string, string> textData)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", PrepareOAuth(URL, textData, "POST"));

                HttpResponseMessage httpResponse = await httpClient.PostAsync(URL, new FormUrlEncodedContent(textData));
                string httpContent = await httpResponse.Content.ReadAsStringAsync();

                return new Tuple<int, string>(
                    (int)httpResponse.StatusCode,
                    httpContent
                    );
            }
        }

        private async Task<Tuple<int, string>> SendImage(string URL, MultipartFormDataContent multipartContent)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", PrepareOAuth(URL, null, "POST"));

                HttpResponseMessage httpResponse = await httpClient.PostAsync(URL, multipartContent);
                string httpContent = await httpResponse.Content.ReadAsStringAsync();

                return new Tuple<int, string>(
                    (int)httpResponse.StatusCode,
                    httpContent
                    );
            }
        }

        #region Some OAuth magic
        private string PrepareOAuth(string URL, Dictionary<string, string> data, string httpMethod)
        {
            int timestamp = (int)((DateTime.UtcNow - _epochUtc).TotalSeconds);

            Dictionary<string, string> oAuthData = new Dictionary<string, string>
            {
                { "oauth_consumer_key", _API_KEY },
                { "oauth_signature_method", "HMAC-SHA1" },
                { "oauth_timestamp", timestamp.ToString() },
                { "oauth_nonce", Guid.NewGuid().ToString() },
                { "oauth_token", _ACCESS_TOKEN },
                { "oauth_version", "1.0" }
            };

            if (data != null)
            {
                foreach (KeyValuePair<string, string> item in data)
                {
                    oAuthData.Add(item.Key, item.Value);
                }
            }

            oAuthData.Add("oauth_signature", GenerateSignature(URL, oAuthData, httpMethod));

            return GenerateOAuthHeader(oAuthData);
        }

        /// <summary>
        /// Generate an OAuth signature from OAuth header values
        /// </summary>
        private string GenerateSignature(string url, Dictionary<string, string> data, string httpMethod)
        {
            string sigString = string.Join(
                "&",
                data
                    .Union(data)
                    .Select(kvp => string.Format("{0}={1}", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))
                    .OrderBy(s => s)
            );

            string fullSigData = string.Format("{0}&{1}&{2}",
                httpMethod,
                Uri.EscapeDataString(url),
                Uri.EscapeDataString(sigString.ToString()
                )
            );

            return Convert.ToBase64String(
                _sigHasher.ComputeHash(
                    new ASCIIEncoding().GetBytes(fullSigData.ToString())
                )
            );
        }

        /// <summary>
        /// Generate the raw OAuth HTML header from the values (including signature)
        /// </summary>
        private static string GenerateOAuthHeader(Dictionary<string, string> data)
        {
            return string.Format(
                "OAuth {0}",
                string.Join(
                    ", ",
                    data
                        .Where(kvp => kvp.Key.StartsWith("oauth_"))
                        .Select(
                            kvp => string.Format("{0}=\"{1}\"",
                            Uri.EscapeDataString(kvp.Key),
                            Uri.EscapeDataString(kvp.Value)
                            )
                        ).OrderBy(s => s)
                    )
                );
        }

        public void Dispose()
        {
            if (_sigHasher != null)
            {
                _sigHasher.Dispose();
            }
        }
        #endregion
    }
}
