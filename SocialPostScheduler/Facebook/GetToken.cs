using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocialPostScheduler
{
    internal class GetToken
    {
        internal static string RenewAccessToken(string USER_TOKEN, string APP_SECRET, string APP_ID)
        {
            try
            {
                using (WebClient wb = new WebClient())
                {
                    string responseInString = wb.DownloadString("https://graph.facebook.com/oauth/access_token?client_id=" + APP_ID + "&client_secret=" + APP_SECRET + "&grant_type=fb_exchange_token&fb_exchange_token=" + USER_TOKEN);

                    JObject json = JObject.Parse(responseInString);

                    return json["access_token"].ToString();
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("GetToken - @RenewAccessToken(1): " + ex);

                return "ERROR: " + ex.Message;
            }
        }

        internal static async Task<string> PostMessage(string PAGE_NAME, string USER_ID, string PAGE_ID, string USER_TOKEN, bool isImage, string MESSAGE_STRING, string IMAGE_PATH, string LINK_URL)
        {
            try
            {
                using (WebClient wb = new WebClient())
                {
                    string responseInString = wb.DownloadString("https://graph.facebook.com/" + USER_ID + "/accounts?access_token=" + USER_TOKEN);

                    JObject jdata = JObject.Parse(responseInString);
                    JToken array = jdata["data"];

                    foreach (JToken account in array)
                    {
                        if (account["name"].ToString().Equals(PAGE_NAME))
                        {
                            string PAGE_TOKEN = account["access_token"].ToString();

                            try
                            {
                                string response = null;

                                if (!isImage)
                                {
                                    response = await Task.Run(() => FacebookText.PostAsync(PAGE_ID, PAGE_TOKEN, MESSAGE_STRING, LINK_URL));

                                    if (response != "OK")
                                    {
                                        EasyLogger.Warning(response);

                                        Form.ActiveForm.TopMost = true;
                                        Form.ActiveForm.WindowState = FormWindowState.Minimized;
                                        Form.ActiveForm.Show();
                                        Form.ActiveForm.ShowInTaskbar = true;
                                        Form.ActiveForm.WindowState = FormWindowState.Normal;
                                        Form.ActiveForm.WindowState = Properties.Settings.Default.windowState;
                                        Form.ActiveForm.BringToFront();
                                        Form.ActiveForm.Focus();
                                        Form.ActiveForm.Activate();
                                        Form.ActiveForm.TopMost = false;

                                        Scheduler.tabControlOne.SelectedIndex = 4;
                                    }
                                    else
                                    {
                                        EasyLogger.Info("Requested message posted successfully!");

                                        return "OK";
                                    }
                                }
                                else if (isImage)
                                {
                                    response = await Task.Run(() => FacebookImage.PostAsync(PAGE_ID, PAGE_TOKEN, MESSAGE_STRING, IMAGE_PATH));

                                    if (response != "OK")
                                    {
                                        EasyLogger.Warning(response);

                                        Form.ActiveForm.TopMost = true;
                                        Form.ActiveForm.WindowState = FormWindowState.Minimized;
                                        Form.ActiveForm.Show();
                                        Form.ActiveForm.ShowInTaskbar = true;
                                        Form.ActiveForm.WindowState = FormWindowState.Normal;
                                        Form.ActiveForm.WindowState = Properties.Settings.Default.windowState;
                                        Form.ActiveForm.BringToFront();
                                        Form.ActiveForm.Focus();
                                        Form.ActiveForm.Activate();
                                        Form.ActiveForm.TopMost = false;

                                        Scheduler.tabControlOne.SelectedIndex = 4;
                                    }
                                    else
                                    {
                                        EasyLogger.Info("Requested message posted successfully!");

                                        return "OK";
                                    }
                                }

                                return "OK";
                            }
                            catch (Exception ex)
                            {
                                EasyLogger.Error("GetToken - @PostMessage(1): " + ex);

                                return "Error: " + ex.Message;
                            }
                        }
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                EasyLogger.Error("GetToken - @GetPageToken(1): " + ex);

                return ex.Message;
            }
        }
    }
}
