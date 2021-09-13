using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Extensions;
using Eshava.Report.Pdf.Core.Interfaces;
using Eshava.Report.Pdf.Core.Models;
using Eshava.Report.Pdf.Models;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;

namespace Eshava.Report.Pdf
{
	public class Graphics : IGraphics
	{
		private XGraphics _xGraphics;
		private readonly Dictionary<string, System.Drawing.Image> _pictures;
		private bool _isDisposed = false;

		public Graphics(XGraphics xGraphics, Dictionary<string, System.Drawing.Image> pictures)
		{
			_xGraphics = xGraphics;
			_pictures = pictures;
		}

		public void Dispose()
		{
			if (!_isDisposed)
			{
				_xGraphics.Dispose();
				_xGraphics = null;
				_isDisposed = true;
			}
		}

		public double GetTextWidth(Font font, string text)
		{
			// Pdf Sharp 1.50 -> without gdi
			//var options = new XPdfFontOptions(PdfFontEncoding.Unicode);
			//var xFont = new XFont(font.Fontfamily, font.Size, GetXFontstyle(font.Bold, font.Italic, font.Underline, font.Strikeout), options);
			//var format = XStringFormats.Default;
			//format.Alignment = XStringAlignment.Near;
			//format.LineAlignment = XLineAlignment.Near;

			//return _xGraphics.MeasureString(text, xFont, format).Width;

			//Pdf Sharp 1.32 -> use gdi
			var frameworkFont = new System.Drawing.Font(font.Fontfamily, (float)font.Size, GetFontstyle(font.Bold, font.Italic, font.Underline, font.Strikeout));
			var xSize = XSize.FromSizeF(_xGraphics.Graphics.MeasureString(text, frameworkFont));
			xSize = new XSize(xSize.Width * 72 / _xGraphics.Graphics.DpiX, (xSize.Height * 72 / _xGraphics.Graphics.DpiY)); //Pixel to Point

			return xSize.Width;
		}

		public Size GetTextSize(IEnumerable<TextSegment> textSegments, double width)
		{
			//Fallback
			var text = String.Join(" ", textSegments.Select(ts => ts.Text));
			var font = textSegments.First().Font;

			return GetTextSize(font, width, text);
		}

		public Size GetTextSize(Font font, double elementWidth, string text)
		{
			// Pdf Sharp 1.50 -> without gdi
			//if (text.IsNullOrEmpty())
			//{
			//	return new Size(0, 0);
			//}

			//var options = new XPdfFontOptions(PdfFontEncoding.Unicode);
			//var xFont = new XFont(font.Fontfamily, font.Size, GetXFontstyle(font.Bold, font.Italic, font.Underline, font.Strikeout), options);
			//var format = XStringFormats.Default;
			//format.Alignment = XStringAlignment.Near;
			//format.LineAlignment = XLineAlignment.Near;

			//var singleLineHeight = CalculateSingleLineHeight(t => _xGraphics.MeasureString(t, xFont, format).Height);
			//var textSize = CalculateTextSize(text, singleLineHeight, elementWidth, t => _xGraphics.MeasureString(t, xFont, format).Width);

			//return textSize;


			//Pdf Sharp 1.32 -> use gdi
			var width = Convert.ToInt32(elementWidth * _xGraphics.Graphics.DpiX / 72); //Point to Pixel
			var frameworkFont = new System.Drawing.Font(font.Fontfamily, (float)font.Size, GetFontstyle(font.Bold, font.Italic, font.Underline, font.Strikeout));
			var xSize = XSize.FromSizeF(_xGraphics.Graphics.MeasureString(text, frameworkFont, width, System.Drawing.StringFormat.GenericDefault));
			xSize = new XSize(xSize.Width * 72 / _xGraphics.Graphics.DpiX, (xSize.Height * 72 / _xGraphics.Graphics.DpiY)); //Pixel to Point

			return new Size(xSize.Width, xSize.Height);
		}

		public IImage LoadImage(string imageName)
		{
			if (imageName.IsNullOrEmpty())
			{
				return null;
			}

			XImage image = null;
			try
			{

				if (_pictures != null && _pictures.ContainsKey(imageName))
				{
					var localImage = _pictures[imageName];
					if (localImage != null)
					{
						var imageStream = new MemoryStream((byte[])new System.Drawing.ImageConverter().ConvertTo(localImage, typeof(byte[])));
						// Pdf Sharp 1.50 -> without gdi
						//image = XImage.FromStream(imageStream);

						//Pdf Sharp 1.32 -> use gdi
						image = XImage.FromGdiPlusImage(localImage);
					}
				}
				else if (File.Exists(imageName))
				{
					image = XImage.FromFile(imageName);
				}
			}
			catch
			{
				image = null;
			}

			if (image == null)
			{
				return null;
			}

			return new Image(image);
		}

		public IGraphics DrawImage(IImage image, Point location, Size size)
		{
			if (image == null || location == null || size == null)
			{
				return this;
			}

			_xGraphics.DrawImage((image as Image).XImage, new XRect(new XPoint(location.X, location.Y), new XSize(size.Width, size.Height)));

			return this;
		}

		public IGraphics DrawLine(string color, double lineWidth, DashStyle dashStyle, Point topLeft, Point bottomRight)
		{
			var pen = new XPen(TranslateColor(color), lineWidth) { DashStyle = (XDashStyle)dashStyle };
			_xGraphics.DrawLine(pen, topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);

			return this;
		}

