using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace SocialPostScheduler
{
    public class PostFromFeed
    {
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// {instagram_id}/media?image_url={image_url}&access_token={access_token}
        /// {instagram_id}/media_publish?creation_id={creation-id}&access_token={access_token}
        /// </summary>
        internal static void PostToInstagram()
        {
            try
            {
                string USER_TOKEN = GetToken.RenewAccessToken(Properties.Settings.Default.InstagramToken, Properties.Settings.Default.InstagramAppSecret, Properties.Settings.Default.InstagramAppID);
                if (USER_TOKEN.Contains("ERROR:"))
                {
                    EasyLogger.Error(USER_TOKEN);
                }
                else
                {
                    if (USER_TOKEN != "The remote server returned an error: (400) Bad Request.")
                    {
                        EasyLogger.Info("Your Instagram user token has been renewed successfully!");

                        Properties.Settings.Default.InstagramToken = USER_TOKEN;
                        Properties.Settings.Default.Save();
                        Properties.Settings.Default.Reload();
                    }
                    else
                    {
                        EasyLogger.Warning("Error: The remote server returned an error: (400) Bad Request.");
                        return;
                    }

                    string feedUrl = Properties.Settings.Default.WebsiteFeed;

                    using (XmlReader feedReader = XmlReader.Create(feedUrl))
                    {
                        SyndicationFeed feedContent = SyndicationFeed.Load(feedReader);
                        List<SyndicationItem> feedItems = new List<SyndicationItem>();

                        foreach (SyndicationItem item in feedContent.Items)
                        {
                            feedItems.Add(item);
                        }
                        feedItems.Reverse();
                        feedContent.Items = feedItems;

                        if (null == feedContent)
                        {
                            return;
                        }

                        foreach (SyndicationItem item in feedContent.Items)
                        {
                            string quota_total = string.Empty;
                            string quota_usage = string.Empty;
                            try
                            {
                                using (WebClient wb = new WebClient())
                                {
                                    string responseInString = wb.DownloadString("https://graph.facebook.com/" + Properties.Settings.Default.InstagramPageID + "/content_publishing_limit?fields=config&access_token=" + Properties.Settings.Default.InstagramToken);

                                    JObject jdata = JObject.Parse(responseInString);
                                    JToken array = jdata["data"];
                                    foreach (JToken info in array)
                                    {
                                        quota_total = info["config"]["quota_total"].ToString();
                                    }
                                }

                                using (WebClient wb = new WebClient())
                                {
                                    string responseInString = wb.DownloadString("https://graph.facebook.com/" + Properties.Settings.Default.InstagramPageID + "/content_publishing_limit?fields=quota_usage&access_token=" + Properties.Settings.Default.InstagramToken);

                                    JObject jdata = JObject.Parse(responseInString);
                                    JToken array = jdata["data"];

                                    foreach (JToken info in array)
                                    {
                                        quota_usage = info["quota_usage"].ToString();
                                    }
                                }
                            }
                            catch { }

                            int usage = Convert.ToInt32(quota_usage);
                            int total = Convert.ToInt32(quota_total);

                            if (usage >= total)
                            {
                                EasyLogger.Info("Instagram post quota reached!");
                                break;
                            }

                            try
                            {
                                System.Collections.ObjectModel.Collection<SyndicationLink> links = item.Links;

                                string title = item.Title.Text + " - " + links[0].Uri.ToString();
                                string image = ExtensionMethods.GetEnclosureUri(item);
                                DateTimeOffset date = item.PublishDate;

                                if (StringToDTOffset(Properties.Settings.Default.PublishDate) < date)
                                {
                                    Task<string> success = PostAsync(image, title);
                                    int posts_used = Convert.ToInt32(quota_usage);
                                    posts_used++;
                                    EasyLogger.Info(success.Result + Environment.NewLine + "Instagram Post usage for the day: " + posts_used + " of " + quota_total);

                                    Properties.Settings.Default.PublishDate = date.ToString();
                                    Properties.Settings.Default.Save();
                                    Properties.Settings.Default.Reload();
                                }
                            }
                            catch (Exception ex)
                            {
                                EasyLogger.Error(ex);
                                continue;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error(ex);
            }
        }

        internal static async Task<string> PostAsync(string IMAGE_URL, string MESSAGE_STRING)
        {
            try
            {
                Dictionary<string, string> values = new Dictionary<string, string>
                {
                    { "image_url", IMAGE_URL },
                    { "caption", MESSAGE_STRING },
                    { "access_token", Properties.Settings.Default.InstagramToken }
                };

                FormUrlEncodedContent content = new FormUrlEncodedContent(values);

                HttpResponseMessage response = null;
                string responseString = string.Empty;
                try
                {
                    response = await client.PostAsync("https://graph.facebook.com/" + Properties.Settings.Default.InstagramPageID + "/media?", content);
                    responseString = await response.Content.ReadAsStringAsync();

                    JObject json = JObject.Parse(responseString);
                    string creation_id = json["id"].ToString();

                    values = new Dictionary<string, string>
                    {
                        { "creation_id", creation_id },
                        { "access_token", Properties.Settings.Default.InstagramToken }
                    };

                    content = new FormUrlEncodedContent(values);
                    response = await client.PostAsync("https://graph.facebook.com/" + Properties.Settings.Default.InstagramPageID + "/media_publish?", content);
                    responseString = await response.Content.ReadAsStringAsync();
                }
                catch
                {
                    return "Instagram error for post " + MESSAGE_STRING + Environment.NewLine + responseString.ToString();
                }

                return "Instagram results for post " + MESSAGE_STRING + Environment.NewLine + responseString.ToString();
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Instagram - @Post(2): " + ex);

                return "Error: " + ex.Message;
            }
        }

        public static DateTimeOffset StringToDTOffset(string offsetString)
        {
            if (!DateTimeOffset.TryParse(offsetString, out DateTimeOffset offset))
            {
                offset = DateTimeOffset.Now;
            }

            return offset;
        }
    }

    public static class ExtensionMethods
    {
        public static string GetEnclosureUri(this SyndicationItem item)
        {
            for (int i = 0; i < item.Links.Count; i++)
            {
                if (item.Links[i].RelationshipType == "enclosure")
                {
                    return item.Links[i].Uri.AbsoluteUri;
                }
            }
            return "";
        }
    }
}