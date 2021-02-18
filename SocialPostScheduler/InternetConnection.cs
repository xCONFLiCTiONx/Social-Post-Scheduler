using System.Net;

namespace SocialPostScheduler
{
    internal class InternetConnection
    {
        public static bool CheckConnection()
        {
            try
            {
                using (WebClient client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
