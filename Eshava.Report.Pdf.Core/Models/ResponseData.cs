using System;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ResponseData<T>
	{
		public bool IsFaulty { get; set; }
		public T Data { get; set; }
		public Exception Error { get; set; }
	}
}