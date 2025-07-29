using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Eshava.Report.Pdf.Core.Extensions
{
	public static class StringExtensions
	{
		private static Regex _regExRGBFunction = new Regex(@"\brgb\b\s*\({1}\s*(?<red>\d{1,3})\s*,{1}\s*(?<green>\d{1,3})\s*,{1}\s*(?<blue>\d{1,3})\s*\){1}.*", RegexOptions.Compiled);
		private static Regex _regExRGBAFunction = new Regex(@"\brgba\b\s*\({1}\s*(?<red>\d{1,3})\s*,{1}\s*(?<green>\d{1,3})\s*,{1}\s*(?<blue>\d{1,3})\s*,{1}\s*(?<alpha>\d{1,3})\s*\){1}.*", RegexOptions.Compiled);

		public static bool IsNullOrEmpty(this string text)
		{
			return String.IsNullOrEmpty(text);
		}

		public static string ConvertRGBFunctionToDecimalColor(this string rgbFunction)
		{
			if (rgbFunction.IsNullOrEmpty())
			{
				return rgbFunction;
			}

			rgbFunction = rgbFunction.ToLowerInvariant();

			Match match = null;
			if (rgbFunction.StartsWith("rgba"))
			{
				match = _regExRGBAFunction.Match(rgbFunction);
			}
			else if (rgbFunction.StartsWith("rgb"))
			{
				match = _regExRGBFunction.Match(rgbFunction);
			}

			if (match is null)
			{
				return rgbFunction;
			}

			var alpha = match.Groups["alpha"].Value;
			var red = match.Groups["red"].Value;
			var green = match.Groups["green"].Value;
			var blue = match.Groups["blue"].Value;

			if (alpha.IsNullOrEmpty())
			{
				alpha = "255";
			}

			return $"{alpha} {red} {green} {blue}";
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