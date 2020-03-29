using System;
using System.Xml.Serialization;
using Eshava.Report.Pdf.Core.Interfaces;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ReportPagePart : ElementContainer
	{
		[XmlAttribute]
		public double Height { get; set; }

		/// <summary>
		/// Returns the size of the element calculated using its elements
		/// </summary>
		/// <param name="graphics">Graphics element</param>
		/// <returns>Total size</returns>
		public override Size GetSize(IGraphics graphics)
		{
			var size = base.GetSize(graphics);
			// Current widths are taken from the elements, but when drawing, the width is automatically determined by the page width minus margins
			var width = size.Width; 

			// The part is always as high as specified, unless 0 (zero) is specified
			var height = Math.Abs(Height) < 0.001 ? size.Height : Height;

			return new Size(width, height);
		}
	}
}