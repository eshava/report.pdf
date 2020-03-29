using System.Xml.Serialization;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ReportPart
	{
		[XmlElement("FirstPage")]
		public ReportPagePart FirstPage { get; set; }

		[XmlElement("FollowingPage")]
		public ReportPagePart FollowingPage { get; set; }
	}
}