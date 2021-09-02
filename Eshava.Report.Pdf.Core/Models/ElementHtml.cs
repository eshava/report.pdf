using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Extensions;
using Eshava.Report.Pdf.Core.Interfaces;
using Eshava.Report.Pdf.Interfaces;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ElementHtml : ElementBase
	{
		private Font _font;
		private IEnumerable<TextSegment> _textSegments;

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

		[XmlIgnore]
		public IEnumerable<TextSegment> TextSegments
		{
			get
			{
				if (_textSegments == default)
				{
					var interpreter = new HtmlInterpreter();
					_textSegments = interpreter.AnalyzeText(GetFont(), Content);
				}

				return _textSegments;
			}
			set
			{
				_textSegments = value;
			}
		}

		public Size GetTextSize(IGraphics graphics, IEnumerable<TextSegment> textSegments)
		{
			return graphics.GetTextSize(textSegments, Width);
		}

		/// <summary>
		/// Calculates the text size considering the field size
		/// </summary>
		/// <param name="graphics">Graphics element</param>
		/// <returns>Field size</returns>
		public override Size GetSize(IGraphics graphics)
		{
			if (!TextSegments.Any() || TextSegments.All(ts => ts.Text.IsNullOrEmpty()))
			{
				return new Size(0, 0);
			}

			var textSize = GetTextSize(graphics, TextSegments);
			double height;
			HeightDifference = 0;

			// Height == 0: Can have an infinite height
			if (Math.Abs(textSize.Height) < 0.001)
			{
				height = 0.0;
			}
			else if (ExpandAndShift && textSize.Height > Height)
			{
				var referenceSize = graphics.GetTextSize(TextSegments.OrderByDescending(ts => ts.Font.Size).First().Font, Width, "a");

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
			graphics.DrawText(TextSegments, Alignment, topLeftPage, sizePage, new Point(PosX, PosY), GetSize(graphics));
		}

		public List<TextSegment> SplittOnEndOfSentences()
		{
			var textSegments = new List<TextSegment>();
			var text = new StringBuilder();

			if (!TextSegments.Any() || TextSegments.All(ts => ts.Text.IsNullOrEmpty()))
			{
				return textSegments;
			}

			foreach (var textSegment in TextSegments)
			{
				if (!textSegment.Text.Contains("."))
				{
					textSegments.Add(textSegment);

					continue;
				}

				var parts = textSegment.Text.Split(' ');
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
						textSegments.Add(new TextSegment
						{
							Text = text.ToString(),
							Font = textSegment.Font
						});
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
			}

			return textSegments;
		}

		public static T Clone<T>(T element) where T : ElementHtml, new()
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
					Color = Color.ConvertHexColorToDecimalColor()
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
