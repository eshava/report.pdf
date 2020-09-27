using System;
using System.Collections.Generic;
using System.Linq;
using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Extensions;
using Eshava.Report.Pdf.Core.Interfaces;
using Eshava.Report.Pdf.Core.Models;
using Eshava.Report.Pdf.Enums;

namespace Eshava.Report.Pdf.Core
{
	public class PageCalculator
	{
		private enum PositionState
		{
			/// <summary>
			/// Position fits including post position completely on the current page
			/// </summary>
			CompleteOnCurrentPage = 1,

			/// <summary>
			/// Position does not fit completely on a new page including pre and post position
			/// </summary>
			SplittetOnCurrentPage = 2,

			/// <summary>
			/// Position fits completely on a new page including pre and post position
			/// </summary>
			UseNewCurrentPage = 4
		}

		private readonly Models.Report _report;
		private Size _pageSize;
		private PageMargins _margin;
		private ReportPagePart _headerFirst;
		private ReportPagePart _headerFollow;
		private ReportPagePart _footerFirst;
		private ReportPagePart _footerFollow;

		private double _prePositionHeight;
		private double _postPositionHeight;

		public PageCalculator(Models.Report report, Size pageSize)
		{
			_report = report;
			_pageSize = pageSize;
			SetPageMargin();
			SetHeader();
			SetFooter();
		}

		private bool HasPositions
		{
			get { return _report != null && ContainerHasPositions(_report.PositionContainer); }
		}

		private bool HasPrePositions
		{
			get { return _report != null && ContainerHasPositions(_report.PrePositionContainer); }
		}

		private bool HasPostPositions
		{
			get { return _report != null && ContainerHasPositions(_report.PostPositionContainer); }
		}

		private bool IsPageBreak(ReportPosition position)
		{
			return position.Type == PositonType.Pagebreak;
		}

		private bool IsForceNewPage(ReportPosition position)
		{
			return position.Type == PositonType.ForceNewPage;
		}

		private bool ContainerHasPositions(ReportPositionContainer container)
		{
			return container != null && container.Positions != null && container.Positions.Count > 0;
		}

		private ReportPage AddPrePositions(IGraphics graphics, ReportPage page, CalulationState state)
		{
			if (!HasPrePositions)
			{
				return page;
			}

			var positionContainer = _report.PrePositionContainer;

			foreach (var position in positionContainer.Positions)
			{
				var positionSize = position.GetSize(graphics);

				// Check whether the position still fits on the same page or whether a page break should be forced
				if (state.PositionPartHeight + positionSize.Height > page.MaxPositionPartHeight)
				{
					state.IsFirstPage = false;
					state.NewPageHeight = CreatePage(state.CurrentPageNumber, false).MaxPositionPartHeight;

					page = AddPositionsToBeSplitt(graphics, page, position, positionContainer, state, state.NewPageHeight, (page.MaxPositionPartHeight - state.PositionPartHeight));
				}
				else
				{
					// Continue using the existing page
					page.Positions.Add(position);
					state.PositionPartHeight += positionSize.Height;
					state.IsNewPage = false;
				}
			}

			return page;
		}