		public IGraphics DrawRectangle(string color, double lineWidth, DashStyle dashStyle, Point topLeft, Point bottomRight, bool fill)
		{
			if (fill)
			{
				XBrush brush = new XSolidBrush(TranslateColor(color));
				_xGraphics.DrawRectangle(brush, topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
			}
			else
			{
				var pen = new XPen(TranslateColor(color), lineWidth) { DashStyle = (XDashStyle)dashStyle };
				_xGraphics.DrawRectangle(pen, topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
			}

			return this;
		}

		public IGraphics DrawText(IEnumerable<TextSegment> textSegments, Alignment alignment, Point topLeftPage, Size sizePage, Point topLeftText, Size textSize)
		{
			// Fallback

			var text = String.Join(" ", textSegments.Select(ts => ts.Text));
			var font = textSegments.First().Font;

			return DrawText(font, text, alignment, topLeftPage, sizePage, topLeftText, textSize);
		}

		public IGraphics DrawText(Font font, string text, Alignment alignment, Point topLeftPage, Size sizePage, Point topLeftText, Size textSize)
		{
			if (text.IsNullOrEmpty())
			{
				return this;
			}

			var options = new XPdfFontOptions(PdfFontEncoding.Unicode);
			var xFont = new XFont(font.Fontfamily, font.Size, GetXFontstyle(font.Bold, font.Italic, font.Underline, font.Strikeout), options);
			var format = XStringFormats.Default;
			var brush = new XSolidBrush(TranslateColor(font.Color));
			var xSize = new XSize(textSize.Width, textSize.Height);
			var topLeftTotal = new XPoint(topLeftPage.X + topLeftText.X, topLeftPage.Y + topLeftText.Y);
			var tf = new XTextFormatter(_xGraphics);

			if (topLeftText.X < sizePage.Width && topLeftText.Y < sizePage.Height)
			{
				if (topLeftText.X + xSize.Width > sizePage.Width)
				{
					xSize = new XSize(sizePage.Width - topLeftText.X, xSize.Height);
				}

				if (topLeftText.Y + xSize.Height > sizePage.Height)
				{
					xSize = new XSize(xSize.Width, sizePage.Height - topLeftText.Y);
				}

				format.Alignment = XStringAlignment.Near;
				format.LineAlignment = XLineAlignment.Near;
				tf.Alignment = (XParagraphAlignment)alignment;
				tf.DrawString(text, xFont, brush, new XRect(topLeftTotal, xSize), format);
			}

			return this;
		}

		public IGraphics SetStationery(byte[] stationery)
		{
			if (stationery == null)
			{
				return this;
			}

			var pdf = XPdfForm.FromStream(new MemoryStream(stationery));
			var point = new XPoint(0, 0);

			_xGraphics.DrawImage(pdf, point);

			return this;
		}

		private System.Drawing.FontStyle GetFontstyle(bool bold, bool italic, bool underline, bool strikeout)
		{
			var style = System.Drawing.FontStyle.Regular;
			if (bold)
			{
				style |=  System.Drawing.FontStyle.Bold;
			}
			if (italic)
			{
				style |=  System.Drawing.FontStyle.Italic;
			}
			if (underline)
			{
				style |=  System.Drawing.FontStyle.Underline;
			}
			if (strikeout)
			{
				style |=  System.Drawing.FontStyle.Strikeout;
			}

			return style;
		}

		public XFontStyle GetXFontstyle(bool bold, bool italic, bool underline, bool strikeout)
		{
			var style = XFontStyle.Regular;
			if (bold)
			{
				style |= XFontStyle.Bold;
			}
			if (italic)
			{
				style |= XFontStyle.Italic;
			}
			if (underline)
			{
				style |= XFontStyle.Underline;
			}
			if (strikeout)
			{
				style |= XFontStyle.Strikeout;
			}

			return style;
		}

		private XColor TranslateColor(string color)
		{
			try
			{
				var parts = color.Split(' ');

				return XColor.FromArgb(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]), Convert.ToInt32(parts[3]));
			}
			catch
			{
				return XColor.FromArgb(0, 0, 0); // System.Drawing.Color.Black;
			}
		}

		//private double CalculateSingleLineHeight(Func<string, double> measureTextHeight)
		//{
		//	var spaceHeight = measureTextHeight(" ");
		//	var totalMultilineHeight = measureTextHeight("#\n#");
		//	var lineGap = totalMultilineHeight - spaceHeight - spaceHeight;

		//	return lineGap + spaceHeight;
		//}

		//private double GetSpaceWidth(Func<string, double> measureTextWith)
		//{
		//	return measureTextWith("x x") - measureTextWith("xx");
		//}

		//private Size CalculateTextSize(string text, double singleLineHeight, double elementWidth, Func<string, double> measureTextWith)
		//{
		//	var spaceWidth = GetSpaceWidth(measureTextWith);
		//	var height = 0.0;
		//	var maxWidth = 0.0;
		//	var lines = text.Replace("\r", "").Split('\n');

		//	foreach (var line in lines)
		//	{
		//		var words = line.Split(' ');
		//		var lineWidth = 0.0;
		//		height += singleLineHeight;

		//		foreach (var word in words)
		//		{
		//			var wordWidth = measureTextWith(word);
		//			if (lineWidth + wordWidth > elementWidth)
		//			{
		//				lineWidth = wordWidth;
		//				height += singleLineHeight;
		//			}
		//			else
		//			{
		//				lineWidth += wordWidth + spaceWidth;
		//				maxWidth = Math.Max(maxWidth, lineWidth);
		//			}
		//		}
		//	}

		//	return new Size(maxWidth, height);
		//}
	}
}