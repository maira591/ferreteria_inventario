using System.Drawing;

namespace Ferreteria.Domain.Utils
{
    public static class GraphicColor
    {

        public const string Red = "rgb(255, 99, 132)";
        public const string Orange = "rgb(255, 159, 64)";
        public const string Yellow = "rgb(255, 205, 86)";
        public const string Green = "rgb(75, 192, 192)";
        public const string Blue = "rgb(54, 162, 235)";
        public const string Purple = "rgb(153, 102, 255)";
        public const string Grey = "rgb(201, 203, 207)";
        public const string Crimson = "rgb(220,20,60)";
        public const string DarkGreen = "rgb(0,100,0)";
        public const string Gold = "rgb(255,215,0)";
        public const string SlateBlue = "rgb(106,90,205)";
        public const string Indigo = "rgb(75,0,130)";
        public const string LightCoral = "rgb(240,128,128)";
        public const string Teal = "rgb(0,128,128)";
        public const string Sienna = "rgb(160,82,45)";
        public const string SteelBlue = "rgb(70,130,180)";
        public const string Tomato = "rgb(253,99,71)";
        public const string MediumBlue = "rgb(0,0,205)";
        public const string Magenta = "rgb(255,0,255)";
        public const string LightSalmon = "rgb(255,160,122)";


        public static string[] Colors = { "rgb(255, 99, 132)", "rgb(255, 159, 64)", "rgb(255, 205, 86)", "rgb(75, 192, 192)", "rgb(54, 162, 235)", "rgb(153, 102, 255)", "rgb(201, 203, 207)", "rgb(220,20,60)", "rgb(0,100,0)", "rgb(255,215,0)", "rgb(106,90,205)", "rgb(75,0,130)", "rgb(240,128,128)", "rgb(0,128,12)", "rgb(160,82,45)", "rgb(70,130,180)", "rgb(253,99,71)", "rgb(0,0,205)", "rgb(255,0,255)", "rgb(255,160,122)" };


        public static string SetTransparency(string color)
        {
            if (color.StartsWith("#"))
            {
                Color colorConverted = ColorTranslator.FromHtml(color);

                color = $"rgb({colorConverted.R}, {colorConverted.G}, {colorConverted.B})";
            }

            color = color.ToLower().Replace("rgb(", "").Trim();
            color = color.ToLower().Replace(")", "").Trim();

            string[] colorArray = color.Split(",");

            return string.Concat("rgba(", int.Parse(colorArray[0].Trim()), ",", int.Parse(colorArray[1].Trim()), ",", int.Parse(colorArray[2].Trim()), ", 0.5)");
        }
    }
}