		private ReportPage AddPostPositions(IGraphics graphics, ReportPage page, CalulationState state)
		{
			if (!HasPostPositions)
			{
				return page;
			}

			state.PositionPartHeight += state.PositionToRepeatHeight;
			state.NewPageHeight = CreatePage(state.CurrentPageNumber, state.IsFirstPage).MaxPositionPartHeight;

			// Check whether all post positions still fits on one page or whether they have to be splitted
			if (_postPositionHeight <= state.NewPageHeight)
			{
				// Check whether all post positions still fits on the current page
				if (state.PositionPartHeight + _postPositionHeight > page.MaxPositionPartHeight)
				{
					AddPostMainPosition(page, _report.PositionContainer, state.CurrentSequenceNumber, page.MaxPositionPartHeight - state.PositionPartHeight, 0);

					// Not all post position elements fit on the current page
					// Create new page
					page = CloseAndCreateNewPage(page, state, 0, null, false);

					page.Positions.AddRange(state.PositionsToRepeat);
					state.PositionPartHeight += state.PositionToRepeatHeight;
					state.PositionPartHeight = AddPreMainPosition(graphics, page, _report.PositionContainer, state.CurrentSequenceNumber + 1, state.PositionPartHeight);
				}

				var postPositions = _report.PostPositionContainer.Positions;
				var differenceHeight = page.MaxPositionPartHeight - state.PositionPartHeight - _postPositionHeight;
				for (var i = 0; i < postPositions.Count; i++)
				{
					// Since the post positions are stuck to the foot, 
					// the difference between available height and post position height must be added to the first post position
					if (i == 0)
					{
						postPositions[i].ChangeElementYPositions(differenceHeight);
					}

					// Add post position to the current page
					page.Positions.Add(postPositions[i]);
				}
			}
			else
			{
				// Post positions have to be splitted
				// Post positions must now be split backwards
				var postPage = new Dictionary<int, List<ReportPosition>>();
				var postPageSize = new Dictionary<int, double>();
				var maxPageIndex = CalculatePageSplittingForPostPositions(graphics, state.NewPageHeight, postPage, postPageSize);

				for (var pageIndex = maxPageIndex; pageIndex >= 0; pageIndex--)
				{
					// Check whether post position still fits on the current page
					if (pageIndex != maxPageIndex || state.PositionPartHeight + postPageSize[pageIndex] > page.MaxPositionPartHeight)
					{
						if (pageIndex == maxPageIndex)
						{
							AddPostMainPosition(page, _report.PositionContainer, state.CurrentSequenceNumber, page.MaxPositionPartHeight - state.PositionPartHeight, 0);
						}

						page = CloseAndCreateNewPage(page, state, 0, null, false);
					}

					var differenceHeight = page.MaxPositionPartHeight - state.PositionPartHeight - postPageSize[pageIndex];
					for (var positionIndex = postPage[pageIndex].Count - 1; positionIndex >= 0; positionIndex--)
					{
						// Last position in the list is the first position to be displayed
						// Set the difference of the first displayed position
						if (positionIndex == postPage[pageIndex].Count - 1)
						{
							postPage[pageIndex][positionIndex].ChangeElementYPositions(differenceHeight);
						}

						page.Positions.Add(postPage[pageIndex][positionIndex]);
					}
				}
			}

			return page;
		}

