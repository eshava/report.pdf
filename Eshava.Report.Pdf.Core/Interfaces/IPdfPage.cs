using Eshava.Report.Pdf.Core.Enums;

namespace Eshava.Report.Pdf.Core.Interfaces
{
	public interface IPdfPage
	{
		double Width { get; }
		double Height { get; }

		PageSize Size { get; set; }
		PageOrientation Orientation { get; set; }
	}
}