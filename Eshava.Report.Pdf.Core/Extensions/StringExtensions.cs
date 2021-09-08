using System;
using System.Globalization;
using System.Linq;

namespace Eshava.Report.Pdf.Core.Extensions
{
	public static class StringExtensions
	{
		public static bool IsNullOrEmpty(this string text)
		{
			return String.IsNullOrEmpty(text);
		}

		public static string ConvertHexColorToDecimalColor(this string hexColor)
		{
			if (hexColor.IsNullOrEmpty() || !hexColor.StartsWith("#"))
			{
				return hexColor;
			}

			hexColor = hexColor.Replace("#", "").ToLower();

			var alpha = "";
			var red = "";
			var green = "";
			var blue = "";

			var parts = hexColor.Select(p => p.ToString()).ToList();

			switch (hexColor.Length)
			{
				case 3:
					red = parts[0] + parts[0];
					green = parts[1] + parts[1];
					blue = parts[2] + parts[2];
					alpha = "ff";
					break;
				case 4:
					red = parts[0] + parts[0];
					green = parts[1] + parts[1];
					blue = parts[2] + parts[2];
					alpha = parts[3] + parts[3];
					break;
				case 6:
					red = parts[0] + parts[1];
					green = parts[2] + parts[3];
					blue = parts[4] + parts[5];
					alpha = "ff";
					break;
				case 8:
					red = parts[0] + parts[1];
					green = parts[2] + parts[3];
					blue = parts[4] + parts[5];
					alpha = parts[6] + parts[7];
					break;
				default:

					return "";

			}

			alpha = Int32.Parse(alpha, NumberStyles.HexNumber).ToString();
			red = Int32.Parse(red, NumberStyles.HexNumber).ToString();
			green = Int32.Parse(green, NumberStyles.HexNumber).ToString();
			blue = Int32.Parse(blue, NumberStyles.HexNumber).ToString();

			return $"{alpha} {red} {green} {blue}";
		}
	}
}