		private (ReportPage Page, PositionResult Result) AddPosition(IGraphics graphics, ReportPage page, CalulationState state, ReportPosition position, ReportPositionContainer positionContainer)
		{
			// Remember the sequence number of the current position
			state.CurrentSequenceNumber = position.SequenceNo;

			// Calculate position height
			var currentPosHeight = positionContainer.MainPositionHeight.CheckDictionary(state.CurrentSequenceNumber);

			if (IsForceNewPage(position))
			{
				state.PositionPartHeight -= currentPosHeight;
			}

			// Check whether the cancelation condition is reached
			if (state.PositionToRepeatHeight + state.PositionPartHeight + currentPosHeight > page.MaxPositionPartHeight)
			{
				// Cancelation condition: if the pre postions of a page uses so many space, that no further positions can be displayed. 
				// Check whether the postion can be split so that it can still be displayed
				if (Math.Abs(state.PositionPartHeight) < 0.0001)
				{
					state.Pages.Add(page);

					return (page, PositionResult.Cancel);
				}
			}

			// Check whether the current position fits on the current page
			var prePositionHeight = positionContainer.PreMainPositionHeight.CheckDictionary(state.CurrentSequenceNumber);
			var postPositionHeight = positionContainer.PostMainPositionHeight.CheckDictionary(state.CurrentSequenceNumber);

			// Check whether the new position incl. post position still fits on the current page
			PositionState positionState;
			if (state.PositionToRepeatHeight + state.PositionPartHeight + currentPosHeight + postPositionHeight > page.MaxPositionPartHeight || IsPageBreak(position) || IsForceNewPage(position))
			{
				// The current position no longer fits on the current page when the post position is taken into account.
				// Fits the current position as a whole on the next page (including pre- and post-position)
				// If the current position is so large that it would not fit on the next page
				// Additionally, the case must be considered that the current position contains an element that appears at the top of a new page, so that the position becomes larger again 
				// Since the current page could be the first page, the next page will most likely have a different layout, so you must check with this height

				state.NewPageHeight = CreatePage(state.CurrentPageNumber, state.IsFirstPage).MaxPositionPartHeight;

				if (state.PositionToRepeatHeight + currentPosHeight + prePositionHeight + postPositionHeight <= state.NewPageHeight || IsPageBreak(position) || IsForceNewPage(position))
				{
					// Check if the preview position is set to "PreventLastOnPage"
					if ((page.Positions.LastOrDefault(p => p.Type == PositonType.Default)?.PreventLastOnPage ?? false))
					{
						return (page, PositionResult.MovePreviewPositonToNextPage);
					}

					// The current position fits completely on a new page including pre- and post-position, 
					// so the post position is added to the previous position and the current page is closed
					// Continue use existing page and add pre-position
					if (positionContainer.PostMainPositions.ContainsKey(state.CurrentSequenceNumber - 1))
					{
						AddPostMainPosition(page, positionContainer, state.CurrentSequenceNumber - 1, page.MaxPositionPartHeight - state.PositionPartHeight - state.PositionToRepeatHeight, 0);
					}

					// Complete current page
					page = CloseAndCreateNewPage(page, state);
					positionState = PositionState.UseNewCurrentPage;
				}
				else
				{
					// The current position has to be split, because this incl. pre- and post-position does not fit completely on a new page
					positionState = PositionState.SplittetOnCurrentPage;
				}
			}
			else
			{
				// The current position still fits completely on the current page
				positionState = PositionState.CompleteOnCurrentPage;
			}

			// Forced page breaks have already been considered
			if (!IsPageBreak(position))
			{
				// If the current page is not the first page but the position is the first to be displayed
				if (state.IsNewPage && !state.IsFirstPage)
				{
					// Continue use exising page and add pre positions
					if (positionContainer.PreMainPositions.ContainsKey(state.CurrentSequenceNumber))
					{
						page.Positions.Add(positionContainer.PreMainPositions[state.CurrentSequenceNumber]);
						state.PositionPartHeight += positionContainer.PreMainPositionHeight[state.CurrentSequenceNumber];
						state.IsNewPage = false;
					}
				}

				switch (positionState)
				{
					case PositionState.CompleteOnCurrentPage:
						// Continue use exising page
						page.Positions.Add(position);
						state.PositionPartHeight += currentPosHeight;
						state.IsNewPage = false;
						break;
					case PositionState.SplittetOnCurrentPage:
						prePositionHeight = positionContainer.PreMainPositionHeight.ContainsKey(state.CurrentSequenceNumber) ? positionContainer.PreMainPositionHeight[state.CurrentSequenceNumber] : 0;
						postPositionHeight = positionContainer.PostMainPositionHeight.ContainsKey(state.CurrentSequenceNumber) ? positionContainer.PostMainPositionHeight[state.CurrentSequenceNumber] : 0;
						position.GetSize(graphics);
						state.NewPageHeight = CreatePage(state.CurrentPageNumber, false).MaxPositionPartHeight;
						var maxElementHeight = state.NewPageHeight - state.PositionToRepeatHeight - prePositionHeight - postPositionHeight;
						var maxElementHeightOnCurrentPage = page.MaxPositionPartHeight - state.PositionToRepeatHeight - state.PositionPartHeight - postPositionHeight;
						page = AddPositionsToBeSplitt(graphics, page, position, positionContainer, state, maxElementHeight, maxElementHeightOnCurrentPage);
						break;
					case PositionState.UseNewCurrentPage:
						// A new page has already been created, which is now to be used
						position.GetSize(graphics);
						page.Positions.Add(position);
						state.NewPageHeight = page.MaxPositionPartHeight;
						state.PositionPartHeight += currentPosHeight;
						state.IsNewPage = false;
						break;
				}
			}

			return (page, PositionResult.Next);
		}

