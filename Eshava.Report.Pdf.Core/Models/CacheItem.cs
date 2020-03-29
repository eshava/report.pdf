using System.Collections.Generic;

namespace Eshava.Report.Pdf.Core.Models
{
	public class CacheItem<T>
	{
		public CacheItem()
		{
			Images = new Dictionary<string, T>();
			Stationaries = new Dictionary<string, byte[]>();
			PrependPdfs = new List<(int SequenceNumber, byte[] Pdf)>();
			AppendPdfs = new List<(int SequenceNumber, byte[] Pdf)>();
		}

		public Dictionary<string, T> Images { get; set; }
		public Dictionary<string, byte[]> Stationaries { get; set; }
		public IEnumerable<(int SequenceNumber, byte[] Pdf)> PrependPdfs { get; set; }
		public IEnumerable<(int SequenceNumber, byte[] Pdf)> AppendPdfs { get; set; }
	}
}