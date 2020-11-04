using Eshava.Report.Pdf.Core.Interfaces;
using Eshava.Report.Pdf.Core.Models;

namespace Eshava.Report.Pdf.Interfaces
{
	public interface IHyperlink
	{
		string Hyperlink { get;  }
		(Point Start, Size Size) GetHyperlinkPosition(IGraphics graphics, Point topLeftPage);
	}
}