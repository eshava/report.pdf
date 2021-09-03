using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Eshava.Report.Pdf.Core.Extensions;
using Eshava.Report.Pdf.Core.Models;

namespace Eshava.Report.Pdf.Core
{
	public class HtmlInterpreter
	{
		private const double CENTIMETERTOPOINTFACTOR = 28.3465;

		public IEnumerable<TextSegment> AnalyzeText(Font font, string text)
		{
			font.Color = font.Color.ConvertHexColorToDecimalColor();

			var textSegments = new List<TextSegment>();

			if (text.IsNullOrEmpty())
			{
				textSegments.Add(new TextSegment
				{
					Font = font,
					Text = ""
				});

				return textSegments;
			}
			try
			{
				var html = $"<div>{text.Replace("<br>", "<br/>")}</div>";
				var xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(html);

				var segment = new TextSegmentExtended
				{
					Font = font
				};

				AnalyzeNode(xmlDocument.FirstChild, segment);

				FlatSegmentTree(segment, textSegments);

				if (textSegments.Last().Text == Environment.NewLine)
				{
					textSegments.RemoveAt(textSegments.Count - 1);
				}

				return textSegments;
			}
			catch
			{
				textSegments.Add(new TextSegment
				{
					Font = font,
					Text = text
				});

				return textSegments;
			}
		}

		private void FlatSegmentTree(TextSegmentExtended segment, IList<TextSegment> textSegments)
		{
			if (segment.Text == Environment.NewLine)
			{
				var lastSegment = textSegments.LastOrDefault();
				if (lastSegment != default && lastSegment.Text != Environment.NewLine)
				{
					textSegments.Add(new TextSegment
					{
						Font = segment.Font,
						Text = segment.Text
					});
				}

				return;
			}

			if (!segment.Text.IsNullOrEmpty())
			{
				var lastSegment = textSegments.LastOrDefault();
				if (lastSegment == default
					|| lastSegment.Text == Environment.NewLine
					|| lastSegment.Font.GetHashCode() != segment.Font.GetHashCode()
					|| lastSegment.LineIndent != segment.LineIndent
					|| lastSegment.ReduceLineIndentByText != segment.ReduceLineIndentByText
					)
				{
					textSegments.Add(new TextSegment
					{
						Font = segment.Font,
						Text = segment.Text,
						LineIndent = segment.LineIndent,
						ReduceLineIndent = segment.ReduceLineIndent,
						ReduceLineIndentByText = segment.ReduceLineIndentByText,
						SkipParagraphAlignment = segment.SkipParagraphAlignment
					});
				}
				else
				{
					lastSegment.Text += " " + segment.Text;
				}
			}

			if (segment.Children.Count == 0)
			{
				return;
			}

			foreach (var childSegment in segment.Children)
			{
				if (segment.LineIndent > 0)
				{
					childSegment.LineIndent += segment.LineIndent;
				}

				FlatSegmentTree(childSegment, textSegments);
			}
		}

