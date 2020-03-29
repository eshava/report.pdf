using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Eshava.Report.Pdf.Core.Models;

namespace Eshava.Report.Pdf.Core
{
	public class XmlReader
	{
		public ResponseData<Models.Report> ReadXmlFromString(string xml)
		{
			try
			{
				var reader = new StringReader(xml);
				var xmlReader = new XmlTextReader(reader);
				
				var serializer = new XmlSerializer(typeof(Models.Report));
				var report = (Models.Report)serializer.Deserialize(xmlReader);
			
				xmlReader.Close();
				reader.Close();

				return new ResponseData<Models.Report>
				{
					Data = report
				};
			}
			catch (Exception ex)
			{
				return new ResponseData<Models.Report>
				{
					Error = ex,
					IsFaulty = true
				};
			}
		}
	}
}