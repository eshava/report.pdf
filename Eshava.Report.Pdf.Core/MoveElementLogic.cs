using System;
using System.Collections.Generic;
using System.Linq;
using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Interfaces;
using Eshava.Report.Pdf.Core.Models;

namespace Eshava.Report.Pdf.Core
{
	public class MoveElementLogic
	{
		/// <summary>
		/// Checks whether elements need to be enlarged and should move others, as well as removing empty elements and, if necessary, moving elements accordingly
		/// </summary>
		/// <param name="graphics">Graphics element</param>
		/// <param name="container">position</param>
		public void AnalyzeElements(IGraphics graphics, ElementContainer container)
		{
			var endless = true;

			while (endless)
			{
				var elements = container.GetAllElements().OrderBy(e => e.PosY).ThenBy(e => e.PosX).ToList();
				var pointsStart = new Dictionary<Guid, Point>();
				var pointsEnd = new Dictionary<Guid, Point>();
				var sizes = new Dictionary<Guid, Size>();

				elements = elements.OrderBy(e => e.PosY).ThenBy(e => e.PosX).ToList();

				var remove = false;
				var indexCurrent = -1;
				for (var index = 0; index < elements.Count; index++)
				{
					indexCurrent = index;

					// Check if the element is allowed to grow and move other elements
					ShiftElementsDown(graphics, elements, index, pointsStart, pointsEnd, sizes);

					// Check if the element is empty and underlying elements can move up
					remove = ShiftElementsUp(graphics, elements, index, pointsStart, pointsEnd, sizes);

					if (remove)
					{
						break;
					}
				}

				if (remove)
				{
					if (elements[indexCurrent] is ElementText)
					{
						container.RemoveElementText(elements[indexCurrent] as ElementText);
					}
					else if (elements[indexCurrent] is ElementHtml)
					{
						container.RemoveElementHtml(elements[indexCurrent] as ElementHtml);
					}
				}
				else
				{
					endless = false;
				}
			}
		}

		/// <summary>
		/// Checks if the element is empty and underlying elements can move up
		/// </summary>
		/// <param name="graphics">Graphics element</param>
		/// <param name="elements">List of all elements to be considered</param>
		/// <param name="index">Index of the current element of the list</param>
		/// <param name="pointsStart">List of all start coordinates</param>
		/// <param name="pointsEnd">List of all end coordinates</param>
		/// <param name="sizes">List of all element sizes</param>
		/// <returns>Remove element</returns>
		private bool ShiftElementsUp(IGraphics graphics, List<ElementBase> elements, int index, Dictionary<Guid, Point> pointsStart, Dictionary<Guid, Point> pointsEnd, Dictionary<Guid, Size> sizes)
		{
			CalculateElement(graphics, elements[index], pointsStart, pointsEnd, sizes);

			if (!IsShiftUpElement(elements[index]))
			{
				return false;
			}

			var elementCurrentText = elements[index] as ElementText;
			var elementCurrentHtml = elements[index] as ElementHtml;
			var startCurrent = pointsStart[elementCurrentText != default ? elementCurrentText.Id : elementCurrentHtml.Id];
			var endCurrent = pointsEnd[elementCurrentText != default ? elementCurrentText.Id : elementCurrentHtml.Id];

			var calculatedResult = CheckCollisions(graphics, elements, new List<int> { index }, pointsStart, pointsEnd, sizes, startCurrent, endCurrent);

			var borderLeft = 0.0;
			var borderRight = Double.MaxValue;

			foreach (var collision in calculatedResult.Collisions)
			{
				if (collision.Vertical != Collision.None)
				{
					var element = elements[collision.ElementIndex];

					if (!IsShiftUpElement(element))
					{
						var start = pointsStart[element.Id];
						var end = pointsEnd[element.Id];

						if (borderLeft > Math.Round(start.X, 2))
						{
							borderLeft = Math.Round(start.X);
						}

						if (borderRight < Math.Round(end.X))
						{
							borderRight = Math.Round(end.X);
						}
					}
				}
			}

			foreach (var below in calculatedResult.ElementIndexesBelow)
			{
				var ShiftUpHeight = elementCurrentText != default ? elementCurrentText.ShiftUpHeight : elementCurrentHtml.ShiftUpHeight;
				ShiftElementsUp(graphics, elements, index, below, borderLeft, borderRight, ShiftUpHeight, pointsStart, pointsEnd, sizes);
			}

			return true;
		}

