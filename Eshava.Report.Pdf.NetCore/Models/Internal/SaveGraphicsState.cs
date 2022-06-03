using PdfSharpCore.Pdf.Content.Objects;

namespace Eshava.Report.Pdf.Models.Internal
{
	/// <summary>
	/// Save graphics state
	/// </summary>
	internal class SaveGraphicsState : AbstractPdfOperation
	{
		public override OpCodeName OperationCode => OpCodeName.q;
	}
}