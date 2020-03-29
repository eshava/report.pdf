using System.Globalization;
using System.Xml.Serialization;
using Eshava.Report.Pdf.Core.Extensions;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ElementPageNo : ElementText
	{
        private string _backup;

		[XmlAttribute]
		public bool SuppressOnSinglePage { get; set; }

		public void SetNumber(int current, int total)
		{
			if (!Content.IsNullOrEmpty())
			{
				_backup = Content;

				if (SuppressOnSinglePage && total == 1)
				{
					Content = "";
				}
				else
				{
					Content = Content.Replace("{current}", current.ToString(CultureInfo.InvariantCulture));
					Content = Content.Replace("{total}", total.ToString(CultureInfo.InvariantCulture));
				}
			}
		}

		public void ResetNumber()
		{
			Content = _backup;
		}
	}
}