		private void ShiftElementsUp(IGraphics graphics, List<ElementBase> elements, int indexCurrent, int indexBelow, double borderLeft, double borderRight, double shiftUpHeight, Dictionary<Guid, Point> pointsStart, Dictionary<Guid, Point> pointsEnd, Dictionary<Guid, Size> sizes)
		{
			var element = elements[indexBelow];
			var start = pointsStart[element.Id];
			var end = pointsEnd[element.Id];

			// Check if the element is in the free area
			if (start.X >= borderLeft && end.X <= borderRight)
			{
				var backupPosY = element.PosY;
				var backupHeight = 0.0;

				// Move element upwards
				element.PosY -= shiftUpHeight;
				element.PosY = Math.Round(element.PosY, 2);

				// For lines and classes derived from them, the height is the Y-coordinate for the end of the element and must therefore also be moved
				var line = element as ElementLine;
				if (line != null && !line.MaxHeight)
				{
					backupHeight = line.Height;
					line.Height -= shiftUpHeight;
					line.Height = Math.Round(line.Height, 2);

					if (line.Height < 0.0)
					{
						line.Height = 0;
					}
				}

				pointsStart[element.Id] = new Point(start.X, Math.Round(start.Y - shiftUpHeight, 2));
				pointsEnd[element.Id] = new Point(end.X, Math.Round(end.Y - shiftUpHeight, 2));

				// Check whether the shift has caused overlaps with other elements
				var collisionItemResult = CheckCollisions(graphics, elements, new List<int> { indexCurrent, indexBelow }, pointsStart, pointsEnd, sizes, pointsStart[element.Id], pointsEnd[element.Id]);
				if (collisionItemResult.Collisions.Any(c => c.Vertical != Collision.None && c.Horizontal != Collision.None && !IsShiftUpElement(elements[c.ElementIndex])))
				{
					// There was at least one overlap with another element, so the shift must be reversed
					element.PosY = backupPosY;

					if (line != null && !line.MaxHeight)
					{
						line.Height = backupHeight;
					}

					pointsStart[element.Id] = start;
					pointsEnd[element.Id] = end;
				}
			}
		}

		/// <summary>
		/// Checks if the element is allowed to grow and move other elements down
		/// </summary>
		/// <param name="graphics">Graphics element</param>
		/// <param name="elements">List of all elements to be considered</param>
		/// <param name="inx">Index of the current element of the list</param>
		/// <param name="pointsStart">List of all start coordinates</param>
		/// <param name="pointsEnd">List of all end coordinates</param>
		/// <param name="sizes">List of all element sizes</param>
		private void ShiftElementsDown(IGraphics graphics, List<ElementBase> elements, int inx, Dictionary<Guid, Point> pointsStart, Dictionary<Guid, Point> pointsEnd, Dictionary<Guid, Size> sizes)
		{


			if ((elements[inx] is ElementText && ((ElementText)elements[inx]).ExpandAndShift) || (elements[inx] is ElementHtml && ((ElementHtml)elements[inx]).ExpandAndShift))
			{
				var eText = elements[inx] as ElementText;
				var eHtml = elements[inx] as ElementHtml;
				CalculateElement(graphics, elements[inx], pointsStart, pointsEnd, sizes);
				var start = pointsStart[eText.Id];
				var end = pointsEnd[eText.Id];

				var heightDifference = eText != default ? eText.HeightDifference : eHtml.HeightDifference;

				// Element may grow and move other elements
				if (heightDifference > 0)
				{
					for (var jnx = inx + 1; jnx < elements.Count; jnx++)
					{
						CalculateElement(graphics, elements[jnx], pointsStart, pointsEnd, sizes);

						// old => o; new => n
						// (nsx < osx und nex > osx) oder (nsx >= osx und nsx <= aex)
						if ((pointsStart[elements[jnx].Id].X < start.X && pointsEnd[elements[jnx].Id].X > start.X) || (pointsStart[elements[jnx].Id].X >= start.X && pointsStart[elements[jnx].Id].X < end.X))
						{
							// Move element below and note new position
							elements[jnx].PosY += heightDifference;

							// Also move the end point of the line 
							if (elements[jnx] is ElementLine)
							{
								elements[jnx].Height += heightDifference;
							}

							pointsStart[elements[jnx].Id] = new Point(elements[jnx].PosX, elements[jnx].PosY);
							pointsEnd[elements[jnx].Id] = new Point(pointsEnd[elements[jnx].Id].X, elements[jnx].PosY + sizes[elements[jnx].Id].Height);

							// Starts the element before the increasing element, remember new StartX
							if (start.X > pointsStart[elements[jnx].Id].X)
							{
								start = new Point(pointsStart[elements[jnx].Id].X, start.Y);
							}

							// If the element is wider to the right than the enlarging element, remember new EndX
							if (end.X < pointsEnd[elements[jnx].Id].X)
							{
								end = new Point(pointsEnd[elements[jnx].Id].X, end.Y);
							}
						}
					}
				}
			}
		}

