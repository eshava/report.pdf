namespace Eshava.Report.Pdf.Core.Models
{
	public class Font
	{
		public string Fontfamily { get; set; }
		public double Size { get; set; }
		public bool Bold { get; set; }
		public bool Italic { get; set; }
		public bool Underline { get; set; }
		public bool Strikeout { get; set; }
		public string Color { get; set; }

		public override int GetHashCode()
		{
			unchecked // Allow arithmetic overflow, numbers will just "wrap around"
			{
				var hashcode = 1572849;
				hashcode *= 4956127 ^ Fontfamily.GetHashCode();
				hashcode *= 4956127 ^ Size.GetHashCode();
				hashcode *= 4956127 ^ (Bold ? 11 : 1).GetHashCode();
				hashcode *= 4956127 ^ (Italic ? 22 : 2).GetHashCode();
				hashcode *= 4956127 ^ (Underline ? 33 : 3).GetHashCode();
				hashcode *= 4956127 ^ (Strikeout ? 44 : 4).GetHashCode();
				hashcode *= 4956127 ^ Color.GetHashCode();

				return hashcode;
			}
		}
	}
}