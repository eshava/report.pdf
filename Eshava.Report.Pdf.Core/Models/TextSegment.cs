namespace Eshava.Report.Pdf.Core.Models
{
	public class TextSegment
	{
		public string Text { get; set; }

		public Font Font { get; set; }

		public double LineIndent { get; set; }
		public bool ReduceLineIndent { get; set; }
		public string ReduceLineIndentByText { get; set; }
		public bool SkipParagraphAlignment { get; set; }
	}
}