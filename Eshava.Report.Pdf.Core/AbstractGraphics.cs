using System;
using Eshava.Report.Pdf.Core.Models;

namespace Eshava.Report.Pdf.Core
{
	public abstract class AbstractGraphics
	{
		protected double CalculateLineGap(Func<string, double> measureTextHeight)
		{
			var spaceHeight = measureTextHeight(" ");
			var totalMultilineHeight = measureTextHeight("#\n#");
			var lineGap = totalMultilineHeight - spaceHeight - spaceHeight;

			return lineGap;
		}


		protected Size CalculateTextSize(string text, double lineGap, double elementWidth, Func<string, double> measureTextWith)
		{
			var spaceWidth = measureTextWith(" ");
			var height = 0.0;
			var maxWidth = 0.0;
			var lines = text.Replace("\r", "").Split('\n');

			foreach (var line in lines)
			{
				var words = line.Split(' ');
				var lineWidth = 0.0;
				height += lineGap;

				foreach (var word in words)
				{
					var wordWidth = measureTextWith(word);
					if (lineWidth + spaceWidth + wordWidth > elementWidth)
					{
						lineWidth = wordWidth;
						height += lineGap;
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