		private void CalculateElement(IGraphics graphics, ElementBase element, Dictionary<Guid, Point> pointsStart, Dictionary<Guid, Point> pointsEnd, Dictionary<Guid, Size> sizes)
		{
			if (!pointsStart.ContainsKey(element.Id))
			{
				var size = element.GetSize(graphics);
				sizes.Add(element.Id, size);

				if (element is ElementLine)
				{
					// For lines and classes derived from them, the height is the Y-coordinate for the end of the element
					pointsStart.Add(element.Id, new Point(element.PosX, element.PosY));
					pointsEnd.Add(element.Id, new Point(element.Width, element.Height));
				}
				else
				{
					pointsStart.Add(element.Id, new Point(element.PosX, element.PosY));
					pointsEnd.Add(element.Id, new Point(element.PosX + size.Width, element.PosY + size.Height));
				}
			}
		}

		private bool IsShiftUpElement(ElementBase element)
		{
			if (!element.IsEmpty)
			{
				return false;
			}

			if (element is ElementText)
			{
				return Math.Round(((ElementText)element).ShiftUpHeight, 2) > 0.0;
			}

			if (element is ElementHtml)
			{
				return Math.Round(((ElementHtml)element).ShiftUpHeight, 2) > 0.0;
			}

			return false;
		}

		private CollisionResult CheckCollisions(IGraphics graphics, List<ElementBase> elements, List<int> indexes, Dictionary<Guid, Point> pointsStart, Dictionary<Guid, Point> pointsEnd, Dictionary<Guid, Size> sizes, Point startCurrent, Point endCurrent)
		{
			var collisionList = new List<CollisionItem>();
			var belowList = new List<int>();

			for (var elementIndex = 0; elementIndex < elements.Count; elementIndex++)
			{
				if (!indexes.Contains(elementIndex))
				{
					var element = elements[elementIndex];

					CalculateElement(graphics, element, pointsStart, pointsEnd, sizes);

					var start = pointsStart[element.Id];
					var end = pointsEnd[element.Id];

					var collisionHorizontal = DetectCollision(startCurrent.X, endCurrent.X, start.X, end.X);
					var collisionVertical = DetectCollision(startCurrent.Y, endCurrent.Y, start.Y, end.Y);

					// All elements that have an overlap with the current element either horizontally or vertically
					if (collisionHorizontal != Collision.None || collisionVertical != Collision.None)
					{
						collisionList.Add(new CollisionItem
						{
							Horizontal = collisionHorizontal,
							Vertical = collisionVertical,
							ElementIndex = elementIndex
						});
					}

					if (collisionVertical == Collision.None && endCurrent.Y < start.Y)
					{
						belowList.Add(elementIndex);
					}
				}
			}

			return new CollisionResult
			{
				Collisions = collisionList,
				ElementIndexesBelow = belowList
			};
		}

		private Collision DetectCollision(double borderAMin, double borderAMax, double borderBMin, double borderBMax)
		{
			borderAMin = Math.Round(borderAMin, 2);
			borderAMax = Math.Round(borderAMax, 2);
			borderBMin = Math.Round(borderBMin, 2);
			borderBMax = Math.Round(borderBMax, 2);

			if (borderAMin <= borderBMin && borderAMax >= borderBMax)
			{
				return Collision.Within;
			}

			var leftOrTop = borderAMin <= borderBMax && borderAMin >= borderBMin;
			var rightOrBottom = borderAMax >= borderBMin && borderAMax <= borderBMax;

			if (leftOrTop && rightOrBottom)
			{
				return Collision.LeftToRightOrTopToBottom;
			}

			if (leftOrTop)
			{
				return Collision.LeftOrTop;
			}

			if (rightOrBottom)
			{
				return Collision.RightOrBottom;
			}

			return Collision.None;
		}

		private class CollisionItem
		{
			public Collision Horizontal { get; set; }
			public Collision Vertical { get; set; }
			public int ElementIndex { get; set; }
		}

		private class CollisionResult
		{
			public IEnumerable<CollisionItem> Collisions { get; set; }
			public IEnumerable<int> ElementIndexesBelow { get; set; }
		}
	}
}