using PdfSharpCore.Pdf.Content.Objects;

namespace Eshava.Report.Pdf.Models.Internal
{
	internal abstract class TextobjectElement
	{
		/// <summary>
		/// Show text
		/// <see cref="OpCodeName.Tj"/>
		/// Show text, allowing individual glyph positioning
		/// <see cref="OpCodeName.TJ"/>
		/// </summary>
		public string Value { get; set; }
	}
}