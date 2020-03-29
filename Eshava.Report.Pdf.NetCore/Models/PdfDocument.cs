using System;
using Eshava.Report.Pdf.Core.Interfaces;

namespace Eshava.Report.Pdf.Models
{
	public class PdfDocument : IPdfDocument<PdfPage>
	{
		public PdfDocument(string internalId)
		{
			InternalId = internalId;
			Pdf = new PdfSharpCore.Pdf.PdfDocument();
		}

		public string InternalId { get; }
		public PdfSharpCore.Pdf.PdfDocument Pdf { get; }
		public string Author { get => Pdf.Info.Author; set => Pdf.Info.Author = value; }
		public string CreatedBy { get => Pdf.Info.Creator; set => Pdf.Info.Creator = value; }
		public string Title { get => Pdf.Info.Title; set => Pdf.Info.Title = value; }
		public DateTime CreatedAt { get => Pdf.Info.CreationDate; set => Pdf.Info.CreationDate = value; }

		public PdfPage AddPage()
		{
			var page = Pdf.AddPage();

			return new PdfPage(page, InternalId);
		}

		public PdfPage AddPage(PdfSharpCore.Pdf.PdfPage page)
		{
			return new PdfPage(page, InternalId);
		}

		public void RemovePage(PdfPage page)
		{
			Pdf.Pages.Remove(page.Page);
		}
	}
}