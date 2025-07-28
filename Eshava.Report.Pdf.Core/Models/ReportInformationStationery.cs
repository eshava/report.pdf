using System.Xml.Serialization;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ReportInformationStationery
	{
		[XmlElement("Stationery")]
		public string Stationery { get; set; }

		[XmlElement("NumberOfAllowedUsage")]
		public int NumberOfAllowedUsage { get; set; }

		[XmlElement("SortIndex")]
		public int SortIndex { get; set; }
	}
}