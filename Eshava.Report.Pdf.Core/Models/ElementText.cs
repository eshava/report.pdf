using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Extensions;
using Eshava.Report.Pdf.Core.Interfaces;
using Eshava.Report.Pdf.Interfaces;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ElementText : ElementBase, IHyperlink
	{
		private Font _font;

		[XmlAttribute]
		public string Fontfamily { get; set; }

		[XmlAttribute]
		public double Size { get; set; }

		[XmlAttribute]
		public string Color { get; set; }

		[XmlAttribute]
		public string BackgroundColor { get; set; }

		[XmlAttribute]
		public int BackgroundSpacing { get; set; }

		[XmlAttribute]
		public bool Bold { get; set; }

		[XmlAttribute]
		public bool EnableHtmlAutoConvert { get; set; }

		[XmlAttribute]
		public bool Italic { get; set; }

		[XmlAttribute]
		public bool Underline { get; set; }

		[XmlAttribute]
		public bool Strikeout { get; set; }

		[XmlAttribute]
		public Alignment Alignment { get; set; }

		[XmlAttribute]
		public bool ExpandAndShift { get; set; }

		[XmlAttribute]
		public double ShiftUpHeight { get; set; }

		[XmlIgnore]
		public double HeightDifference { get; private set; }

		[XmlAttribute]
		public bool NoShift { get; set; }

		[XmlAttribute]
		public string Hyperlink { get; set; }

		public Size GetTextSize(IGraphics graphics, string text)
		{
			return graphics.GetTextSize(GetFont(), Width, text, Alignment);
		}

		/// <summary>
		/// Calculates the text size considering the field size
		/// </summary>
		/// <param name="graphics">Graphics element</param>
		/// <returns>Field size</returns>
		public override Size GetSize(IGraphics graphics)
		{
			var textSize = GetSizeWithAdjustments(graphics);

			return textSize.Adjusted;
		}

		public override void Draw(IGraphics graphics, Point topLeftPage, Size sizePage)
		{
			var textSize = GetSizeWithAdjustments(graphics);

			if (!BackgroundColor.IsNullOrEmpty())
			{
				var spaceWidth = (BackgroundSpacing * textSize.SpaceWidth) / 2;
				Point topLeft;
				Point bottomRight;
				switch (Alignment)
				{
					case Alignment.Center:
						var remainingSpace = (Width - textSize.Real.Width) / 2;
						topLeft = new Point(topLeftPage.X + PosX + remainingSpace - spaceWidth, topLeftPage.Y + PosY);
						bottomRight = new Point(textSize.Real.Width + (2 * spaceWidth), textSize.Real.Height);
						break;
					case Alignment.Right:
						topLeft = new Point(topLeftPage.X + PosX + (Width - textSize.Real.Width) - spaceWidth, topLeftPage.Y + PosY);
						bottomRight = new Point(textSize.Real.Width + (2 * spaceWidth), textSize.Real.Height);

						break;
					default:
						topLeft = new Point(topLeftPage.X + PosX - spaceWidth, topLeftPage.Y + PosY);
						bottomRight = new Point(textSize.Real.Width + (2 * spaceWidth), textSize.Real.Height);

						break;
				}

				graphics.DrawRectangle(BackgroundColor, 1, DashStyle.Solid, topLeft, bottomRight, true);
			}

			graphics.DrawText(GetFont(), Content, Alignment, topLeftPage, sizePage, new Point(PosX, PosY), textSize.Adjusted);
		}

		public List<string> SplitBySpacesAndLineBreaks()
		{
			var textLines = new List<string>();
			if (Content.IsNullOrEmpty())
			{
				return textLines;
			}

			foreach (var textPart in Content.Split(' '))
			{
				if (textPart.Contains("\n"))
				{
					var currentText = "";
					for (var index = 0; index < textPart.Length; index++)
					{
						if (textPart[index] == '\r' && index < (textPart.Length - 2))
						{
							textLines.Add(currentText);
							textLines.Add("\r\n");
							currentText = "";
							index++;
						}
						else if (textPart[index] == '\n' && index < (textPart.Length - 1))
						{
							textLines.Add(currentText);
							textLines.Add("\n");
							currentText = "";
						}
						else
						{
							currentText += textPart[index];
						}
					}

					if (!currentText.IsNullOrEmpty())
					{
						textLines.Add(currentText);
					}
				}
				else
				{
					textLines.Add(textPart);
				}
			}

			return textLines;
		}

		public (Point Start, Size Size) GetHyperlinkPosition(IGraphics graphics, Point topLeftPage)
		{
			var topLeftTotal = new Point(topLeftPage.X + PosX, topLeftPage.Y + PosY);

			return (topLeftTotal, GetSize(graphics));
		}

		public static T Clone<T>(T element) where T : ElementText, new()
		{
			var clone = new T();
			foreach (var propertyInfo in element.GetType().GetProperties())
			{
				if (propertyInfo.CanWrite)
				{
					propertyInfo.SetValue(clone, propertyInfo.GetValue(element, null));
				}
			}
			return clone;
		}

		private Font GetFont()
		{
			if (_font == null)
			{
				_font = new Font
				{
					Fontfamily = Fontfamily,
					Size = Size,
					Bold = Bold,
					Italic = Italic,
					Underline = Underline,
					Strikeout = Strikeout,
					Color = Color
				};
			}

			return _font;
		}
		private (Size Real, Size Adjusted, double SpaceWidth) GetSizeWithAdjustments(IGraphics graphics)
		{
			if (Content.IsNullOrEmpty())
			{
				return (new Size(0, 0), new Size(0, 0), 0);
			}

			var textSize = GetTextSize(graphics, Content);
			var referenceSize = graphics.GetTextSize(GetFont(), Width, "a", Alignment.Left);
			var referenceSize2 = graphics.GetTextSize(GetFont(), Width, "a a", Alignment.Left);

			double height;
			HeightDifference = 0;

			// Height == 0: Can have an infinite height
			if (Math.Abs(textSize.Height) < 0.001)
			{
				height = 0.0;
			}
			else if (ExpandAndShift && textSize.Height > Height)
			{

				height = textSize.Height;
				HeightDifference = height - referenceSize.Height;
			}
			else if (Math.Abs(Height) < 0.001 || (Height > textSize.Height && !ExpandAndShift))
			{
				height = textSize.Height;
			}
			else
			{
				height = Height;
			}

			return (new Size(textSize.Width, textSize.Height), new Size(Width, height), referenceSize2.Width - (2 * referenceSize.Width));
		}
	}
}