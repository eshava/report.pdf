using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
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
		public bool Bold { get; set; }

		[XmlAttribute]
		public bool Italic { get; set; }

		[XmlAttribute]
		public bool Underline { get; set; }

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
			return graphics.GetTextSize(GetFont(), Width, text);
		}

		/// <summary>
		/// Calculates the text size considering the field size
		/// </summary>
		/// <param name="graphics">Graphics element</param>
		/// <returns>Field size</returns>
		public override Size GetSize(IGraphics graphics)
		{
			if (Content.IsNullOrEmpty())
			{
				return new Size(0, 0);
			}

			var textSize = GetTextSize(graphics, Content);
			double height;
			HeightDifference = 0;

			// Height == 0: Can have an infinite height
			if (Math.Abs(textSize.Height) < 0.001)
			{
				height = 0.0;
			}
			else if (ExpandAndShift && textSize.Height > Height)
			{
				var referenceSize = graphics.GetTextSize(GetFont(), Width, "a");

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

			return new Size(Width, height);
		}

		public override void Draw(IGraphics graphics, Point topLeftPage, Size sizePage)
		{
			graphics.DrawText(GetFont(), Content, Alignment, topLeftPage, sizePage, new Point(PosX, PosY), GetSize(graphics));
		}

		public List<string> SplittByNewLine()
		{
			var textLines = new List<string>();
			if (Content.IsNullOrEmpty())
			{
				return textLines;
			}

			var parts = Content.Split('\n');
			for (var i = 0; i < parts.Length; i++)
			{
				// As long as the text part is not the last block, the NewLine character must be added again
				if (i + 1 < parts.Length)
				{
					textLines.Add(parts[i] + "\n");
				}
				else
				{
					textLines.Add(parts[i]);
				}
			}

			return textLines;
		}

		public List<string> SplittOnEndOfSentences()
		{
			var textLines = new List<string>();
			var text = new StringBuilder();

			if (Content.IsNullOrEmpty())
			{
				return textLines;
			}

			var parts = Content.Split(' ');
			for (var i = 0; i < parts.Length; i++)
			{
				if (parts[i].EndsWith(".") && ((i + 1) == parts.Length || IsCapitalLetter(parts[i + 1][0])))
				{
					// Last text section reached
					if (i > 0)
					{
						text.Append(" ");
					}

					text.Append(parts[i]);
					textLines.Add(text.ToString());
					text.Clear();
				}
				else
				{
					// It continues after the space character with a lower case character or there's no point at the end of the word
					if (i > 0)
					{
						text.Append(" ");
					}

					text.Append(parts[i]);
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
					Color = Color
				};
			}

			return _font;
		}

		private bool IsCapitalLetter(char letter)
		{
			var letterString = letter.ToString(CultureInfo.InvariantCulture);

			return Equals(letterString, letterString.ToUpper());
		}
	}
}