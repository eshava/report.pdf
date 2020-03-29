namespace Eshava.Report.Pdf.Core.Models
{
	public class Size
	{
		public Size()
		{

		}

		public Size(double width, double height)
		{
			Width = width;
			Height = height;
		}

		public double Height { get; set; }
		public double Width { get; set; }
	}
}