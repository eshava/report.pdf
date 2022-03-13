using System;
using System.Collections.Generic;
using System.Linq;
using Eshava.Report.Pdf.Models;
using PdfSharpCore.Pdf.Content.Objects;

namespace Eshava.Report.Pdf.Extensions
{
	/// <summary>
	/// Origin of the Code
	/// https://github.com/DavidS/PdfTextract/blob/master/PdfTextract/PdfTextExtractor.cs
	/// </summary>
	public static class PdfDocumentExtensions
	{
		public static IEnumerable<TextContent> ExtractText(this PdfSharpCore.Pdf.PdfDocument document)
		{
			return ExtractText(document, 0, document.Pages.Count - 1);
		}

		public static IEnumerable<TextContent> ExtractText(this PdfSharpCore.Pdf.PdfDocument document, int pageIndex)
		{
			return ExtractText(document, pageIndex, pageIndex);
		}

		public static IEnumerable<TextContent> ExtractText(this PdfSharpCore.Pdf.PdfDocument document, int pageIndexFrom, int pageIndexTo)
		{
			var pdfContent = new List<TextContent>();

			for (var pageIndex = pageIndexFrom; pageIndex <= pageIndexTo; pageIndex++)
			{
				var page = document.Pages[pageIndex];
				var content = PdfSharpCore.Pdf.Content.ContentReader.ReadContent(page);
				
				ExtractText(content, pdfContent, pageIndex);
			}

			pdfContent = pdfContent
				.OrderBy(c => c.PageIndex)
				.ThenByDescending(c => c.PosY)
				.ThenBy(c => c.PosX)
				.ToList();

			return pdfContent;
		}

		private static void ExtractText(CObject @object, IList<TextContent> pdfContent, int pageIndex)
		{
			if (@object is CArray array)
			{
				ExtractText(array, pdfContent, pageIndex);
			}
			else if (@object is CComment comment)
			{
				ExtractText(comment, pdfContent, pageIndex);
			}
			else if (@object is CInteger integer)
			{
				ExtractText(integer, pdfContent, pageIndex);
			}
			else if (@object is CName name)
			{
				ExtractText(name, pdfContent, pageIndex);
			}
			else if (@object is CNumber number)
			{
				ExtractText(number, pdfContent, pageIndex);
			}
			else if (@object is COperator @operator)
			{
				ExtractText(@operator, pdfContent, pageIndex);
			}
			else if (@object is CReal real)
			{
				ExtractText(real, pdfContent, pageIndex);
			}
			else if (@object is CSequence sequence)
			{
				ExtractText(sequence, pdfContent, pageIndex);
			}
			else if (@object is CString @string)
			{
				ExtractText(@string, pdfContent, pageIndex);
			}
			else
			{
				throw new NotImplementedException(@object.GetType().AssemblyQualifiedName);
			}
		}

		private static void ExtractText(CArray array, IList<TextContent> pdfContent, int pageIndex)
		{
			foreach (var element in array)
			{
				ExtractText(element, pdfContent, pageIndex);
			}
		}

		private static void ExtractText(COperator @operator, IList<TextContent> pdfContent, int pageIndex)
		{
			if (@operator.OpCode.OpCodeName == OpCodeName.Tm)
			{
				pdfContent.Add(new TextContent
				{
					PageIndex = pageIndex,
					PosX = ((CReal)@operator.Operands[4]).Value,
					PosY = ((CReal)@operator.Operands[5]).Value
				});
			}

			if (@operator.OpCode.OpCodeName == OpCodeName.Tj || @operator.OpCode.OpCodeName == OpCodeName.TJ)
			{
				foreach (var element in @operator.Operands)
				{
					ExtractText(element, pdfContent, pageIndex);
				}
			}
		}
		
		private static void ExtractText(CSequence sequence, IList<TextContent> pdfContent, int pageIndex)
		{
			foreach (var element in sequence)
			{
				ExtractText(element, pdfContent, pageIndex);
			}
		}

		private static void ExtractText(CString @string, IList<TextContent> pdfContent, int pageIndex)
		{
			var part = pdfContent.Last();
			if (part.Value == null)
			{
				part.Value = @string.Value;
			}
			else
			{
				part.Value += @string.Value;
			}
		}

		private static void ExtractText(CComment comment, IList<TextContent> pdfContent, int pageIndex) { /* nothing */ }
		private static void ExtractText(CInteger integer, IList<TextContent> pdfContent, int pageIndex) { /* nothing */ }
		private static void ExtractText(CName name, IList<TextContent> pdfContent, int pageIndex) { /* nothing */ }
		private static void ExtractText(CNumber number, IList<TextContent> pdfContent, int pageIndex) { /* nothing */ }
		private static void ExtractText(CReal real, IList<TextContent> pdfContent, int pageIndex) { /* nothing */ }
	}
}