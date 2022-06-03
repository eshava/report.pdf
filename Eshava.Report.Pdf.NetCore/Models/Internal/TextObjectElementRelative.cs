namespace Eshava.Report.Pdf.Models.Internal
{
	/// <summary>
	/// Move text position
	/// <see cref="OpCodeName.Td"/>
	/// Move text position and set leading
	/// <see cref="OpCodeName.TD"/>
	/// </summary>
	internal class TextObjectElementRelative : TextobjectElement
	{
		public double ShiftPosX { get; set; }
		public double ShiftPosY { get; set; }
	}
}