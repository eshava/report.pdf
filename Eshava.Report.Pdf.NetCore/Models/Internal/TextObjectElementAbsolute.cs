using PdfSharpCore.Pdf.Content.Objects;

namespace Eshava.Report.Pdf.Models.Internal
{
	/// <summary>
	/// Set text matrix and text line matrix
	/// <see cref="OpCodeName.Tm"/>
	/// </summary>
	internal class TextObjectElementAbsolute : TextobjectElement
	{
		public Transformation Transformation { get; set; }
	}
}