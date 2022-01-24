using System.Linq;
using System.Xml.Serialization;
using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Interfaces;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ElementLine : ElementBase
	{
		[XmlAttribute]
		public string Color { get; set; }

		[XmlAttribute]
		public DashStyle Style { get; set; }

		[XmlAttribute]
		public double Linewidth { get; set; }

		[XmlAttribute]
		public bool MaxHeight { get; set; }

		[XmlAttribute]
		public bool EndsDiffHeight { get; set; }

		[XmlAttribute]
		public double SplitExtraMargin { get; set; }
		
		public override void Draw(IGraphics graphics, Point topLeftPage, Size sizePage)
		{
			var size = GetSize(graphics);
			var lineEnd = new Point(size.Width, size.Height);
			var lineStart = new Point(topLeftPage.X + PosX, topLeftPage.Y + PosY);
			var newLineEnd = lineEnd;

			if (PosX < sizePage.Width && PosY < sizePage.Height)
			{
				if (lineEnd.X > sizePage.Width && lineEnd.Y > sizePage.Height)
				{
					newLineEnd = CalculatEndCoordinate(lineStart, lineEnd, new Point(sizePage.Width, sizePage.Height));
				}
				else if (lineEnd.X > sizePage.Width)
				{
					newLineEnd = CalculatEndCoordinateY(lineStart, lineEnd, sizePage.Width);
				}
				else if (lineEnd.Y > sizePage.Height)
				{
					newLineEnd = CalculatEndCoordinateX(lineStart, lineEnd, sizePage.Height);
				}

				newLineEnd = new Point(newLineEnd.X + topLeftPage.X, newLineEnd.Y + topLeftPage.Y);

				graphics.DrawLine(Color, Linewidth, Style, lineStart, newLineEnd);
			}
		}

		public ElementLine Clone()
		{
			var line = new ElementLine();
			foreach (var propertyInfo in line.GetType().GetProperties().Where(p => p.CanWrite && p.CanRead))
			{
				propertyInfo.SetValue(line, propertyInfo.GetValue(this));
			}

			return line;
		}

		private Point CalculatEndCoordinateX(Point start, Point end, double newEndY)
		{
			var newEndX = end.X - (((end.X - start.X) / (end.Y - start.Y)) * (end.Y - newEndY));

			return new Point(newEndX, newEndY);
		}

		private Point CalculatEndCoordinateY(Point start, Point end, double newEndX)
		{
			var newEndY = end.Y - (((end.Y - start.Y) * (end.X - newEndX)) / (end.X - start.X));

			return new Point(newEndX, newEndY);
		}

		private Point CalculatEndCoordinate(Point start, Point end, Point container)
		{
			Point newEnd;
			// Increase
			var m = (end.Y - start.Y) / (end.X - start.X);
			// Calc constant c
			var c = end.Y - (m * end.X);
			// Line height when X reaches the container limit
			var yc = (m * container.X) + c;
			// Width of the line when Y reaches the container limit
			var xc = (container.Y - c) / m;

			if (yc > container.Y)
			{
				newEnd = new Point(xc, container.Y);
			}
			else if (xc > container.X)
			{
				newEnd = new Point(container.X, yc);
			}
			else
			{
				newEnd = new Point(xc, yc);
			}

			return newEnd;
		}
	}
}