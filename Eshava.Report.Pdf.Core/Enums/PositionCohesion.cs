namespace Eshava.Report.Pdf.Core.Enums
{
	public enum PositionCohesion
	{
		/// <summary>
		/// The content of an item is split only if the item does not fit in its entirety on the current page or on a new page.
		/// </summary>
		KeepTogether = 0,
		/// <summary>
		/// The content of a position is always split if it does not fit on the current page as a whole.
		/// </summary>
		SplittAlways = 1,
		/// <summary>
		/// The content of a position is split if the available area of the current page is at least equal to the percentage
		/// </summary>
		SplittByPercent = 2
	}
}