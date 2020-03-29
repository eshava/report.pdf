using System;
using System.Collections.Generic;

namespace Eshava.Report.Pdf.Core.Models
{
	public class ElementCollection
	{
		private readonly Dictionary<Type, object> _elements;

		public ElementCollection()
		{
			_elements = new Dictionary<Type, object>();
		}

		public void AddElement<T>(T element) where T : ElementBase
		{
			var list = CheckType<T>();
			list.Add(element);
		}

		public void AddElementRange<T>(IEnumerable<T> elements) where T : ElementBase
		{
			var list = CheckType<T>();
			list.AddRange(elements);
		}

		public void RemoveElement<T>(T element) where T : ElementBase
		{
			var list = CheckType<T>();
			list.Remove(element);
		}

		public List<T> GetElements<T>() where T : ElementBase
		{
			return CheckType<T>();
		}

		/// <summary>
		/// Removes all elements for the given type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void Clear<T>() where T : ElementBase
		{
			var list = CheckType<T>();
			list.Clear();
		}

		/// <summary>
		/// Removes all elements
		/// </summary>
		public void Clear()
		{
			_elements.Clear();
		}

		private List<T> CheckType<T>() where T : ElementBase
		{
			var t = typeof(T);
			if (!_elements.ContainsKey(t))
			{
				_elements.Add(t, new List<T>());
			}

			return (List<T>)_elements[t];
		}
	}
}