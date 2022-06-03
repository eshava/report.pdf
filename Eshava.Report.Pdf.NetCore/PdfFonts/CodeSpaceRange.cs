using System.Collections.Generic;

namespace Eshava.Report.Pdf.PdfFonts
{
	/// <summary>
	/// Origin of the code
	/// https://github.com/cleversolutions/pdfSharpTextExtrationTests
	/// </summary>
	public class CodeSpaceRange
	{
		public CodeSpaceRange()
		{
			Mapping = new Dictionary<int, Map>();
		}

		public int Low { get; set; }
		public int High { get; set; }
		public int NumberOfBytes { get; set; }
		public Dictionary<int, Map> Mapping { get; set; }

		public void AddMap(int cid, string unicode, int sourceByteLength)
		{
			Mapping[cid] = new Map
			{
				UnicodeValue = unicode,
				SourceByteLength = sourceByteLength
			};
		}
	}
}
