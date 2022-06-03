using System.Collections.Generic;
using System.Linq;
using PdfSharpCore.Pdf.Content.Objects;

namespace Eshava.Report.Pdf.Models.Internal
{
	internal class TextObject : AbstractPdfOperation
	{
		public override OpCodeName OperationCode => OpCodeName.BT;
		public bool IsEmpty => Elements.All(e => (e is TextObjectElementFont));
		public List<TextobjectElement> Elements { get; set; }

	}
}