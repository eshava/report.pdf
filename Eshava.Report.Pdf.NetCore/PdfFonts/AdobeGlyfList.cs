using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Eshava.Report.Pdf.PdfFonts
{
	/// <summary>
	/// Origin of the code
	/// https://github.com/cleversolutions/pdfSharpTextExtrationTests
	/// </summary>
	public class AdobeGlyfList
	{
		private static AdobeGlyfList _instance = null;
		private Dictionary<string, string> _dictionary;

		public static AdobeGlyfList Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AdobeGlyfList();
				}

				return _instance;
			}
		}

		private AdobeGlyfList()
		{
			Init();
		}

		public string Lookup(string glyph)
		{
			if (glyph.StartsWith(@"/"))
			{
				glyph = glyph.Substring(1);
			}

			_dictionary.TryGetValue(glyph, out var unicode);
			return unicode;
		}

		private void Init()
		{
			_dictionary = new Dictionary<string, string>();
			string line;

			// Read the file and display it line by line.  
			var file = new System.IO.StreamReader(@"glyphlist.txt");
			while ((line = file.ReadLine()) != null)
			{
				if (line.StartsWith("#"))
				{
					continue;
				}

				var match = Regex.Match(line, @"(?<glyph>^.*);((?<unicode>[0-9A-F]{4})\s*)+");
				if (match.Success)
				{
					var glyphName = match.Groups["glyph"].Value;
					var chars = new List<char>();
					
					foreach (Capture capture in match.Groups["unicode"].Captures)
					{
						chars.Add((char)Convert.ToInt16(capture.Value, 16));
					}

					var unicode = String.Concat(chars);
					_dictionary[glyphName] = unicode;
				}
			}

			file.Close();
		}
	}
}