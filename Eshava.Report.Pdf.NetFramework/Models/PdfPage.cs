using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Interfaces;
using Eshava.Report.Pdf.Core.Models;

namespace Eshava.Report.Pdf.Models
{
	public class PdfPage : IPdfPage
	{
		public PdfPage(PdfSharp.Pdf.PdfPage page, string internalDocumentId)
		{
			InternalDocumentId = internalDocumentId;
			Page = page;
		}

		public string InternalDocumentId { get; }
		public PdfSharp.Pdf.PdfPage Page { get; }
		public double Width => Page.Width;
		public double Height => Page.Height;

		public PageSize Size { get => (PageSize)Page.Size; set => Page.Size = (PdfSharp.PageSize)value; }
		public PageOrientation Orientation { get => (PageOrientation)Page.Orientation; set => Page.Orientation = (PdfSharp.PageOrientation)value; }

		public void AddWebLink(Point start, Size size, string hyperlink)
		{
			// invert y coordinate, because page 0,0 is bottom/left instead of top/left 
			var startPoint = new PdfSharp.Drawing.XPoint(start.X, Height - start.Y - size.Height);
			var rectangle = new PdfSharp.Pdf.PdfRectangle(startPoint, new PdfSharp.Drawing.XSize(size.Width, size.Height));

			Page.AddWebLink(rectangle, hyperlink);
		}
	}
}