		/// <summary>
		/// Creates the pages from the passed report object which are printed in the Pdf.
		/// </summary>
		/// <param name="graphics">Graphics element</param>
		/// <returns>List of all report pages</returns>
		public List<ReportPage> CalculatePages(IGraphics graphics)
		{
			var state = new CalulationState();

			if (_report != null)
			{
				// Prepare first page
				var page = CreatePage(state.CurrentPageNumber, true);

				// Calculate how large the area is that appears before and after the positions on the page
				SetPreAndPostPositionHeight(graphics);

				// Contains the report positions
				if (!HasPositions && !HasPrePositions && !HasPostPositions)
				{
					// No positions available, so print only the header and footer of the first page
					state.Pages.Add(page);
				}
				else
				{
					var minPositionHeight = Math.Min(page.MaxPositionPartHeight, CreatePage(state.CurrentPageNumber, false).MaxPositionPartHeight);

					// Add pre position elements (These are to be inserted on the first page between header and first position)
					page = AddPrePositions(graphics, page, state);

					if (HasPositions)
					{
						// Start analysis of the position list, determines the actual positions and the corresponding pre and post positons
						_report.PositionContainer.AnalyzePositions(graphics);

						// First determine all positions to be repeated
						if (GetPositionsToRepeatOnTop(graphics, state, minPositionHeight))
						{
							// All positions to be repeated at the beginning of the page are already larger than the position area (measured at the smallest position area first or following page)
							// Normal position can no longer be printed
							page.Positions.AddRange(state.PositionsToRepeat);
							state.Pages.Add(page);

							return state.Pages;
						}

						// Do the positions to be repeated still fit on the current page? 
						// Check by the pre position area required
						if (page.MaxPositionPartHeight - state.PositionPartHeight <= state.PositionToRepeatHeight)
						{
							// Positions to be repeated do not fit as a whole on the current page
							page = CloseAndCreateNewPage(page, state, 0, null, false);
						}

						// Add all positions to be repeated at the beginning of the position
						page.Positions.AddRange(state.PositionsToRepeat);

						// Remember current container
						var positionContainer = _report.PositionContainer;

						var positionIndex = 0;
						do
						{
							var position = positionContainer.MainPositions[positionIndex];
							var result = AddPosition(graphics, page, state, position, positionContainer);
							page = result.Page;

							if (result.Result == PositionResult.Cancel)
							{
								break;
							}

							if (result.Result == PositionResult.MovePreviewPositonToNextPage)
							{
								var previousSequenceNumber = position.SequenceNo - 1;
								var tempPositionIndex = positionIndex;
								while (true)
								{
									tempPositionIndex--;
									position = positionContainer.MainPositions[tempPositionIndex];
									if (position.SequenceNo == previousSequenceNumber)
									{
										var indexPositonRemove = page.Positions.IndexOf(position);
										for (var i = page.Positions.Count - 1; i >= indexPositonRemove; i--)
										{
											page.Positions.RemoveAt(i);
										}
									}

									if (position.Type == PositonType.Default || position.SequenceNo < previousSequenceNumber)
									{
										break;
									}
								}

								if (position.SequenceNo == previousSequenceNumber)
								{
									position.Type = PositonType.ForceNewPage;
									position.PreventLastOnPage = false;

									result = AddPosition(graphics, page, state, position, positionContainer);
									position.Type = PositonType.Default;
									page = result.Page;
									positionIndex = tempPositionIndex;
								}
							}

							positionIndex++;
						} while (positionIndex < positionContainer.MainPositions.Count);
					}

					// Add post position elements (these are to be inserted on the last page between the last position and footer)
					// These elements must be calculated foot-oriented
					page = AddPostPositions(graphics, page, state);
				}

				if (page != null)
				{
					state.Pages.Add(page);
				}
			}

			SetTotalPageCount(graphics, state.Pages);

			// At the end the graphics element must be disposed so that a new one can be created
			graphics.Dispose();

			return state.Pages;
		}

