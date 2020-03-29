using System.Xml.Serialization;

namespace Eshava.Report.Pdf.Core.Models
{
	public class PageMargins
	{
		public PageMargins(double left, double top, double right, double bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public PageMargins()
		{

		}

		[XmlAttribute]
		public double Left { get; set; }

		[XmlAttribute]
		public double Top { get; set; }

		[XmlAttribute]
		public double Right { get; set; }

		[XmlAttribute]
		public double Bottom { get; set; }
	}
}