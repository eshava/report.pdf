namespace Eshava.Test.Report.Pdf.NetCore
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var test = new PdfPrinterTests();
			test.GeneratePortraitDocumentTest();
			//test.GenerateLandscapeDocumentTest();
		}
	}
}