using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Eshava.Report.Pdf.Core.Enums;
using Eshava.Report.Pdf.Core.Interfaces;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ReportPositionContainer
	{
		public ReportPositionContainer()
		{
			Positions = new List<ReportPosition>();
			RepeatOnTop = new List<ReportPosition>();
			MainPositions = new List<ReportPosition>();
			PreMainPositions = new Dictionary<int, ReportPosition>();
			PostMainPositions = new Dictionary<int, ReportPosition>();

			PreMainPositionHeight = new Dictionary<int, double>();
			MainPositionHeight = new Dictionary<int, double>();
			PostMainPositionHeight = new Dictionary<int, double>();
			PositionToRepeatHeight = 0;
		}

		/// <summary>
		/// Returns a list with all positions contained in the document 
		/// </summary>
		[XmlElement("Position")]
		public List<ReportPosition> Positions { get; }

		/// <summary>
		/// Returns a list of all positions that must be repeated on each new page 
		/// </summary>
		[XmlIgnore]
		public List<ReportPosition> RepeatOnTop { get; }

		/// <summary>
		/// Returns a list with all positions that represent the actual positions to be displayed 
		/// </summary>
		[XmlIgnore]
		public List<ReportPosition> MainPositions { get; }

		/// <summary>
		/// Returns a dictionary containing all positions to be displayed before an position (if the position is displayed first on a page)
		/// Note: The key is the SequenceNo
		/// </summary>
		[XmlIgnore]
		public Dictionary<int, ReportPosition> PreMainPositions { get; }

		/// <summary>
		/// Returns a dictionary containing all positions to be displayed after an position (if the position is displayed last on a page)
		/// Note: The key is the SequenceNo
		/// </summary>
		[XmlIgnore]
		public Dictionary<int, ReportPosition> PostMainPositions { get; }

		/// <summary>
		/// Returns a dictionary with the heights of each pre-position
		/// Note: The key is the SequenceNo
		/// </summary>
		[XmlIgnore]
		public Dictionary<int, double> PreMainPositionHeight { get; }

		/// <summary>
		/// Returns a dictionary with the heights of the individual actual positions
		/// Note: The key is the SequenceNo
		/// </summary>
		[XmlIgnore]
		public Dictionary<int, double> MainPositionHeight { get; }

		/// <summary>
		/// Returns a dictionary with the heights of the individual post positions
		/// Note: The key is the SequenceNo
		/// </summary>
		[XmlIgnore]
		public Dictionary<int, double> PostMainPositionHeight { get; }

		/// <summary>
		/// Returns the total height of all positions to be repeated at the top of the page
		/// </summary>
		[XmlIgnore]
		public double PositionToRepeatHeight { get; private set; }

		public void AnalyzePositions(IGraphics graphics)
		{
			var move = new MoveElementLogic();
			RepeatOnTop.Clear();
			MainPositions.Clear();
			PreMainPositions.Clear();
			PostMainPositions.Clear();

			PreMainPositionHeight.Clear();
			MainPositionHeight.Clear();
			PostMainPositionHeight.Clear();
			PositionToRepeatHeight = 0;

			foreach (var position in Positions)
			{
				switch (position.Type)
				{
					case PositonType.RepeatOnTop:
						RepeatOnTop.Add(position);
						PositionToRepeatHeight += position.GetSize(graphics).Height;
						break;
					case PositonType.OnlyOnNewPage:
						AddToList(position, PreMainPositions);
						AddHeightToList(graphics, position, PreMainPositionHeight);
						break;
					case PositonType.OnlyAsLastOnPage:
						AddToList(position, PostMainPositions);
						AddHeightToList(graphics, position, PostMainPositionHeight);
						break;
					default:
						MainPositions.Add(position);
						move.AnalyzeElements(graphics, position);
						AddHeightToList(graphics, position, MainPositionHeight);
						break;
				}

				CalculateDynamicLineHeight(graphics, position);
				CheckPositionCohesion(position);
			}
		}

		private void AddToList(ReportPosition pos, Dictionary<int, ReportPosition> list)
		{
			if (list.ContainsKey(pos.SequenceNo))
			{
				list[pos.SequenceNo] = pos;
			}
			else
			{
				list.Add(pos.SequenceNo, pos);
			}
		}

		private void AddHeightToList(IGraphics graphics, ReportPosition pos, Dictionary<int, double> list)
		{
			var size = pos.GetSize(graphics);

			if (list.ContainsKey(pos.SequenceNo))
			{
				list[pos.SequenceNo] = size.Height;
			}
			else
			{
				list.Add(pos.SequenceNo, size.Height);
			}
		}

		private void CalculateDynamicLineHeight(IGraphics graphics, ReportPosition pos)
		{
			var elements = new List<ElementLine>();
			elements.AddRange(pos.ContentLine);
			elements.AddRange(pos.ContentRectangle);
			elements.AddRange(pos.ContentRectangleFill);
			CalculateDynamicLineHeight(graphics, pos, elements);
		}

		private void CalculateDynamicLineHeight(IGraphics graphics, ReportPosition pos, List<ElementLine> elements)
		{
			var size = pos.GetSize(graphics);

			elements.Where(p => p.MaxHeight).ToList().ForEach(p =>
			{
				if (p.Height > 0)
				{
					p.Height = size.Height - p.Height;
				}
				else
				{
					p.Height = size.Height;
				}
			});
		}

		private void CheckPositionCohesion(ReportPosition pos)
		{
			if (pos.CohesionPercentage < 0)
			{
				pos.CohesionPercentage = 0;
			}
			else if (pos.CohesionPercentage > 100)
			{
				pos.CohesionPercentage = 100;
			}
		}
	}
}
