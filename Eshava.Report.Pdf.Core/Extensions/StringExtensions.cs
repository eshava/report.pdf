using System;

namespace Eshava.Report.Pdf.Core.Extensions
{
	public static class StringExtensions
	{
		public static bool IsNullOrEmpty(this string text)
		{
			return String.IsNullOrEmpty(text);
		}
	}
}