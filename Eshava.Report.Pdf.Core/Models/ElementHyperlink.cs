using System.Xml.Serialization;
using Eshava.Report.Pdf.Core.Interfaces;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ElementHyperlink : ElementText
	{
		[XmlAttribute]
		public string Hyperlink { get; set; }

		public (Point Start, Size Size) GetElementPosition(IGraphics graphics, Point topLeftPage)
		{
			var topLeftTotal = new Point(topLeftPage.X + PosX, topLeftPage.Y + PosY);

			return (topLeftTotal, GetSize(graphics));
		}
	}	
}