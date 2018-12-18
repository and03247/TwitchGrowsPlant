using System;

namespace TwitchToHue
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class HueClient
    {
        private readonly string ipAddress = ConfigurationManager.AppSettings["ipAddress"];
        private readonly string hueUsername = ConfigurationManager.AppSettings["hueUsername"];
        private readonly string lightNumber = "1";
        private static readonly HttpClient client = new HttpClient();


        public async Task ChangeLightToColor(string color)
        {
            var url = "https://" + ipAddress + "/api/" + hueUsername + "/lights/" + lightNumber + "/state";

            var values = new Dictionary<string, string>
                {
                    { "on", "true" },
                    { "sat", "254" },
                    { "bri", "254" },
                    { "hue", "10000" }
                };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync(url, content);

            var responseString = await response.Content.ReadAsStringAsync();
        }

    }
}
