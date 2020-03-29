using System;

namespace Eshava.Report.Pdf.Core.Models
{
	public class Font
	{
		public string Fontfamily { get; set; }
		public double Size { get; set; }
		public bool Bold { get; set; }
		public bool Italic { get; set; }
		public bool Underline { get; set; }
		public string Color { get; set; }
	}
}