using Eshava.Report.Pdf.Core.Interfaces;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ElementRectangle : ElementLine
	{
		public override void Draw(IGraphics graphics, Point topLeftPage, Size sizePage)
		{
			var rectangleSize = GetSize(graphics);
			var topLeft = new Point(topLeftPage.X + PosX, topLeftPage.Y + PosY);
			var bottomRight = new Point(rectangleSize.Width, rectangleSize.Height);
			
			graphics.DrawRectangle(Color, Linewidth, Style, topLeft, bottomRight, false);
		}
	}
}