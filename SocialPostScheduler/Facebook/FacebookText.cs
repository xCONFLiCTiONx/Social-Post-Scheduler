using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SocialPostScheduler
{
    internal class FacebookText
    {
        private static readonly HttpClient client = new HttpClient();

        internal static async Task<string> PostAsync(string PAGE_ID, string NEW_PAGE_TOKEN, string MESSAGE_STRING, string LINK_URL)
        {
            try
            {
                Dictionary<string, string> values = new Dictionary<string, string>
                {
                    { "message", MESSAGE_STRING },
                    { "link", LINK_URL },
                    { "access_token", NEW_PAGE_TOKEN }
                };

                FormUrlEncodedContent content = new FormUrlEncodedContent(values);

                HttpResponseMessage response = await client.PostAsync("https://graph.facebook.com/" + PAGE_ID + "/feed?", content);

                _ = await response.Content.ReadAsStringAsync();

                return "OK";
            }
            catch (Exception ex)
            {
                EasyLogger.Error("FacebookText - @Post(2): " + ex);

                return "Error: " + ex.Message;
            }
        }
    }
}
