using System.Collections.Generic;
using Eshava.Report.Pdf.PdfFonts;

namespace Eshava.Report.Pdf.Extensions
{
	/// <summary>
	/// Origin of the code
	/// https://github.com/cleversolutions/pdfSharpTextExtrationTests
	/// </summary>
	public static class PdfPageExtensions
	{
		public static Dictionary<string, FontResource> ParseFonts(this PdfSharpCore.Pdf.PdfPage page)
		{
			var fonts = new Dictionary<string, FontResource>();

			var fontResource = page.Resources.Elements.GetDictionary("/Font")?.Elements;
			if (fontResource == null)
			{
				return fonts;
			}

			//All that above isn't going to do, but it's close...
			foreach (var fontName in fontResource.Keys)
			{
				var resource = fontResource[fontName] as PdfSharpCore.Pdf.Advanced.PdfReference;
				var font = new FontResource(fontName, resource);

				fonts[fontName] = font;
			}

			return fonts;
		}
	}
}