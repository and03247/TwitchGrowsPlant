namespace TwitchToHue
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Net.Http;

    public class HueClient
    {
        private readonly string ipAddress = ConfigurationManager.AppSettings["ipAddress"];
        private readonly string hueUsername = ConfigurationManager.AppSettings["hueUsername"];
        private const string LightNumber = "1";
        private static readonly HttpClient Client = new HttpClient();
        private readonly ColorConverter colorConverter;

        public HueClient()
        {
            colorConverter = new ColorConverter();
        }

        public string ChangeLightToColor(string colorName)
        {
            var color = colorConverter.GetColorByName(colorName.ToLower());

            // If can't find the specified color, return an error message.
            if (color == null)
            {
                return "Unable to detect the color: " + colorName + ". Known colors are listed here: https://htmlcolorcodes.com/color-names/";
            }

            Console.WriteLine("Color: " + color.Name + " Hue: " + color.Hue + " Bri: " + color.Bri + " Sat: " + color.Sat);

            var url = "https://" + ipAddress + "/api/" + hueUsername + "/lights/" + LightNumber + "/state";

            var values = new Dictionary<string, string>
                {
                    { "on", "true" },
                    { "sat", color.Sat.ToString() },
                    { "bri", color.Bri.ToString() },
                    { "hue", color.Hue.ToString() }
                };

            //var content = new FormUrlEncodedContent(values);

            //var response = Client.PostAsync(url, content);

            //Console.WriteLine(response.Result);

            return "Light color changed to: " + color.Name;
        }

    }
}
