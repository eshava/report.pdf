using PdfSharpCore.Pdf.Content.Objects;

namespace Eshava.Report.Pdf.Models.Internal
{
	/// <summary>
	/// Concatenate matrix to current transformation matrix
	/// </summary>
	internal class Concatenate : AbstractPdfOperation
	{
		public override OpCodeName OperationCode => OpCodeName.cm;

		public Transformation Transformation { get; set; }
	}
}