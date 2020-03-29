using System.Xml.Serialization;
using Eshava.Report.Pdf.Core.Enums;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ReportInformation
	{
		public ReportInformation()
		{
			Orientation = PageOrientation.Portrait;
			DocumentSize = PageSize.A4;
		}
		
		[XmlElement("Stationery")]
		public string Stationery { get; set; }

		[XmlElement("Stationery2nd")]
		public string Stationery2nd { get; set; }

		[XmlElement("StationeryOnlyFirstPage")]
		public bool StationeryOnlyFirstPage { get; set; }

		[XmlElement("Watermark")]
		public string Watermark { get; set; }

		[XmlElement("Orientation")]
		public PageOrientation Orientation { get; set; }

		[XmlElement("Reporttitle")]
		public string Title { get; set; }

		[XmlElement("Reportauthor")]
		public string Author { get; set; }

		[XmlElement("DocumentSize")]
		public PageSize DocumentSize { get; set; }

		[XmlElement("PageMargins")]
		public PageMargins PageMargins { get; set; }
	}
}