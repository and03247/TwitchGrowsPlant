namespace TwitchToHue
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json;

    public class ColorConverter
    {
        public List<Color> AvailableColors;

        public ColorConverter()
        {
            const string FilePath = @"../../../colors.json";
            var json = File.ReadAllText(FilePath);
            var list = JsonConvert.DeserializeObject<List<Color>>(json);
            AvailableColors = list;
        }

        public Color GetColorByName(string colorName)
        {
            return AvailableColors.FirstOrDefault(c => c.Name == colorName);
        }
    }

    public class Color
    {
        private int hue;
        private int bri;
        private int sat;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("hue")]
        public int Hue
        {
            get => hue;
            set => hue = (int)((value/360.0)*65535.0); // This converts the hue scale from 0-360 to 0-65535
        }

        [JsonProperty("sat")]
        public int Sat
        {
            get => sat;
            set => sat = (int)(value * 2.54); // This converts the hue scale from 0-100 to 0-254
        }

        [JsonProperty("bri")]
        public int Bri
        {
            get => bri;
            set => bri = (int)(value * 2.54); // This converts the hue scale from 0-100 to 0-254
        }
    }
}
