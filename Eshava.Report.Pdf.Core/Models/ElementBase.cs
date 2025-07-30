using System;
using System.Xml.Serialization;
using Eshava.Report.Pdf.Core.Extensions;
using Eshava.Report.Pdf.Core.Interfaces;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ElementBase
	{
		private static readonly int _hashCode = Guid.Parse("e02a8e8e-6835-4b7f-9a3b-2f8626ce31cb").GetHashCode();

		public ElementBase()
		{
			Id = Guid.NewGuid();
		}

		[XmlIgnore]
		public Guid Id { get; }

		[XmlAttribute]
		public double Width { get; set; }

		[XmlAttribute]
		public double Height { get; set; }

		[XmlAttribute]
		public double PosX { get; set; }

		[XmlAttribute]
		public double PosY { get; set; }

		[XmlAttribute]
		public bool ConsiderAsCollidedForShift { get; set; }

		[XmlText]
		public string Content { get; set; }

		[XmlIgnore]
		public virtual bool IsEmpty => Content.IsNullOrEmpty();

		public virtual Size GetSize(IGraphics graphics)
		{
			return new Size(Width, Height);
		}

		public Point GetPosition()
		{
			return new Point(PosX, PosY);
		}

		public virtual void Draw(IGraphics graphics, Point topLeftPage, Size sizePage)
		{
			throw new NotImplementedException();
		}

		public override bool Equals(object obj)
		{
			var baseElement = obj as ElementBase;
			if (baseElement == null)
			{
				return false;
			}

			return Equals(Id, baseElement.Id);
		}

		public override int GetHashCode()
		{
			return _hashCode;
		}
	}
}