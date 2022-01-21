using System.Collections.Generic;
using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Models;

namespace Eshava.Report.Pdf.Core.Interfaces
{
	public interface IGraphics
	{
		double GetTextWidth(Font font, string text);
		Size GetTextSize(IEnumerable<TextSegment> textSegments, double width, Alignment alignment);
		Size GetTextSize(Font font, double width, string text, Alignment alignment);
		IGraphics SetStationery(byte[] stationery);
		IImage LoadImage(string imageName);
		IGraphics DrawImage(IImage image, Point location, Size size);
		IGraphics DrawText(Font font, string text, Alignment alignment, Point topLeftPage, Size sizePage, Point topLeftText, Size textSize);
		IGraphics DrawText(IEnumerable<TextSegment> textSegments, Alignment alignment, Point topLeftPage, Size sizePage, Point topLeftText, Size textSize);

		IGraphics DrawLine(string color, double lineWidth, DashStyle dashStyle, Point topLeft, Point bottomRight);
		IGraphics DrawRectangle(string color, double lineWidth, DashStyle dashStyle, Point topLeft, Point bottomRight, bool fill);
		void Dispose();
	}
}