using Eshava.Report.Pdf.Core.Interfaces;
using PdfSharp.Drawing;

namespace Eshava.Report.Pdf.Models
{
	public class Image : IImage
	{
		public Image(XImage image)
		{
			XImage = image;
		}

		public int PixelWidth => XImage.PixelWidth;

		public int PixelHeight => XImage.PixelHeight;

		public double HorizontalResolution => XImage.HorizontalResolution;

		public double VerticalResolution => XImage.VerticalResolution;

		public XImage XImage { get; }
	}
}