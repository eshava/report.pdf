using System;
using Eshava.Report.Pdf.Core.Models;

namespace Eshava.Report.Pdf.Core
{
	public abstract class AbstractGraphics
	{
		protected double CalculateSingleLineHeight(Func<string, double> measureTextHeight)
		{
			var spaceHeight = measureTextHeight(" ");
			var totalMultilineHeight = measureTextHeight("#\n#");
			var lineGap = totalMultilineHeight - spaceHeight - spaceHeight;

			return lineGap + spaceHeight;
		}

		protected double GetSpaceWidth(Func<string, double> measureTextWith)
		{
			return measureTextWith("x x") - measureTextWith("xx");
		}

		protected Size CalculateTextSize(string text, double singleLineHeight, double elementWidth, Func<string, double> measureTextWith)
		{
			var spaceWidth = GetSpaceWidth(measureTextWith);
			var height = 0.0;
			var maxWidth = 0.0;
			var lines = text.Replace("\r", "").Split('\n');

			foreach (var line in lines)
			{
				var words = line.Split(' ');
				var lineWidth = 0.0;
				height += singleLineHeight;

				foreach (var word in words)
				{
					var wordWidth = measureTextWith(word);
					if (lineWidth + spaceWidth + wordWidth > elementWidth)
					{
						lineWidth = wordWidth;
						height += singleLineHeight;
					}
					else
					{
						lineWidth += wordWidth + spaceWidth;
						maxWidth = Math.Max(maxWidth, lineWidth);
					}
				}
			}

			return new Size(maxWidth, height);
		}
	}
}