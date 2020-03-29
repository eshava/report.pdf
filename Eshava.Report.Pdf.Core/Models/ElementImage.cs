using System.Xml.Serialization;
using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Extensions;
using Eshava.Report.Pdf.Core.Interfaces;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ElementImage : ElementBase
	{
		[XmlAttribute]
		public Alignment Alignment { get; set; }

		public override void Draw(IGraphics graphics, Point start, Size container)
		{
			if (Content.IsNullOrEmpty())
			{
				return;
			}

			var image = graphics.LoadImage(Content);
			if (image == null)
			{
				return;
			}

			var imageSize = new Size(image.PixelWidth * 72 / image.HorizontalResolution, image.PixelHeight * 72 / image.VerticalResolution);
			var size = GetSize(graphics);

			if (PosX > container.Width || PosY > container.Height)
			{
				return;
			}

			if (PosX + size.Width > container.Width)
			{
				size = new Size(container.Width - PosX, size.Height);
			}

			if (PosY + size.Height > container.Height)
			{
				size = new Size(size.Width, container.Height - PosY);
			}

			var scale = size.Height / imageSize.Height;
			var differenceWidth = size.Width - (imageSize.Width * scale);
			size = new Size(imageSize.Width * scale, imageSize.Height * scale);

			Point location;
			switch (Alignment)
			{
				case Alignment.Left:
					location = new Point(start.X + PosX, start.Y + PosY);
					break;
				case Alignment.Center:
					location = new Point(start.X + PosX + (differenceWidth / 2.0), start.Y + PosY);
					break;
				case Alignment.Right:
				default:
					location = new Point(start.X + PosX + differenceWidth, start.Y + PosY);
					break;
			}

			graphics.DrawImage(image, location, size);
		}
	}
}