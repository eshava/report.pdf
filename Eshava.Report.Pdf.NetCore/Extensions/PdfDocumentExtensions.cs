using System;
using System.Collections.Generic;
using System.Linq;
using Eshava.Report.Pdf.Models;
using Eshava.Report.Pdf.PdfFonts;
using PdfSharpCore.Pdf.Content.Objects;

namespace Eshava.Report.Pdf.Extensions
{
	/// <summary>
	/// Origin of the code
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

				var fonts = page.ParseFonts();

				ExtractText(content, pdfContent, pageIndex, fonts);
			}

			pdfContent = pdfContent
				.OrderBy(c => c.PageIndex)
				.ThenByDescending(c => c.PosY)
				.ThenBy(c => c.PosX)
				.ToList();

			return pdfContent;
		}

		private static void ExtractText(CObject @object, IList<TextContent> pdfContent, int pageIndex, Dictionary<string, FontResource> fonts)
		{
			if (@object is CArray array)
			{
				ExtractText(array, pdfContent, pageIndex, fonts);
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
				ExtractText(@operator, pdfContent, pageIndex, fonts);
			}
			else if (@object is CReal real)
			{
				ExtractText(real, pdfContent, pageIndex);
			}
			else if (@object is CSequence sequence)
			{
				ExtractText(sequence, pdfContent, pageIndex, fonts);
			}
			else if (@object is CString @string)
			{
				ExtractText(@string, pdfContent, pageIndex, fonts);
			}
			else
			{
				throw new NotImplementedException(@object.GetType().AssemblyQualifiedName);
			}
		}

		private static void ExtractText(CArray array, IList<TextContent> pdfContent, int pageIndex, Dictionary<string, FontResource> fonts)
		{
			foreach (var element in array)
			{
				ExtractText(element, pdfContent, pageIndex, fonts);
			}
		}

		private static void ExtractText(COperator @operator, IList<TextContent> pdfContent, int pageIndex, Dictionary<string, FontResource> fonts)
		{
			if (@operator.OpCode.OpCodeName == OpCodeName.Tf)
			{
				var lastContent = pdfContent.LastOrDefault();
				if (lastContent == null || !String.IsNullOrEmpty(lastContent.Font))
				{
					pdfContent.Add(new TextContent
					{
						PageIndex = pageIndex,
						Font = @operator.Operands.OfType<CName>().FirstOrDefault()?.Name,
					});
				}
				else
				{
					lastContent.Font = @operator.Operands.OfType<CName>().FirstOrDefault()?.Name;
				}
			}

			if (@operator.OpCode.OpCodeName == OpCodeName.Tm)
			{
				SetPosition(@operator, pdfContent, 4, 5);
			}
			else if (@operator.OpCode.OpCodeName == OpCodeName.Td)
			{
				SetPosition(@operator, pdfContent, 0, 1);
			}

			if (@operator.OpCode.OpCodeName == OpCodeName.Tj || @operator.OpCode.OpCodeName == OpCodeName.TJ)
			{
				foreach (var element in @operator.Operands)
				{
					ExtractText(element, pdfContent, pageIndex, fonts);
				}
			}
		}

		private static void SetPosition(COperator @operator, IList<TextContent> pdfContent, int indexPosX, int indexPosY)
		{
			var lastContent = pdfContent.LastOrDefault();
			var posX = GetPosition(@operator, indexPosX);
			var posY = GetPosition(@operator, indexPosY);

			if (HasPositionValues(lastContent.PosX, lastContent.PosY))
			{
				pdfContent.Add(new TextContent
				{
					PageIndex = lastContent.PageIndex,
					Font = lastContent.Font,
					PosX = posX,
					PosY = posY
				});
			}
			else
			{
				lastContent.PosX = posX;
				lastContent.PosY = posY;
			}
		}

		private static bool HasPositionValues(double posX, double posY)
		{
			return Math.Round(posX, 4) != Math.Round(0.0, 4) || Math.Round(posY, 4) != Math.Round(0.0, 4);
		}

		private static double GetPosition(COperator @operator, int index)
		{
			return @operator.Operands[index] is CInteger
				? ((CInteger)@operator.Operands[index]).Value
				: ((CReal)@operator.Operands[index]).Value
				;
		}

		private static void ExtractText(CSequence sequence, IList<TextContent> pdfContent, int pageIndex, Dictionary<string, FontResource> fonts)
		{
			foreach (var element in sequence)
			{
				ExtractText(element, pdfContent, pageIndex, fonts);
			}
		}

		private static void ExtractText(CString @string, IList<TextContent> pdfContent, int pageIndex, Dictionary<string, FontResource> fonts)
		{
			var part = pdfContent.Last();

			var text = @string.Value;
			if (!String.IsNullOrEmpty(part.Font) && fonts.ContainsKey(part.Font))
			{
				var font = fonts[part.Font];
				text = font.Encode(text);
			}

			if (part.Value == null)
			{
				part.Value = text;
			}
			else
			{
				part.Value += text;
			}
		}

		private static void ExtractText(CComment comment, IList<TextContent> pdfContent, int pageIndex) { /* nothing */ }
		private static void ExtractText(CInteger integer, IList<TextContent> pdfContent, int pageIndex) { /* nothing */ }
		private static void ExtractText(CName name, IList<TextContent> pdfContent, int pageIndex) { /* nothing */ }
		private static void ExtractText(CNumber number, IList<TextContent> pdfContent, int pageIndex) { /* nothing */ }
		private static void ExtractText(CReal real, IList<TextContent> pdfContent, int pageIndex) { /* nothing */ }
	}
}