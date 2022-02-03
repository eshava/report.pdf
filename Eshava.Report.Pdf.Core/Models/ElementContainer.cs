using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Eshava.Report.Pdf.Core.Constants;
using Eshava.Report.Pdf.Core.Interfaces;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ElementContainer
	{
		private readonly ElementCollection _collection;

		public ElementContainer()
		{
			_collection = new ElementCollection();
		}

		[XmlElement(ElementTypes.IMAGE)]
		public List<ElementImage> ContentImage
		{
			get { return _collection.GetElements<ElementImage>(); }
		}

		[XmlElement(ElementTypes.TEXT)]
		public List<ElementText> ContentText
		{
			get { return _collection.GetElements<ElementText>(); }
		}

		[XmlElement(ElementTypes.HTML)]
		public List<ElementHtml> ContentHtml
		{
			get { return _collection.GetElements<ElementHtml>(); }
		}

		[XmlElement(ElementTypes.LINE)]
		public List<ElementLine> ContentLine
		{
			get { return _collection.GetElements<ElementLine>(); }
		}

		[XmlElement(ElementTypes.RECTANGLE)]
		public List<ElementRectangle> ContentRectangle
		{
			get { return _collection.GetElements<ElementRectangle>(); }
		}

		[XmlElement(ElementTypes.RECTANGLEFILL)]
		public List<ElementRectangleFill> ContentRectangleFill
		{
			get { return _collection.GetElements<ElementRectangleFill>(); }
		}

		[XmlElement(ElementTypes.PAGENO)]
		public List<ElementPageNo> ContentPageNo
		{
			get { return _collection.GetElements<ElementPageNo>(); }
		}

		public List<ElementBase> GetAllElements()
		{
			return GetAllStandardElements(true);
		}

		/// <summary>
		/// Determines the total size of the container
		/// </summary>
		/// <param name="graphics">Graphics element</param>
		/// <returns>Total size of this container</returns>
		public virtual Size GetSize(IGraphics graphics)
		{
			var elements = GetAllStandardElements(false);
			var size = CalculateSize(elements, graphics);
			var lines = CalculateSizeLine(_collection.GetElements<ElementLine>(), graphics);

			return new Size(Math.Max(size.Width, lines.Width), Math.Max(size.Height, lines.Height));
		}

		private List<ElementBase> GetAllStandardElements(bool includeNonStandard)
		{
			var elements = new List<ElementBase>();

			if (includeNonStandard)
			{
				elements.AddRange(_collection.GetElements<ElementLine>().Select(e => (ElementBase)e));
			}

			elements.AddRange(_collection.GetElements<ElementRectangleFill>().Select(e => (ElementBase)e));
			elements.AddRange(_collection.GetElements<ElementImage>().Select(e => (ElementBase)e));
			elements.AddRange(_collection.GetElements<ElementText>().Select(e => (ElementBase)e));
			elements.AddRange(_collection.GetElements<ElementHtml>().Select(e => (ElementBase)e));
			elements.AddRange(_collection.GetElements<ElementRectangle>().Select(e => (ElementBase)e));
			elements.AddRange(_collection.GetElements<ElementPageNo>().Select(e => (ElementBase)e));

			elements.Sort((ElementBase a, ElementBase b) =>
			{
				if (a.PosY < b.PosY)
				{
					return -1;
				}

				if (a.PosY > b.PosY)
				{
					return 1;
				}

				if (a.PosX == b.PosX)
				{
					return 0;
				}

				return a.PosX > b.PosX ? 1 : -1;
			});

			return elements;
		}

		public void RemoveElementText(ElementText element)
		{
			_collection.RemoveElement(element);
		}

		public void AddElementHtml(ElementHtml element)
		{
			_collection.AddElement(element);
		}

		public void RemoveElementHtml(ElementHtml element)
		{
			_collection.RemoveElement(element);
		}

		/// <summary>
		/// Determines the total size of the container using the elements passed to it
		/// </summary>
		/// <param name="elements">elements</param>
		/// <param name="graphics">Graphics element</param>
		/// <returns>Gesamtgrößes des Containers</returns>
		private Size CalculateSize(IEnumerable<ElementBase> elements, IGraphics graphics)
		{
			double width = 0;
			double height = 0;

			foreach (var element in elements)
			{
				var elementPosition = element.GetPosition();
				var elementSize = element.GetSize(graphics);
				if (width < elementPosition.X + elementSize.Width)
				{
					width = elementPosition.X + elementSize.Width;
				}

				if (height < elementPosition.Y + elementSize.Height && elementSize.Height > 0)
				{
					height = elementPosition.Y + elementSize.Height;
				}
			}

			return new Size(width, height);
		}

		private Size CalculateSizeLine(IEnumerable<ElementLine> elements, IGraphics graphics)
		{
			double width = 0;
			double height = 0;

			foreach (var element in elements)
			{
				var elementPos = element.GetPosition();
				var elementSize = element.GetSize(graphics);

				var maximum = Math.Max(elementPos.X, elementSize.Width);
				if (width < maximum)
				{
					width = maximum;
				}

				maximum = Math.Max(elementPos.Y, elementSize.Height);
				if (height < maximum)
				{
					height = maximum;
				}
			}

			return new Size(width, height);
		}
	}
}