using System.Xml.Serialization;

namespace Eshava.Report.Pdf.Core.Models
{
	[XmlRoot("Report")]
	public class Report
	{
		public Report()
		{
			Information = new ReportInformation();
		}

		[XmlElement("Information")]
		public ReportInformation Information { get; set; }

		[XmlElement("Header")]
		public ReportPart Header { get; set; }

		[XmlElement("Footer")]
		public ReportPart Footer { get; set; }

		[XmlElement("Positions")]
		public ReportPositionContainer PositionContainer { get; set; }

		[XmlElement("PrePositions")]
		public ReportPositionContainer PrePositionContainer { get; set; }

		[XmlElement("PostPositions")]
		public ReportPositionContainer PostPositionContainer { get; set; }

		public void SortPositions()
		{
			PositionContainer?.SortPositions();
			PrePositionContainer?.SortPositions();
			PostPositionContainer?.SortPositions();
		}
	}
}
