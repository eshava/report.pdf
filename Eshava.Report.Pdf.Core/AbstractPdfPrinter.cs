using System;
using System.Collections.Generic;
using Eshava.Report.Pdf.Core.Extensions;
using Eshava.Report.Pdf.Core.Interfaces;
using Eshava.Report.Pdf.Core.Models;
using Eshava.Report.Pdf.Interfaces;

namespace Eshava.Report.Pdf.Core
{
	public abstract class AbstractPdfPrinter<T, P> where T : IPdfDocument<P> where P : IPdfPage
	{
		protected abstract T GetPdfDocumentInstance(string internalDocumentId);
		protected abstract IGraphics GetGraphicsFromPdfPage(P pdfPage);
		protected abstract byte[] GetStationary(string internalDocumentId, string stationaryName);
		protected abstract void PrependPdfs(T document);
		protected abstract void AppendPdfs(T document);

		/// <summary>
		/// Creates a Pdf from a given Xml string
		/// </summary>
		///<param name="internalDocumentId">Internal document id</param>
		/// <param name="xml">Xml Document</param>
		/// <returns>PDF Dokument</returns>
		protected T CreatePDF(string internalDocumentId, string xml)
		{
			var document = GetPdfDocumentInstance(internalDocumentId);
			var reader = new XmlReader();
			var reportResult = reader.ReadXmlFromString(xml);

			if (reportResult.IsFaulty)
			{
				return default;
			}

			var report = reportResult.Data;

			SetDocumentInformation(document, report.Information);
			PrependPdfs(document);
			DrawReport(document, report);
			AppendPdfs(document);

			return document;
		}

		/// <summary>
		/// Creates a Pdf document from the passed report object
		/// </summary>
		/// <param name="document">Pdf document</param>
		/// <param name="report">Report object</param>
		private void DrawReport(T document, Models.Report report)
		{
			if (report == null)
			{
				return;
			}

			var moveLogic = new MoveElementLogic();
			var preparePage = document.AddPage();
			SetPageSettings(preparePage, report.Information);
			var pageCalculator = new PageCalculator(report, new Size(preparePage.Width, preparePage.Height));
			var graphics = GetGraphicsFromPdfPage(preparePage);

			if (report.Header != null && report.Header.FirstPage != null)
			{
				moveLogic.AnalyzeElements(graphics, report.Header.FirstPage);
			}

			if (report.Header != null && report.Header.FollowingPage != null)
			{
				moveLogic.AnalyzeElements(graphics, report.Header.FollowingPage);
			}

			if (report.Footer != null && report.Footer.FirstPage != null)
			{
				moveLogic.AnalyzeElements(graphics, report.Footer.FirstPage);
			}

			if (report.Footer != null && report.Footer.FirstPage != null)
			{
				moveLogic.AnalyzeElements(graphics, report.Footer.FirstPage);
			}

			var pages = pageCalculator.CalculatePages(graphics);
			document.RemovePage(preparePage);

			var stationery = GetStationary(document.InternalId, report.Information.Stationery);
			var stationery2nd = GetStationary(document.InternalId, report.Information.Stationery2nd) ?? stationery;

			foreach (var page in pages)
			{
				var currentPage = document.AddPage();
				SetPageSettings(currentPage, report.Information);
				graphics = GetGraphicsFromPdfPage(currentPage);

				if (stationery != null && page.PageNumber == 1)
				{
					graphics.SetStationery(stationery);
				}
				else if (stationery2nd != null && !report.Information.StationeryOnlyFirstPage && page.PageNumber > 1)
				{
					graphics.SetStationery(stationery2nd);
				}

				var headerHeight = page.Header?.Height ?? 0;

				DrawHeader(currentPage, graphics, page);
				DrawPositions(currentPage, graphics, page.Positions, page.Margins, new Size(page.MaxPositionPartWidth, page.MaxPositionPartHeight + headerHeight + page.Margins.Top), headerHeight);
				DrawFooter(currentPage, graphics, page);

				graphics.Dispose();
			}
		}

