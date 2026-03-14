using System;
using System.Collections.Generic;
using System.Linq;

namespace JPEG.Utilities;

static class IEnumerableExtensions
{
	public static T MinOrDefault<T>(this IEnumerable<T> enumerable, Func<T, int> selector)
	{
		return enumerable.OrderBy(selector).FirstOrDefault();
	}
	public static IEnumerable<T> Without<T>(this IEnumerable<T> enumerable, T element)
	{
		var comparer = EqualityComparer<T>.Default;

		foreach (var item in enumerable)
		{
			if (!comparer.Equals(item, element))
				yield return item;
		}
	}

	public static IEnumerable<T> ToEnumerable<T>(this T element)
	{
		yield return element;
	}
}