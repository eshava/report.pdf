using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Models;

namespace Eshava.Report.Pdf.Core.Interfaces
{
	public interface IPdfPage
	{
		double Width { get; }
		double Height { get; }

		PageSize Size { get; set; }
		PageOrientation Orientation { get; set; }

		void AddWebLink(Point start, Size size, string hyperlink);
	}
}