		private int CalculatePageSplittingForPostPositions(IGraphics graphics, double newPageHeight, Dictionary<int, List<ReportPosition>> postPage, Dictionary<int, double> postPageSize)
		{
			Size positionSize;
			var postSize = new Dictionary<int, Size>();
			var post = new Dictionary<int, ReportPosition>();
			var lastPositionIndex = 0;

			// Run through all positions and remember position and size
			foreach (var position in _report.PostPositionContainer.Positions)
			{
				positionSize = position.GetSize(graphics);
				postSize.Add(lastPositionIndex, positionSize);
				post.Add(lastPositionIndex, position);
				lastPositionIndex++;
			}
			lastPositionIndex--;

			var pageIndex = 0;
			var pageHeight = 0.0;

			// Now go backwards through the dictionary
			for (var positionIndex = lastPositionIndex; positionIndex >= 0; positionIndex--)
			{
				if (pageHeight + postSize[positionIndex].Height <= newPageHeight)
				{
					// Position still fits completely on the page
					AddPostPositionToList(postPage, postPageSize, post[positionIndex], ref pageHeight, pageIndex, postSize[positionIndex].Height);
				}
				else if (postSize[positionIndex].Height <= newPageHeight)
				{
					// Position still fits completely on a new page
					pageIndex++;
					pageHeight = 0;
					AddPostPositionToList(postPage, postPageSize, post[positionIndex], ref pageHeight, pageIndex, postSize[positionIndex].Height);
				}
				else
				{
					// The position as a whole does not fit on a new page
					// The position must now be split backwards

					// Position is now tried to split, to the current page and to the following pages (backwards)
					var splittPositions = post[positionIndex].SplitPositionInvert(graphics, newPageHeight, newPageHeight - pageHeight);

					for (var splittPositionIndex = 0; splittPositionIndex < splittPositions.Count; splittPositionIndex++)
					{
						if (splittPositionIndex != 0)
						{
							pageIndex++;
							pageHeight = 0;
						}
						positionSize = splittPositions[splittPositionIndex].GetSize(graphics);
						AddPostPositionToList(postPage, postPageSize, splittPositions[splittPositionIndex], ref pageHeight, pageIndex, positionSize.Height);
					}
				}
			}

			return pageIndex;
		}

		/// <summary>
		/// Inserts a passed position into the list of positions per page 
		/// </summary>
		/// <param name="postPages">Post postions per page</param>
		/// <param name="postPageSize">Liste of post oosition sizes per page</param>
		/// <param name="position">Current position</param>
		/// <param name="pageHeight">Height of the current page</param>
		/// <param name="pageIndex">Index of the current page</param>
		/// <param name="positionHeight">Height of the current position</param>
		private void AddPostPositionToList(Dictionary<int, List<ReportPosition>> postPages, Dictionary<int, double> postPageSize, ReportPosition position, ref double pageHeight, int pageIndex, double positionHeight)
		{
			pageHeight += positionHeight;

			if (!postPages.ContainsKey(pageIndex))
			{
				postPages.Add(pageIndex, new List<ReportPosition>());
				postPageSize.Add(pageIndex, 0);
			}

			postPages[pageIndex].Add(position);
			postPageSize[pageIndex] += positionHeight;
		}

		/// <summary>
		/// Completes the current page and creates a new page
		/// </summary>
		/// <param name="page">Current page</param>
		/// <param name="state">Calculation state</param>
		/// <param name="positionHeight">Height of the current position</param>
		/// <param name="position">Current position</param>
		/// <param name="usePositionsToRepeat"></param>
		/// <returns>New page</returns>
		private ReportPage CloseAndCreateNewPage(ReportPage page, CalulationState state, double positionHeight = 0, ReportPosition position = null, bool usePositionsToRepeat = true)
		{
			// Complete existing page, because current position fits on the next page
			state.Pages.Add(page);

			// New page
			state.CurrentPageNumber++;
			state.IsFirstPage = false;
			page = CreatePage(state.CurrentPageNumber, false);
			state.IsNewPage = true;
			state.PositionPartHeight = positionHeight;

			// Add all positions to be repeated at the beginning of the position
			if (usePositionsToRepeat && state.PositionsToRepeat != null && state.PositionsToRepeat.Count > 0)
			{
				page.Positions.AddRange(state.PositionsToRepeat);
			}

			if (position != null)
			{
				page.Positions.Add(position);
			}

			return page;
		}

		/// <summary>
		/// Creates a new Pdf page template
		/// </summary>
		/// <param name="pageNumber">Aktuelle Seitenzahl</param>
		/// <param name="isFirstPage">First page of the document</param>
		/// <returns>Pdf page</returns>
		private ReportPage CreatePage(int pageNumber, bool isFirstPage)
		{
			var page = new ReportPage
			{
				Margins = _margin,
				PageNumber = pageNumber,
				TotalPageCount = pageNumber,
				Header = GetHeader(isFirstPage),
				Footer = GetFooter(isFirstPage),
				Positions = new List<ReportPosition>()
			};

			page.CalcPositionsHeightPart(_pageSize.Height);
			page.MaxPositionPartWidth = _pageSize.Width;

			return page;
		}