		private void AnalyzeNode(XmlNode node, TextSegmentExtended parentSegment)
		{
			if (node.NodeType == XmlNodeType.Text)
			{
				parentSegment.Children.Add(new TextSegmentExtended
				{
					Font = new Font
					{
						Fontfamily = parentSegment.Font.Fontfamily,
						Size = parentSegment.Font.Size,
						Bold = parentSegment.Font.Bold,
						Italic = parentSegment.Font.Italic,
						Underline = parentSegment.Font.Underline,
						Color = parentSegment.Font.Color,
					},
					Text = node.Value?
								.Replace("\r", "")
								.Replace("\n", " ")
								.Replace("  ", " ")
								.Trim()
								?? ""
				});

				return;
			}

			if (node.NodeType != XmlNodeType.Element)
			{
				return;
			}

			var xmlElement = node as XmlElement;
			var segment = new TextSegmentExtended
			{
				Font = new Font
				{
					Fontfamily = parentSegment.Font.Fontfamily,
					Size = parentSegment.Font.Size,
					Bold = parentSegment.Font.Bold,
					Italic = parentSegment.Font.Italic,
					Underline = parentSegment.Font.Underline,
					Color = parentSegment.Font.Color,
				}
			};

			if (xmlElement.Name.ToLower() == "br")
			{
				segment.Text = Environment.NewLine;
				parentSegment.Children.Add(segment);

				return;
			}

			if (xmlElement.Name.ToLower() == "p" || xmlElement.Name.ToLower() == "ul" || xmlElement.Name.ToLower() == "ol")
			{
				parentSegment.Children.Add(new TextSegmentExtended
				{
					Font = segment.Font,
					Text = Environment.NewLine
				});

				// HACK
				if (xmlElement.Name.ToLower() == "ul")
				{
					segment.LineIndent = 2.0;
				}
				else if (xmlElement.Name.ToLower() == "ol")
				{
					segment.LineIndent = 1.5;
				}
			}
			else if (xmlElement.Name.ToLower() == "b")
			{
				segment.Font.Bold = true;
			}
			else if (xmlElement.Name.ToLower() == "i")
			{
				segment.Font.Italic = true;
			}
			else if (xmlElement.Name.ToLower() == "u")
			{
				segment.Font.Underline = true;
			}

			CheckAttributes(xmlElement, segment);

			parentSegment.Children.Add(segment);

			foreach (XmlNode item in node.ChildNodes)
			{
				AnalyzeNode(item, segment);
			}

			if (xmlElement.Name.ToLower() == "p" || xmlElement.Name.ToLower() == "ul" || xmlElement.Name.ToLower() == "ol" || xmlElement.Name.ToLower() == "li")
			{
				parentSegment.Children.Add(new TextSegmentExtended
				{
					Font = segment.Font,
					Text = Environment.NewLine
				});
			}

			if (xmlElement.Name.ToLower() == "ul")
			{
				segment.LineIndent = 0.0;
				foreach (var childSegment in segment.Children.Where(s => s.Text != Environment.NewLine))
				{
					childSegment.Text = "-";
					childSegment.ReduceLineIndent = true;
					childSegment.ReduceLineIndentByText = $"{childSegment.Text} ";
					childSegment.SkipParagraphAlignment = true;
				}
			}
			else if (xmlElement.Name.ToLower() == "ol")
			{
				segment.LineIndent = 0.0;

				var counter = 1;
				foreach (var childSegment in segment.Children.Where(s => s.Text != Environment.NewLine))
				{
					childSegment.Text = $"{counter}.";
					counter++;
				}

				counter--;
				foreach (var childSegment in segment.Children.Where(s => s.Text != Environment.NewLine))
				{
					childSegment.ReduceLineIndent = true;
					childSegment.ReduceLineIndentByText = $"{counter}. ";
					childSegment.SkipParagraphAlignment = true;
				}
			}
			else if (xmlElement.Name.ToLower() == "li")
			{
				if (segment.LineIndent == 0.0 && parentSegment.LineIndent > 0.0)
				{
					segment.LineIndent = CENTIMETERTOPOINTFACTOR / parentSegment.LineIndent;
				}
			}
		}

		private void CheckAttributes(XmlElement xmlElement, TextSegment textSegment)
		{
			if (!xmlElement.HasAttributes)
			{
				return;
			}

			foreach (XmlAttribute attribute in xmlElement.Attributes)
			{
				if (attribute.Name.ToLower() == "style" && !attribute.Value.IsNullOrEmpty())
				{
					var styles = attribute.Value.Split(';');
					foreach (var style in styles)
					{
						var tuple = style.Split(':');
						if (tuple.Length != 2)
						{
							continue;
						}

						switch (tuple[0].Trim().ToLower())
						{
							case "font-family":
								textSegment.Font.Fontfamily = tuple[1].Split(',')[0].Trim();

								break;
							case "font-size":
								if (Double.TryParse(tuple[1].Trim(), out var size))
								{
									textSegment.Font.Size = size;
								}

								break;
							case "color":
								textSegment.Font.Color = tuple[1].Trim().ConvertHexColorToDecimalColor();

								break;
							case "margin-left" when xmlElement.Name.ToLower() == "li":
								if (Double.TryParse(tuple[1].Trim(), out var indent))
								{
									textSegment.LineIndent = indent;
								}

								break;
						}
					}

					break;
				}
			}
		}

		private class TextSegmentExtended : TextSegment
		{
			public TextSegmentExtended()
			{
				Children = new List<TextSegmentExtended>();
			}

			public IList<TextSegmentExtended> Children { get; set; }
		}
	}
}