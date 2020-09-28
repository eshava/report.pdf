using System.Xml.Serialization;
using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Extensions;
using Eshava.Report.Pdf.Core.Interfaces;
using Eshava.Report.Pdf.Enums;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ElementImage : ElementBase
	{
		[XmlAttribute]
		public Alignment Alignment { get; set; }

		[XmlAttribute]
		public VerticalAlignment VerticalAlignment { get; set; }

		[XmlAttribute]
		public Scale Scale { get; set; }

		public override void Draw(IGraphics graphics, Point start, Size container)
		{
			if (Content.IsNullOrEmpty())
			{
				return;
			}

			var calculationResult = CalculateSize(graphics);
			if (calculationResult.Image == default)
			{
				return;
			}

			var differenceWidth = calculationResult.ElementSize.Width - calculationResult.Size.Width;
			var differenceHeight = calculationResult.ElementSize.Height - calculationResult.Size.Height;
			var locationX = 0.0;
			var locationY = 0.0;

			switch (Alignment)
			{
				case Alignment.Left:
					locationX = start.X + PosX;
					break;
				case Alignment.Center:
					locationX = start.X + PosX + (differenceWidth / 2.0);
					break;
				case Alignment.Default:
				case Alignment.Right:
					locationX = start.X + PosX + differenceWidth;
					break;
			}

			switch (VerticalAlignment)
			{
				case VerticalAlignment.Default:
				case VerticalAlignment.Top:
					locationY = start.Y + PosY;
					break;
				case VerticalAlignment.Center:
					locationY = start.Y + PosY + (differenceHeight / 2.0);
					break;
				case VerticalAlignment.Bottom:
					locationY = start.Y + PosY + differenceHeight;
					break;
			}

			var location = new Point(locationX, locationY);

			graphics.DrawImage(calculationResult.Image, location, calculationResult.Size);
		}

		public override Size GetSize(IGraphics graphics)
		{
			return CalculateSize(graphics).Size;
		}

		private CalculationResult CalculateSize(IGraphics graphics)
		{
			var elementSize = new Size(Width, Height);
			var image = graphics.LoadImage(Content);
			if (image == default)
			{
				return new CalculationResult { Size = elementSize };
			}

			var imageSize = new Size(image.PixelWidth * 72 / image.HorizontalResolution, image.PixelHeight * 72 / image.VerticalResolution);

			var size = default(Size);
			switch (Scale)
			{
				case Scale.Width:
				case Scale.FitsWidth when imageSize.Width > elementSize.Width:
					size = ScaleWidth(elementSize, imageSize);
					break;
				case Scale.Default:
				case Scale.Height:
				case Scale.FitsHeight when imageSize.Height > elementSize.Height:
					size = ScaleHeight(elementSize, imageSize);
					break;
				case Scale.WidthOrHeight:
					var scale = elementSize.Height / imageSize.Height;

					if (scale * imageSize.Width <= elementSize.Width)
					{
						size = ScaleHeight(elementSize, imageSize);
					}
					else
					{
						size = ScaleWidth(elementSize, imageSize);
					}
					break;
				case Scale.FitsWidthOrHeight:

					if (imageSize.Height > elementSize.Height)
					{
						size = ScaleHeight(elementSize, imageSize);

						if (size.Width > elementSize.Width)
						{
							size = ScaleWidth(elementSize, size);
						}
					}
					else if (imageSize.Width > elementSize.Width)
					{
						size = ScaleWidth(elementSize, imageSize);

						if (size.Height > elementSize.Height)
						{
							size = ScaleHeight(elementSize, size);
						}
					}

					break;
			}

			return new CalculationResult
			{
				ElementSize = elementSize,
				ImageSize = imageSize,
				Image = image,
				Size = size ?? imageSize
			};
		}

		private Size ScaleWidth(Size elementSize, Size imageSize)
		{
			var scale = elementSize.Width / imageSize.Width;

			return new Size(imageSize.Width * scale, imageSize.Height * scale);
		}

		private Size ScaleHeight(Size elementSize, Size imageSize)
		{
			var scale = elementSize.Height / imageSize.Height;

			return new Size(imageSize.Width * scale, imageSize.Height * scale);
		}

		private class CalculationResult
		{
			public Size ElementSize { get; set; }
			public Size ImageSize { get; set; }
			public IImage Image { get; set; }
			public Size Size { get; set; }
		}
	}
}