		/// <summary>
		/// Sets the total number of all determined pages in each page, 
		/// additionally the first position on each page is determined (positions to be repeated are not considered)
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="pages">Report pages</param>
		private void SetTotalPageCount(IGraphics graphics, List<ReportPage> pages)
		{
			foreach (var page in pages)
			{
				page.TotalPageCount = pages.Count;
				if (page.PageNumber > 1 && page.Positions != null && page.Positions.Count > 0)
				{
					page.Positions.FirstOrDefault(p => p.Type != PositonType.RepeatOnTop)?.SetPositionOnTop(graphics);
				}
			}
		}

		/// <summary>
		/// Determines all positions from the report object to be repeated at the beginning of the page
		/// </summary>
		/// <param name="graphics">Graphics element</param>
		/// <param name="state">Calculation state</param>
		/// <param name="maxPositionPartHeight">Maximum height of the position area</param>
		/// <returns>Returns whether the page calculation should be canceld</returns>
		private bool GetPositionsToRepeatOnTop(IGraphics graphics, CalulationState state, double maxPositionPartHeight)
		{
			state.PositionToRepeatHeight = _report.PositionContainer.PositionToRepeatHeight;

			if (state.PositionToRepeatHeight < maxPositionPartHeight)
			{
				// All positions to be repeated at the beginning of the page fit into the position area
				state.PositionsToRepeat.AddRange(_report.PositionContainer.RepeatOnTop);
				return false;
			}

			// All positions to be repeated at the beginning of the page are already larger than the position area
			// Normal position can no longer be printed
			state.PositionToRepeatHeight = 0;

			foreach (var position in _report.PositionContainer.RepeatOnTop)
			{
				var positionSize = position.GetSize(graphics);
				if (state.PositionToRepeatHeight + positionSize.Height > maxPositionPartHeight)
				{
					// All positions to be repeated at the beginning of the page are already larger than the position area
					// Normal position can no longer be printed
					return true;
				}

				state.PositionToRepeatHeight += positionSize.Height;
				state.PositionsToRepeat.Add(position);
			}

			return true;
		}

		/// <summary>
		/// Calculates the height of the position area that should appear before and after the positions on the document
		/// <param name="graphics">Graphics element</param>
		/// </summary>
		private void SetPreAndPostPositionHeight(IGraphics graphics)
		{
			if (_report == null)
			{
				return;
			}

			if (HasPrePositions)
			{
				_report.PrePositionContainer.AnalyzePositions(graphics);
				_report.PrePositionContainer.Positions.ForEach(p => _prePositionHeight += p.GetSize(graphics).Height);
			}

			if (HasPostPositions)
			{
				_report.PostPositionContainer.AnalyzePositions(graphics);
				_report.PostPositionContainer.Positions.ForEach(p => _postPositionHeight += p.GetSize(graphics).Height);
			}
		}

		/// <summary>
		/// Returns the corresponding header area for the first page or following page
		/// Note: If no following page is stored, the first page is automatically used
		/// </summary>
		/// <param name="isFirstPage">First page of the document</param>
		/// <returns>Header area</returns>
		private ReportPagePart GetHeader(bool isFirstPage)
		{
			return isFirstPage ? _headerFirst : _headerFollow;
		}

		/// <summary>
		/// Returns the corresponding footer area for the first page or following page
		/// Note: If no following page is stored, the first page is automatically used
		/// </summary>
		/// <param name="isFirstPage">First page of the document</param>
		/// <returns>Header area</returns>
		private ReportPagePart GetFooter(bool isFirstPage)
		{
			return isFirstPage ? _footerFirst : _footerFollow;
		}

		/// <summary>
		/// Set the object that contains the margins of the report
		/// </summary>
		/// <returns>Page porders</returns>
		private void SetPageMargin()
		{
			_margin = (_report == null || _report.Information == null) ? new PageMargins(0, 0, 0, 0) : _report.Information.PageMargins;
		}

		/// <summary>
		/// Set the header area of the first and following page
		/// </summary>
		private void SetHeader()
		{
			if (_report.Header != null)
			{
				_headerFirst = _report.Header.FirstPage;
				_headerFollow = _report.Header.FollowingPage ?? _report.Header.FirstPage;
			}
		}

		/// <summary>
		///  Set the footer area of the first and following page
		/// </summary>
		private void SetFooter()
		{
			if (_report.Footer != null)
			{
				_footerFirst = _report.Footer.FirstPage;
				_footerFollow = _report.Footer.FollowingPage ?? _report.Footer.FirstPage;
			}
		}

