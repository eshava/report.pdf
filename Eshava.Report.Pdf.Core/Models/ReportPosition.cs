using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Extensions;
using Eshava.Report.Pdf.Core.Interfaces;
using Eshava.Report.Pdf.Interfaces;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ReportPosition : ElementContainer
	{
		public ReportPosition()
		{
			Type = PositonType.Default;
		}

		[XmlAttribute]
		public PositonType Type { get; set; }

		[XmlAttribute]
		public int SequenceNo { get; set; }

		[XmlAttribute]
		public bool PreventLastOnPage { get; set; }

		public bool IsEmpty
		{
			get { return GetAllElements().Count == 0; }
		}

		/// <summary>
		/// Distributes the elements of the current position to several positions so that they fit into the page frame. 
		/// The method should only be used if a new page has been started and the position still does not fit into the position range.
		/// </summary>
		/// <param name="graphics">Graphics element</param>
		/// <param name="maxElementHeight">Height of the entire position range minus the height again the positions to be repeated</param>
		/// <param name="maxElementHeightOnPage">Remaining height of the current page </param>
		/// <returns></returns>
		public List<ReportPosition> SplitPosition(IGraphics graphics, double maxElementHeight, double maxElementHeightOnPage)
		{
			return SplitPositionIntern(graphics, maxElementHeight, maxElementHeightOnPage, false);
		}

		/// <summary>
		/// Distributes the elements of the current position to several positions so that they fit into the page frame.
		/// The distribution of the elements is calculated backwards (Note: OnlyOnTop elements are ignored).
		/// </summary>
		/// <param name="graphics">Graphics element</param>
		/// <param name="maxElementHeight">Height of the entire position range minus the height again the positions to be repeated</param>
		/// <param name="maxElementHeightOnPage">Remaining height of the current page </param>
		/// <returns></returns>
		public List<ReportPosition> SplitPositionInvert(IGraphics graphics, double maxElementHeight, double maxElementHeightOnPage)
		{
			return SplitPositionIntern(graphics, maxElementHeight, maxElementHeightOnPage, true);
		}

		private List<ReportPosition> SplitPositionIntern(IGraphics graphics, double maxElementHeight, double maxElementHeightOnPage, bool invertAnalyse)
		{
			var positions = new List<ReportPosition>();
			var noMatch = new ElementContainer();
			var noMatch2nd = new ElementContainer();
			var position = new ReportPosition();
			double currentHeight = 0;

			// Check which elements still fit on the current page
			CheckContentImage(graphics, position, this, noMatch, ref currentHeight, maxElementHeight, maxElementHeightOnPage);
			CheckContentRectangle(graphics, position, this, noMatch, ref currentHeight, maxElementHeight, maxElementHeightOnPage);
			AddContentLine(this, noMatch);

			var notMatchedLineVertical = noMatch.ContentLine.Where(l => l.PosX == l.Width).ToList();
			var notMatchedLineHorizontal = noMatch.ContentLine.Where(l => l.PosX != l.Width).ToList();
			var extraHeight = notMatchedLineHorizontal.Any() ? notMatchedLineHorizontal.Max(line => line.SplittExtraMargin) : 0.0;

			if (invertAnalyse)
			{
				CheckContentTextInvert(graphics, position, this, noMatch, ref currentHeight, maxElementHeightOnPage - extraHeight);
			}
			else
			{
				CheckContentText(graphics, position, this, noMatch, ref currentHeight, maxElementHeightOnPage - extraHeight);
			}
			// PageNo elements are ignored in positions
			positions.Add(position);

			// Move only lines that are below all text elements
			var horizontalLineToRemove = new List<ElementLine>();
			foreach (var elementLine in notMatchedLineHorizontal)
			{
				var count = position.ContentText.Count(e => e.PosY > elementLine.PosY)
					+ position.ContentHtml.Count(e => e.PosY > elementLine.PosY);

				if (count > 0)
				{
					horizontalLineToRemove.Add(elementLine);
					position.ContentLine.Add(elementLine);
				}
			}
			horizontalLineToRemove.ForEach(e => notMatchedLineHorizontal.Remove(e));

			currentHeight = 0;
			// Create position for next page
			position = new ReportPosition();
			// Current page height corresponds to the complete page height
			CheckContentImage(graphics, position, noMatch, null, ref currentHeight, maxElementHeight, maxElementHeight);
			CheckContentRectangle(graphics, position, noMatch, null, ref currentHeight, maxElementHeight, maxElementHeightOnPage);

			if (invertAnalyse)
			{
				CheckContentTextInvert(graphics, position, noMatch, noMatch2nd, ref currentHeight, maxElementHeight - extraHeight);
			}
			else
			{
				CheckContentText(graphics, position, noMatch, noMatch2nd, ref currentHeight, maxElementHeight - extraHeight);
			}

			positions.Add(position);

			// If there are still elements left after the new page, create positions until there are no more elements
			while (noMatch2nd.ContentText.Count > 0 || noMatch2nd.ContentHtml.Count > 0)
			{
				currentHeight = 0;
				noMatch = noMatch2nd;
				noMatch2nd = new ElementContainer();
				position = new ReportPosition();
				if (invertAnalyse)
				{
					CheckContentTextInvert(graphics, position, noMatch, noMatch2nd, ref currentHeight, maxElementHeight - extraHeight);
				}
				else
				{
					CheckContentText(graphics, position, noMatch, noMatch2nd, ref currentHeight, maxElementHeight - extraHeight);
				}

				positions.Add(position);
			}

			// If the lines do not fit on the current page, 
			// it is assumed that they were moved to the end of the position by a dynamic text field

			// Calculate new height for the vertical lines
			notMatchedLineHorizontal.ForEach(line => line.Height = line.PosY = 0.0);
			notMatchedLineVertical.ForEach(line => line.Height = 0.0);

			positions.ForEach(pos =>
			{
				var posSize = pos.GetSize(graphics);
				notMatchedLineHorizontal.ForEach(line =>
				{
					var newLine = line.Clone();
					newLine.Height = posSize.Height + extraHeight;
					newLine.PosY = posSize.Height + extraHeight;

					pos.ContentLine.Add(newLine);
				});

				notMatchedLineVertical.ForEach(line =>
				{
					var newLine = line.Clone();
					newLine.Height = posSize.Height + extraHeight;
					pos.ContentLine.Add(newLine);
				});
			});

			return positions;
		}


		/// <summary>
		/// Determines whether any images still fit on the current page or have to fit on the next page
		/// </summary>
		/// <param name="graphics">Graphics element</param>
		/// <param name="position">Position for the current page (remaining height)</param>
		/// <param name="current"></param>
		/// <param name="noMatch">Elements for the next page</param>
		/// <param name="currentHeight">Current height of the position</param>
		/// <param name="maxElementHeight">Height of the entire position range minus the height again the positions to be repeated</param>
		/// <param name="maxElementHeightOnPage">Remaining height of the current page </param>
		private void CheckContentImage(IGraphics graphics, ReportPosition position, ElementContainer current, ElementContainer noMatch, ref double currentHeight, double maxElementHeight, double maxElementHeightOnPage)
		{
			if (current.ContentImage.Count == 0)
			{
				return;
			}

			foreach (var image in current.ContentImage)
			{
				var size = image.GetSize(graphics);
				// Image does not fit on the current page
				if (maxElementHeightOnPage < size.Height + image.PosY && noMatch != null)
				{
					// Image does not fit on a new page, so it is scaled to fit on one page
					if (maxElementHeight < size.Height + image.PosY)
					{
						image.Width *= (image.Height / maxElementHeight);
						image.Height = maxElementHeight;
					}
					noMatch.ContentImage.Add(image);
				}
				else
				{
					// Set current maximum height
					if (currentHeight < size.Height + image.PosY)
					{
						currentHeight += size.Height + image.PosY;
					}

					position.ContentImage.Add(image);
				}
			}
		}

		/// <summary>
		/// All lines are displayed on the next page and scaled to the container height
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		private void AddContentLine(ElementContainer source, ElementContainer target)
		{
			foreach (var line in source.ContentLine)
			{
				target.ContentLine.Add(line);
			}
		}

		private void CheckContentRectangle(IGraphics graphics, ReportPosition position, ElementContainer current, ElementContainer noMatch, ref double currentHeight, double maxElementHeight, double maxElementHeightOnPage)
		{
			if (current.ContentImage.Count > 0)
			{
				foreach (var rectangle in current.ContentRectangle)
				{
					if (CheckSingleContentRectangle(rectangle, graphics, ref currentHeight, maxElementHeight, maxElementHeightOnPage))
					{
						position.ContentRectangle.Add(rectangle);
					}
					else
					{
						noMatch.ContentRectangle.Add(rectangle);
					}
				}

				foreach (var rectangle in current.ContentRectangleFill)
				{
					if (CheckSingleContentRectangle(rectangle, graphics, ref currentHeight, maxElementHeight, maxElementHeightOnPage))
					{
						position.ContentRectangleFill.Add(rectangle);
					}
					else
					{
						noMatch.ContentRectangleFill.Add(rectangle);
					}
				}
			}
		}

		private bool CheckSingleContentRectangle(ElementBase rectangle, IGraphics graphics, ref double currentHeight, double maxElementHeight, double maxElementHeightOnPage)
		{
			var size = rectangle.GetSize(graphics);
			// Rectangle does not fit on the current page
			if (maxElementHeightOnPage < size.Height + rectangle.PosY)
			{
				// Rectangle does not fit on a new page, so it is scaled to fit on one page
				if (maxElementHeight < size.Height + rectangle.PosY)
				{
					rectangle.Width *= (rectangle.Height / maxElementHeight);
					rectangle.Height = maxElementHeight;
				}
				return false;
			}

			// Set current maximum height
			if (currentHeight < size.Height + rectangle.PosY)
			{
				currentHeight += size.Height + rectangle.PosY;
			}

			return true;
		}

		private void CheckContentTextInvert(IGraphics graphics, ReportPosition position, ElementContainer current, ElementContainer noMatch, ref double currentHeight, double maxElementHeightOnPage)
		{
			if (current.ContentText.Count > 0)
			{
				foreach (var text in current.ContentText)
				{
					AddElementTextToList(graphics, text, position.ContentText, noMatch.ContentText, ref currentHeight, maxElementHeightOnPage, true);
				}
			}

			if (current.ContentHtml.Count > 0)
			{
				foreach (var text in current.ContentHtml)
				{
					AddElementHtmlToList(graphics, text, position.ContentHtml, noMatch.ContentHtml, ref currentHeight, maxElementHeightOnPage, true);
				}
			}
		}

		private void CheckContentText(IGraphics graphics, ReportPosition position, ElementContainer current, ElementContainer noMatch, ref double currentHeight, double maxElementHeightOnPage)
		{
			if (current.ContentText.Count > 0)
			{
				foreach (var text in current.ContentText)
				{
					AddElementTextToList(graphics, text, position.ContentText, noMatch.ContentText, ref currentHeight, maxElementHeightOnPage, false);
				}
			}

			if (current.ContentHtml.Count > 0)
			{
				foreach (var text in current.ContentHtml)
				{
					AddElementHtmlToList(graphics, text, position.ContentHtml, noMatch.ContentHtml, ref currentHeight, maxElementHeightOnPage, false);
				}
			}
		}

		private void AddElementTextToList<T>(IGraphics graphics, T text, List<T> elementList, List<T> noMatchList, ref double currentHeight, double maxElementHeightOnPage, bool invertAnalyse) where T : ElementText, new()
		{
			var size = text.GetSize(graphics);
			// Text does not fit completely on the current page
			if (maxElementHeightOnPage < size.Height + text.PosY)
			{
				// Split text until it fits on the rest of the current page
				var textparts = text.SplittByNewLine();
				var eText = CheckTextparts(graphics, textparts, text, elementList, ref currentHeight, maxElementHeightOnPage, invertAnalyse);
				if (eText == null)
				{
					// Nothing could be split
					textparts = text.SplittOnEndOfSentences();
					eText = CheckTextparts(graphics, textparts, text, elementList, ref currentHeight, maxElementHeightOnPage, invertAnalyse);

					// IF eText == null -> Add complete text to the list of elements for the next page 
					// ELSE -> otherwise add remaining text to the list of elements for the next page
					noMatchList.Add(eText ?? text);
				}
				else
				{
					// Add remaining text to the list of elements for the next page
					noMatchList.Add(eText);
				}
			}
			else
			{
				// Set current maximum height
				if (currentHeight < size.Height + text.PosY)
				{
					currentHeight = size.Height + text.PosY;
				}

				elementList.Add(text);
			}
		}

		private void AddElementHtmlToList<T>(IGraphics graphics, T text, List<T> elementList, List<T> noMatchList, ref double currentHeight, double maxElementHeightOnPage, bool invertAnalyse) where T : ElementHtml, new()
		{
			var size = text.GetSize(graphics);
			// Text does not fit completely on the current page
			if (maxElementHeightOnPage < size.Height + text.PosY)
			{
				// Split text until it fits on the rest of the current page
				var textparts = text.TextSegments.ToList();
				var eText = CheckTextparts(graphics, textparts, text, elementList, ref currentHeight, maxElementHeightOnPage, invertAnalyse);
				if (eText == null)
				{
					// Nothing could be split
					textparts = text.SplittOnEndOfSentences();
					eText = CheckTextparts(graphics, textparts, text, elementList, ref currentHeight, maxElementHeightOnPage, invertAnalyse);

					// IF eText == null -> Add complete text to the list of elements for the next page 
					// ELSE -> otherwise add remaining text to the list of elements for the next page
					noMatchList.Add(eText ?? text);
				}
				else
				{
					// Add remaining text to the list of elements for the next page
					noMatchList.Add(eText);
				}
			}
			else
			{
				// Set current maximum height
				if (currentHeight < size.Height + text.PosY)
				{
					currentHeight = size.Height + text.PosY;
				}

				elementList.Add(text);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="textparts"></param>
		/// <param name="currentText"></param>
		/// <param name="position"></param>
		/// <param name="currentHeight"></param>
		/// <param name="maxElementHeightOnPage"></param>
		/// <param name="invertAnalyse"></param>
		/// <returns>Remaining text that did not fit on the current page</returns>
		private T CheckTextparts<T>(IGraphics graphics, List<string> textparts, T currentText, List<T> elementList, ref double currentHeight, double maxElementHeightOnPage, bool invertAnalyse) where T : ElementText, new()
		{
			double tempHeight = 0;
			var tempText = "";
			var textNewPage = "";
			T eText;
			var newPage = false;
			Size textSize;

			if (invertAnalyse)
			{
				// The entire text must be scrolled backwards
				for (var i = textparts.Count - 1; i >= 0; i--)
				{
					textSize = currentText.GetTextSize(graphics, textparts[i] + tempText);

					// Check how many text parts fit on the current page
					if (textSize.Height < maxElementHeightOnPage && !newPage)
					{
						tempText = textparts[i] + tempText;
						tempHeight = textSize.Height;
					}
					else
					{
						newPage = true;
						textNewPage = textparts[i] + textNewPage;
					}
				}
			}
			else
			{
				foreach (var part in textparts)
				{
					textSize = currentText.GetTextSize(graphics, tempText + part);

					// Check how many text parts fit on the current page
					if (textSize.Height + currentText.PosY < maxElementHeightOnPage && !newPage)
					{
						tempText += part;
						tempHeight = textSize.Height;
					}
					else
					{
						newPage = true;
						textNewPage += part;
					}
				}
			}

			if (Math.Abs(tempHeight) < 0.001)
			{
				// Nothing could be distributed to the current page
				eText = null;
			}
			else
			{
				// Set current maximum height
				if (tempHeight + currentText.PosY > currentHeight)
				{
					currentHeight = tempHeight + currentText.PosY;
				}

				// Remember remaining text for the next page
				eText = ElementText.Clone(currentText);
				eText.Content = textNewPage;
				if (!invertAnalyse)
				{
					eText.PosY = 0;
				}

				// Assign text that still fits on the page to the position
				currentText.Content = tempText;
				elementList.Add(currentText);
			}

			if (!(eText?.Content.IsNullOrEmpty() ?? true))
			{
				eText.Content = eText.Content.Trim();
			}

			return eText;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="textSegments"></param>
		/// <param name="currentHtml"></param>
		/// <param name="position"></param>
		/// <param name="currentHeight"></param>
		/// <param name="maxElementHeightOnPage"></param>
		/// <param name="invertAnalyse"></param>
		/// <returns>Remaining text that did not fit on the current page</returns>
		private T CheckTextparts<T>(IGraphics graphics, List<TextSegment> textSegments, T currentHtml, List<T> elementList, ref double currentHeight, double maxElementHeightOnPage, bool invertAnalyse) where T : ElementHtml, new()
		{
			double tempHeight = 0;
			var tempSegments = new List<TextSegment>();
			var textSegmentsNewPage = new List<TextSegment>();
			T eHtml;
			var newPage = false;
			Size textSize;

			if (invertAnalyse)
			{
				// The entire text must be scrolled backwards
				for (var i = textSegments.Count - 1; i >= 0; i--)
				{
					var tempTextSegments = tempSegments.ToList();
					tempTextSegments.Add(textSegments[i]);

					textSize = currentHtml.GetTextSize(graphics, tempTextSegments);

					// Check how many text parts fit on the current page
					if (textSize.Height < maxElementHeightOnPage && !newPage)
					{
						tempSegments.Insert(0, textSegments[i]);
						tempHeight = Math.Max(tempHeight, textSize.Height);
					}
					else
					{
						newPage = true;
						textSegmentsNewPage.Insert(0, textSegments[i]);
					}
				}
			}
			else
			{
				foreach (var textSegment in textSegments)
				{
					var tempTextSegments = tempSegments.ToList();
					tempTextSegments.Add(textSegment);

					textSize = currentHtml.GetTextSize(graphics, tempTextSegments);

					// Check how many text parts fit on the current page
					if (textSize.Height + currentHtml.PosY < maxElementHeightOnPage && !newPage)
					{
						tempSegments.Add(textSegment);
						tempHeight = Math.Max(tempHeight, textSize.Height);
					}
					else
					{
						newPage = true;
						textSegmentsNewPage.Add(textSegment);
					}
				}
			}

			if (Math.Abs(tempHeight) < 0.001)
			{
				// Nothing could be distributed to the current page
				eHtml = null;
			}
			else
			{
				// Set current maximum height
				if (tempHeight + currentHtml.PosY > currentHeight)
				{
					currentHeight = tempHeight + currentHtml.PosY;
				}

				// Remember remaining text for the next page
				eHtml = ElementHtml.Clone(currentHtml);
				eHtml.TextSegments = textSegmentsNewPage;
				if (!invertAnalyse)
				{
					eHtml.PosY = 0;
				}

				// Assign text that still fits on the page to the position
				currentHtml.TextSegments = tempSegments;
				elementList.Add(currentHtml);
			}

			if (!(eHtml?.Content.IsNullOrEmpty() ?? true))
			{
				eHtml.Content = eHtml.Content.Trim();
			}

			return eHtml;
		}

		/// <summary>
		/// Sets the option in the position that it is the first on the current page
		/// Attention: This operation cannot be undone
		/// </summary>
		/// <param name="graphics">Graphics element</param>
		public void SetPositionOnTop(IGraphics graphics)
		{
			// Position size, if it does not appear first on the page
			var size = GetSize(graphics);
			// Position size, if it does appear first on the page
			var sizeNew = GetSize(graphics);
			var heightDifference = sizeNew.Height - size.Height;

			ChangeElementYPositions(heightDifference);
		}

		public void ChangeElementYPositions(double heightDifference)
		{
			// Add height difference to the Y-position of all elements (except those that only appear if the position is at the first position on the page
			if (heightDifference > 0)
			{
				ContentImage.ToList().ForEach(e => { e.PosY += heightDifference; });
				ContentLine.Where(e => !e.MaxHeight).ToList().ForEach(e => { e.PosY += heightDifference; e.Height += heightDifference; });
				ContentRectangle.Where(e => !e.MaxHeight).ToList().ForEach(e => { e.PosY += heightDifference; });
				ContentRectangleFill.Where(e => !e.MaxHeight).ToList().ForEach(e => { e.PosY += heightDifference; });
				ContentLine.Where(e => e.MaxHeight && e.EndsDiffHeight).ToList().ForEach(e => { e.Height = heightDifference; });
				ContentLine.Where(e => e.MaxHeight && !e.EndsDiffHeight).ToList().ForEach(e => { e.Height += heightDifference; });
				ContentRectangle.Where(e => e.MaxHeight).ToList().ForEach(e => { e.Height += heightDifference; });
				ContentRectangleFill.Where(e => e.MaxHeight).ToList().ForEach(e => { e.Height += heightDifference; });
				ContentText.ToList().ForEach(e => { e.PosY += heightDifference; });
				ContentHtml.ToList().ForEach(e => { e.PosY += heightDifference; });
				ContentPageNo.ToList().ForEach(e => { e.PosY += heightDifference; });
			}
		}
	}
}