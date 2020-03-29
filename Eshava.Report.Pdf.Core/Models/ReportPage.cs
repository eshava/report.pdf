using System.Collections.Generic;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ReportPage
	{
		public ReportPagePart Header { get; set; }

		public ReportPagePart Footer { get; set; }

		public List<ReportPosition> Positions { get; set; }

		public int PageNumber { get; set; }

		public int TotalPageCount { get; set; }

		public double MaxPositionPartHeight { get; private set; }

		public double MaxPositionPartWidth { get; set; }

		public PageMargins Margins { get; set; }
			   
		public void CalcPositionsHeightPart(double pageHeight)
		{
			MaxPositionPartHeight = pageHeight - Margins.Top - (Header == null ? 0 : Header.Height) - Margins.Bottom - (Footer == null ? 0 : Footer.Height);
		}
	}
}