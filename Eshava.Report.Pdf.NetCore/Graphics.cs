using System;
using System.Collections.Generic;
using System.IO;
using Eshava.Report.Pdf.Core;
using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Extensions;
using Eshava.Report.Pdf.Core.Interfaces;
using Eshava.Report.Pdf.Core.Models;
using Eshava.Report.Pdf.Models;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;
using SixLabors.ImageSharp.Formats.Png;

namespace Eshava.Report.Pdf
{
	public class Graphics : AbstractGraphics, IGraphics
	{
		private XGraphics _xGraphics;
		private readonly Dictionary<string, SixLabors.ImageSharp.Image> _pictures;
		private bool _isDisposed = false;

		public Graphics(XGraphics xGraphics, Dictionary<string, SixLabors.ImageSharp.Image> pictures)
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

		public Size GetTextSize(Font font, double elementWidth, string text)
		{
			if (text.IsNullOrEmpty())
			{
				return new Size(0, 0);
			}

			var options = new XPdfFontOptions(PdfFontEncoding.Unicode);
			var xFont = new XFont(font.Fontfamily, font.Size, GetXFontstyle(font.Bold, font.Italic, font.Underline), options);
			var format = XStringFormats.Default;
			format.Alignment = XStringAlignment.Near;
			format.LineAlignment = XLineAlignment.Near;

			var singleLineHeight = CalculateSingleLineHeight(t => _xGraphics.MeasureString(t, xFont, format).Height);
			var textSize = CalculateTextSize(text, singleLineHeight, elementWidth, t => _xGraphics.MeasureString(t, xFont, format).Width);

			return textSize;
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
						var imageStream = new MemoryStream();
						localImage.Save(imageStream, new PngEncoder { CompressionLevel =  PngCompressionLevel.BestSpeed, ColorType = PngColorType.RgbWithAlpha });
						imageStream.Position = 0;
						image = XImage.FromStream(() => imageStream);
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

			return image == null ? null : new Image(image);
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

		public IGraphics DrawText(Font font, string text, Alignment alignment, Point topLeftPage, Size sizePage, Point topLeftText, Size textSize)
		{
			if (text.IsNullOrEmpty())
			{
				return this;
			}

			var options = new XPdfFontOptions(PdfFontEncoding.Unicode);
			var xFont = new XFont(font.Fontfamily, font.Size, GetXFontstyle(font.Bold, font.Italic, font.Underline), options);
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

		public XFontStyle GetXFontstyle(bool bold, bool italic, bool underline)
		{
			XFontStyle style;
			if (bold && italic && underline)
			{
				style = XFontStyle.Bold & XFontStyle.Italic & XFontStyle.Underline; //B, I, U
			}
			else if (bold && italic)
			{
				style = XFontStyle.Bold & XFontStyle.Italic;                        //B, I
			}
			else if (bold && underline)
			{
				style = XFontStyle.Bold & XFontStyle.Underline;                     //B, U
			}
			else if (bold)
			{
				style = XFontStyle.Bold;                                            //B
			}
			else if (italic && underline)
			{
				style = XFontStyle.Italic & XFontStyle.Underline;                   //I, U
			}
			else if (!italic && underline)
			{
				style = XFontStyle.Underline;                                       //U
			}
			else if (italic)
			{
				style = XFontStyle.Italic;                                          //I
			}
			else
			{
				style = XFontStyle.Regular;                                         //R
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
				return XColor.FromArgb(0, 0, 0);
			}
		}
	}
}