using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
	public static class RandomUtils
	{
		/// <summary>
		/// Возвращает случайный элемент из коллекции
		/// </summary>
		public static T GetRandomItem<T>(IEnumerable<T> values)
		{
			var enumerable = values as T[] ?? values.ToArray();
			if (enumerable.Count() == 0)
				return default(T);
			var index = Random.Range(0, enumerable.Count());
			return enumerable.ElementAt(index);
		}
	}
}
