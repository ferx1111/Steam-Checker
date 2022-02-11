using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SteamChecker
{
    internal static class Twitch
    {
        static HttpClientHandler hcHandle = new HttpClientHandler();

        public static async Task<bool> IsOnline(string channel)
        {
            using (var hc = new HttpClient(hcHandle, false))

            {
                hc.DefaultRequestHeaders.Add("Client-ID", "gp762nuuoqcoxypju8c569th9wz7q5");
                hc.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "d12hk677h7edr9vak6oybkhk31ag38");
                hc.Timeout = TimeSpan.FromSeconds(5);

                using (var response = await hc.GetAsync($"https://api.twitch.tv/helix/streams?user_login={channel}"))
                {
                    response.EnsureSuccessStatusCode();
                    string jsonString = await response.Content.ReadAsStringAsync();

                    if (jsonString.Contains("live"))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
