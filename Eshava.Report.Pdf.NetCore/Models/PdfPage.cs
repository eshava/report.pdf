using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Interfaces;

namespace Eshava.Report.Pdf.Models
{
	public class PdfPage : IPdfPage
	{
		public PdfPage(PdfSharpCore.Pdf.PdfPage page, string internalDocumentId)
		{
			InternalDocumentId = internalDocumentId;
			Page = page;
		}

		public string InternalDocumentId { get; }
		public PdfSharpCore.Pdf.PdfPage Page { get; }
		public double Width => Page.Width;
		public double Height => Page.Height;

		public PageSize Size { get => (PageSize)Page.Size; set => Page.Size = (PdfSharpCore.PageSize)value; }
		public PageOrientation Orientation { get => (PageOrientation)Page.Orientation; set => Page.Orientation = (PdfSharpCore.PageOrientation)value; }
	}
}