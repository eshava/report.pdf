using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Extensions;
using Eshava.Report.Pdf.Core.Interfaces;

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
				_textSegments = SummerizeTextSegments(value);
			}
		}

		public Size GetTextSize(IGraphics graphics, IEnumerable<TextSegment> textSegments)
		{
			return graphics.GetTextSize(SummerizeTextSegments(textSegments), Width);
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

			// calculate line indent reduction
			var lineIndentReductions = TextSegments.Where(ts => ts.ReduceLineIndent).ToList();
			foreach (var textSegment in lineIndentReductions)
			{
				var textWidth = graphics.GetTextWidth(GetFont(), textSegment.ReduceLineIndentByText);
				if (textSegment.LineIndent > textWidth)
				{
					textSegment.LineIndent -= textWidth;
				}

				textSegment.ReduceLineIndent = false;
				textSegment.ReduceLineIndentByText = null;
			}

			var textSize = graphics.GetTextSize(TextSegments, Width);
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

		public List<TextSegment> SplittBySpaces()
		{
			var textSegments = new List<TextSegment>();
			var text = new StringBuilder();

			if (!TextSegments.Any() || TextSegments.All(ts => ts.Text.IsNullOrEmpty()))
			{
				return textSegments;
			}

			foreach (var textSegment in TextSegments)
			{
				if (textSegment.Text == Environment.NewLine)
				{
					textSegments.Add(textSegment.Clone());

					continue;
				}

				var words = textSegment.Text.Trim().Split(' ');
				for (var index = 0; index < words.Length; index++)
				{
					var word = words[index];
					if (index > 0 || textSegment.Text.StartsWith(" "))
					{
						word = " " + word;
					}

					var textSegmentWord = textSegment.Clone();
					textSegmentWord.Text = word;

					textSegments.Add(textSegmentWord);
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
					Strikeout = Strikeout,
					Color = Color.ConvertHexColorToDecimalColor()
				};
			}

			return _font;
		}

		private IEnumerable<TextSegment> SummerizeTextSegments(IEnumerable<TextSegment> textSegments)
		{
			var summerizedTextSegments = new List<TextSegment>();

			var lastTextSegment = default(TextSegment);
			foreach (var textSegment in textSegments)
			{
				if (lastTextSegment != default
					&& (textSegment.Text != Environment.NewLine && lastTextSegment.Text != Environment.NewLine)
					&& lastTextSegment.Font.GetHashCode() == textSegment.Font.GetHashCode()
					&& lastTextSegment.LineIndent == textSegment.LineIndent)
				{
					lastTextSegment.Text += textSegment.Text;
				}
				else
				{
					lastTextSegment = default(TextSegment);
				}

				if (lastTextSegment == default)
				{
					lastTextSegment = textSegment.Clone();
					summerizedTextSegments.Add(lastTextSegment);
				}
			}

			return summerizedTextSegments;
		}
	}
}