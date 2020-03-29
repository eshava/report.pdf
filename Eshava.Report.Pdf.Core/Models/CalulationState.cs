using System.Collections.Generic;

namespace Eshava.Report.Pdf.Core.Models
{
	public class CalulationState
	{
		public CalulationState()
		{
			IsFirstPage = true;
			CurrentPageNumber = 1;
			PositionsToRepeat = new List<ReportPosition>();
			Pages = new List<ReportPage>();
		}

		public bool IsFirstPage { get; set; }
		public bool IsNewPage { get; set; }
		public int CurrentPageNumber { get; set; }
		public int CurrentSequenceNumber { get; set; }
		public double PositionPartHeight { get; set; }
		public double PositionToRepeatHeight { get; set; }
		public double NewPageHeight { get; set; }
		public List<ReportPosition> PositionsToRepeat { get; private set; }
		public List<ReportPage> Pages { get; private set; }
	}
}