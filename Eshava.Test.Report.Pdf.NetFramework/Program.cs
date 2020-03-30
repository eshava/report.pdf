namespace Eshava.Test.Report.Pdf.NetFramework
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var test = new PdfPrinterTests();
			test.GeneratePortraitDocumentTest();
			test.GenerateLandscapeDocumentTest();
		}
	}
}