		/// <summary>
		/// Draws the passed positions on the Pdf page
		/// </summary>
		/// <param name="graphics">Graphics element</param>
		/// <param name="positions">Positions</param>
		/// <param name="margins">Page borders</param>
		/// <param name="positionArea">Size of the postion area of this page</param>
		/// <param name="headerHeight">Height of the header area of this page</param>
		private void DrawPositions(P page, IGraphics graphics, IEnumerable<ReportPosition> positions, PageMargins margins, Size positionArea, double headerHeight)
		{
			var positionAreaHeight = headerHeight;
			foreach (var position in positions)
			{
				//Determine starting point for drawing the position
				var start = new Point(margins.Left, margins.Top + positionAreaHeight);

				DrawElements(page, graphics, start, position, positionArea, 0, 0);

				//Add height of current position to current total page height
				positionAreaHeight += position.GetSize(graphics).Height;
			}
		}

		/// <summary>
		/// Draws the passed header on the Pdf page
		/// </summary>
		/// <param name="page">Current Pdf page</param>
		/// <param name="graphics">Graphics element</param>
		/// <param name="reportPage">Report page</param>
		private void DrawHeader(P page, IGraphics graphics, ReportPage reportPage)
		{
			if (reportPage.Header == default)
			{
				return;
			}

			var containerSize = new Size(page.Width - reportPage.Margins.Left - reportPage.Margins.Right, reportPage.Header.Height);
			var start = new Point(reportPage.Margins.Left, reportPage.Margins.Top);

			DrawElements(page, graphics, start, reportPage.Header, containerSize, reportPage.PageNumber, reportPage.TotalPageCount);
		}

		/// <summary>
		/// Draws the passed footer on the Pdf page
		/// </summary>
		/// <param name="page">Current Pdf page</param>
		/// <param name="graphics">Graphics element</param>
		/// <param name="reportPage">Report page</param>
		private void DrawFooter(P page, IGraphics graphics, ReportPage reportPage)
		{
			if (reportPage.Footer == default)
			{
				return;
			}

			var containerSize = new Size(page.Width - reportPage.Margins.Left - reportPage.Margins.Right, reportPage.Footer.Height);
			var maxSize = reportPage.Footer.GetSize(graphics);
			var start = new Point(reportPage.Margins.Left, page.Height - reportPage.Margins.Bottom - maxSize.Height);

			DrawElements(page, graphics, start, reportPage.Footer, containerSize, reportPage.PageNumber, reportPage.TotalPageCount);
		}

		private void DrawElements(P page, IGraphics graphics, Point start, ElementContainer part, Size container, int currentPageNumber, int totalPageCount)
		{
			var elements = part.GetAllElements();
			foreach (var element in elements)
			{
				//Check whether the page number must be set in the current element
				if (element is ElementPageNo)
				{
					((ElementPageNo)element).SetNumber(currentPageNumber, totalPageCount);
				}

				element.Draw(graphics, start, container);
				var hyperlink = element as IHyperlink;

				if (hyperlink != default && !hyperlink.Hyperlink.IsNullOrEmpty())
				{
					var location = hyperlink.GetHyperlinkPosition(graphics, start);
					page.AddWebLink(location.Start, location.Size, hyperlink.Hyperlink);
				}

				//Check whether the page number must be removed from the current element
				if (element is ElementPageNo)
				{
					((ElementPageNo)element).ResetNumber();
				}
			}
		}

		/// <summary>
		/// Sets information like author, title, creation date etc. 
		/// </summary>
		/// <param name="document">Pdf document</param>
		/// <param name="information">Information object</param>
		private void SetDocumentInformation(T document, ReportInformation information)
		{
			document.Author = information.Author;
			document.CreatedAt = DateTime.Now;
			document.CreatedBy = information.Author;
			document.Title = information.Title;
		}

		/// <summary>
		/// Sets the paper format (e.g. A4) and orientation (portrait / landscape) in a Pdf page
		/// </summary>
		/// <param name="page">Current Pdf page</param>
		/// <param name="information">Page information</param>
		private void SetPageSettings(P page, ReportInformation information)
		{
			page.Size = information.DocumentSize;
			page.Orientation = information.Orientation;
		}
	}
}