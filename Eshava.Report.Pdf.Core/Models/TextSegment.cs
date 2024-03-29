﻿namespace Eshava.Report.Pdf.Core.Models
{
	public class TextSegment
	{
		public string Text { get; set; }

		public Font Font { get; set; }

		public double LineIndent { get; set; }
		public bool ReduceLineIndent { get; set; }
		public string ReduceLineIndentByText { get; set; }
		public bool SkipParagraphAlignment { get; set; }

		public TextSegment Clone()
		{
			return new TextSegment
			{
				Text = Text,
				Font = Font,
				LineIndent = LineIndent,
				ReduceLineIndent = ReduceLineIndent,
				ReduceLineIndentByText = ReduceLineIndentByText,
				SkipParagraphAlignment = SkipParagraphAlignment
			};
		}
	}
}