		/// <summary>
		/// Splits a passed position into several positions and adds them to the current and the new page(s)
		/// </summary>
		/// <param name="graphics">Graphics element</param>
		/// <param name="page">Current page</param>
		/// <param name="position">Positions to be splitt</param>
		/// <param name="positionContainer"></param>
		/// <param name="state">Calculation state</param>
		/// <param name="maxElementHeight">Maximum height of the next page</param>
		/// <param name="maxElementHeightOnCurrentPage">Remaining height of the current page</param>
		/// <returns>Last created page</returns>
		private ReportPage AddPositionsToBeSplitt(IGraphics graphics, ReportPage page, ReportPosition position, ReportPositionContainer positionContainer, CalulationState state, double maxElementHeight, double maxElementHeightOnCurrentPage)
		{
			// Position is now tried to split, to the current page and to the following pages
			var splittPositions = position.SplitPosition(graphics, maxElementHeight, maxElementHeightOnCurrentPage);

			var postMainPositionHeight = 0.0;
			if (positionContainer.PostMainPositionHeight.ContainsKey(position.SequenceNo))
			{
				postMainPositionHeight = positionContainer.PostMainPositionHeight[position.SequenceNo];
			}

			if (splittPositions[0].IsEmpty)
			{
				// Continue use exising page
				// Don't add first empty splitt position
				// Add post position to the previous page

				AddPostMainPosition(page, positionContainer, position.SequenceNo - 1, maxElementHeightOnCurrentPage + postMainPositionHeight, 0);
			}
			else
			{
				// Continue use exising page
				page.Positions.Add(splittPositions[0]);
				// Add post position to the current page
				AddPostMainPosition(page, positionContainer, position.SequenceNo, maxElementHeightOnCurrentPage + postMainPositionHeight, splittPositions[0].GetSize(graphics).Height);
			}

			// Complete existing page, because current position fits on the next page
			page = CloseAndCreateNewPage(page, state);

			// Go through all split positions (the position at index 0 must be skipped as it has already been added
			for (var positionIndex = 1; positionIndex < splittPositions.Count; positionIndex++)
			{
				// Each position in the list represents a new page

				// Add pre position
				if (positionContainer.PreMainPositions.ContainsKey(position.SequenceNo + 1))
				{
					page.Positions.Add(positionContainer.PreMainPositions[position.SequenceNo + 1]);
					// Note the height of the current position, if there are still positions following
					state.PositionPartHeight = positionContainer.PreMainPositionHeight[position.SequenceNo + 1];
				}

				page.Positions.Add(splittPositions[positionIndex]);
				// Note the height of the current position, if there are still positions following
				state.PositionPartHeight += splittPositions[positionIndex].GetSize(graphics).Height;
				state.IsNewPage = false;
				// If the last page of the splitt positions is reached, do not create a new page, as the following positions may still fit on this page
				if (positionIndex + 1 < splittPositions.Count)
				{
					AddPostMainPosition(page, positionContainer, position.SequenceNo, maxElementHeight, splittPositions[positionIndex].GetSize(graphics).Height);

					// Complete existing page, because current position fits on the next page
					page = CloseAndCreateNewPage(page, state);
				}
			}

			return page;
		}

		private void AddPostMainPosition(ReportPage page, ReportPositionContainer positionContainer, int sequenceNumber, double positionPartHeight, double positionHeight)
		{
			if (positionContainer.PostMainPositions.ContainsKey(sequenceNumber))
			{
				page.Positions.Add(positionContainer.PostMainPositions[sequenceNumber]);
				// Should be at the end of the page directly before the footer
				positionContainer.PostMainPositions[sequenceNumber].ChangeElementYPositions(positionPartHeight - positionContainer.PostMainPositionHeight[sequenceNumber] - positionHeight);
			}
		}

		private double AddPreMainPosition(IGraphics graphics, ReportPage page, ReportPositionContainer positionContainer, int sequenceNumber, double positionPartHeight)
		{
			if (positionContainer.PreMainPositions.ContainsKey(sequenceNumber))
			{
				page.Positions.Add(positionContainer.PreMainPositions[sequenceNumber]);
				positionPartHeight += positionContainer.PreMainPositions[sequenceNumber].GetSize(graphics).Height;
			}

			return positionPartHeight;
		}
	}
}