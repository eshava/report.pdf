using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Interfaces;

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
	}
}