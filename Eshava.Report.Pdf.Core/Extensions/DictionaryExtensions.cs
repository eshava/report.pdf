using System.Collections.Generic;

namespace Eshava.Report.Pdf.Core.Extensions
{
	public static class DictionaryExtensions
	{
		public static T CheckDictionary<K, T>(this Dictionary<K, T> dictionary, K key, T defaultResult = default)
		{
			return dictionary != default
				&& key != null
				&& dictionary.ContainsKey(key) ? dictionary[key] : defaultResult;
		}
	}
}