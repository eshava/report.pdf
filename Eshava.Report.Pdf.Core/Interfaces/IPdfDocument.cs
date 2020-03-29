using System;

namespace Eshava.Report.Pdf.Core.Interfaces
{
	public interface IPdfDocument<P> where P : IPdfPage
	{
		string InternalId { get; }
		string Author { get; set; }
		string CreatedBy { get; set; }
		string Title { get; set; }
		DateTime CreatedAt { get; set; }

		P AddPage();
		void RemovePage(P page);
	}
}