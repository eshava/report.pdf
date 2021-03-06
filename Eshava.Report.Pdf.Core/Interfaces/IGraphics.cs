﻿using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Models;

namespace Eshava.Report.Pdf.Core.Interfaces
{
	public interface IGraphics
	{
		Size GetTextSize(Font font, double width, string text);
		IGraphics SetStationery(byte[] stationery);
		IImage LoadImage(string imageName);
		IGraphics DrawImage(IImage image, Point location, Size size);
		IGraphics DrawText(Font font, string text, Alignment alignment, Point topLeftPage, Size sizePage, Point topLeftText, Size textSize);
		IGraphics DrawLine(string color, double lineWidth, DashStyle dashStyle, Point topLeft, Point bottomRight);
		IGraphics DrawRectangle(string color, double lineWidth, DashStyle dashStyle, Point topLeft, Point bottomRight, bool fill);
		void Dispose();
	}
}