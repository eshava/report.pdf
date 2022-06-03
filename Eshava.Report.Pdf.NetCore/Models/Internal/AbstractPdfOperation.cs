using PdfSharpCore.Pdf.Content.Objects;

namespace Eshava.Report.Pdf.Models.Internal
{
	internal abstract class AbstractPdfOperation
	{
		public abstract OpCodeName OperationCode { get; }
		public int PageIndex { get; set; }
	}
}