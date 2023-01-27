using System;
using System.IO;
using System.Text;
using System.Xml;
using Eshava.Report.Pdf;
using Eshava.Report.Pdf.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Report.Pdf.NetCore
{
	[TestClass, TestCategory("Eshava.Report.Pdf.NetCore")]
	public class PdfPrinterTests
	{
		[TestMethod]
		public void GeneratePortraitDocumentTest()
		{
			var xmlFileName = "document_portrait";
			var xmlFullFileName = Path.Combine("Input", xmlFileName + ".xml");
			string xmlString;

			using (var stream = new StreamReader(File.Open(xmlFullFileName, FileMode.Open), Encoding.UTF8))
			{
				var doc = new XmlDocument();
				doc.Load(stream);
				xmlString = doc.OuterXml;
			}

			var fileStationeryFirstPage = File.Open(Path.Combine("Input", "stationery_first_page.pdf"), FileMode.Open);
			var fileStationeryFisrPageArray = default(byte[]);
			using (var memoryStream = new MemoryStream())
			{
				fileStationeryFirstPage.CopyTo(memoryStream);
				fileStationeryFisrPageArray = memoryStream.ToArray();
			}
			fileStationeryFirstPage.Close();

			var fileStationeryFollowingPage = File.Open(Path.Combine("Input", "stationery_following_page.pdf"), FileMode.Open);
			var fileStationeryFollowingPageArray = default(byte[]);
			using (var memoryStream = new MemoryStream())
			{
				fileStationeryFollowingPage.CopyTo(memoryStream);
				fileStationeryFollowingPageArray = memoryStream.ToArray();
			}
			fileStationeryFollowingPage.Close();

			var imageBlue = SkiaSharp.SKBitmap.Decode(Path.Combine("Input", "image_blue.png"));
			var imageGreen = SkiaSharp.SKBitmap.Decode(Path.Combine("Input", "image_green.png"));
			var imageRed = SkiaSharp.SKBitmap.Decode(Path.Combine("Input", "image_red.png"));

			try
			{
				var cacheItem = new CacheItem<SkiaSharp.SKBitmap>
				{
					Images = new System.Collections.Generic.Dictionary<string, SkiaSharp.SKBitmap>
					{
						{ "image_blue", imageBlue },
						{ "image_green", imageGreen },
						{ "image_red", imageRed }
					},
					Stationaries = new System.Collections.Generic.Dictionary<string, byte[]>
					{
						{ "stationery_first_page", fileStationeryFisrPageArray },
						{ "stationery_following_page", fileStationeryFollowingPageArray }
					}
				};

				var printer = new PdfPrinter();
				var document = printer.CreatePDF(xmlString, cacheItem);

				document.Save(Path.Combine(Environment.CurrentDirectory, xmlFileName + ".pdf"));
			}
			catch (System.Exception ex)
			{

			}
		}

		[TestMethod]
		public void GenerateLandscapeDocumentTest()
		{
			var xmlFileName = "document_landscape";
			var xmlFullFileName = Path.Combine("Input", xmlFileName + ".xml");
			string xmlString;

			using (var stream = new StreamReader(File.Open(xmlFullFileName, FileMode.Open), Encoding.UTF8))
			{
				var doc = new XmlDocument();
				doc.Load(stream);
				xmlString = doc.OuterXml;
			}

			var imageBlue = SkiaSharp.SKBitmap.Decode(Path.Combine("Input", "image_blue.png"));

			try
			{
				var cacheItem = new CacheItem<SkiaSharp.SKBitmap>
				{
					Images = new System.Collections.Generic.Dictionary<string, SkiaSharp.SKBitmap>
					{
						{ "image_blue", imageBlue }
					}
				};

				var printer = new PdfPrinter();
				var document = printer.CreatePDF(xmlString, cacheItem);

				document.Save(Path.Combine(Environment.CurrentDirectory, xmlFileName + ".pdf"));
			}
			catch (System.Exception ex)
			{

			}
		}
	}
}
