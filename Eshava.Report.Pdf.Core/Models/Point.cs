namespace Eshava.Report.Pdf.Core.Models
{
	public class Point
	{
		public Point()
		{

		}

		public Point(double x, double y)
		{
			X = x;
			Y = y;
		}

		public double X { get; set; }
		public double Y { get; set; }
	}
}