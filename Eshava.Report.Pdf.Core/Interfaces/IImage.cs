namespace Eshava.Report.Pdf.Core.Interfaces
{
	public interface IImage
	{
		int PixelWidth { get; }
		int PixelHeight { get; }

		double HorizontalResolution { get; }
		double VerticalResolution { get; }
	}
}