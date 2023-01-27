using System;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using Eshava.Report.Pdf.Core;
using Eshava.Report.Pdf.Core.Extensions;
using Eshava.Report.Pdf.Core.Interfaces;
using Eshava.Report.Pdf.Core.Models;
using Eshava.Report.Pdf.Models;
using PdfSharpCore.Pdf.IO;

namespace Eshava.Report.Pdf
{
	public class PdfPrinter : AbstractPdfPrinter<PdfDocument, PdfPage>
	{
		private readonly MemoryCache _itemCache;

		public PdfPrinter()
		{
			_itemCache = MemoryCache.Default;
		}

		public PdfSharpCore.Pdf.PdfDocument CreatePDF(string xml, CacheItem<SkiaSharp.SKBitmap> cacheItem = null)
		{
			if (cacheItem == null)
			{
				cacheItem = new CacheItem<SkiaSharp.SKBitmap>();
			}

			var cacheItemPolicy = new CacheItemPolicy();
			var internalDocumentId = Guid.NewGuid().ToString();
			while (_itemCache.Contains(internalDocumentId))
			{
				internalDocumentId = Guid.NewGuid().ToString();
			}

			_itemCache.Set(internalDocumentId, cacheItem, cacheItemPolicy);

			var pdfDocument = base.CreatePDF(internalDocumentId, xml);

			_itemCache.Remove(internalDocumentId);

			return pdfDocument?.Pdf;
		}


		protected override IGraphics GetGraphicsFromPdfPage(PdfPage pdfPage)
		{
			var xGraphics = PdfSharpCore.Drawing.XGraphics.FromPdfPage(pdfPage.Page);
			var cacheItem = _itemCache.Get(pdfPage.InternalDocumentId) as CacheItem<SkiaSharp.SKBitmap>;

			return new Graphics(xGraphics, cacheItem.Images);
		}

		protected override PdfDocument GetPdfDocumentInstance(string internalDocumentId)
		{
			return new PdfDocument(internalDocumentId);
		}

		protected override byte[] GetStationary(string internalDocumentId, string stationaryName)
		{
			var cacheItem = _itemCache.Get(internalDocumentId) as CacheItem<SkiaSharp.SKBitmap>;

			return cacheItem.Stationaries.CheckDictionary(stationaryName);
		}

		protected override void PrependPdfs(PdfDocument document)
		{
			var cacheItem = _itemCache.Get(document.InternalId) as CacheItem<SkiaSharp.SKBitmap>;

			foreach (var pdfDocument in cacheItem.PrependPdfs.OrderBy(t => t.SequenceNumber))
			{
				AddPdfToDocument(document, pdfDocument.Pdf);
			}
		}
		protected override void AppendPdfs(PdfDocument document)
		{
			var cacheItem = _itemCache.Get(document.InternalId) as CacheItem<SkiaSharp.SKBitmap>;

			foreach (var pdfDocument in cacheItem.AppendPdfs.OrderBy(t => t.SequenceNumber))
			{
				AddPdfToDocument(document, pdfDocument.Pdf);
			}
		}

		private void AddPdfToDocument(PdfDocument document, byte[] pdf)
		{
			var pdfStream = new MemoryStream(pdf)
			{
				Position = 0
			};

			var appendPdf = PdfReader.Open(pdfStream, PdfDocumentOpenMode.Import);
			foreach (var page in appendPdf.Pages)
			{
				document.AddPage(page);
			}
		}
	}
}