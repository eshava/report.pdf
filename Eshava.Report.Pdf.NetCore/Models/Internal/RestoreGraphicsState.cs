using PdfSharpCore.Pdf.Content.Objects;

namespace Eshava.Report.Pdf.Models.Internal
{
	/// <summary>
	/// Restore graphics state
	/// </summary>
	internal class RestoreGraphicsState : AbstractPdfOperation
	{
		public override OpCodeName OperationCode => OpCodeName.Q;
	}
}