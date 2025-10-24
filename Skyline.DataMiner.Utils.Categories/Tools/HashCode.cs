namespace Skyline.DataMiner.Utils.Categories.Tools
{
	using System.Collections.Generic;

	public static class HashCode
	{
		/// <summary>
		/// Computes an order-independent hash code for a collection of items.
		/// </summary>
		/// <typeparam name="T">Type of the items.</typeparam>
		/// <param name="items">The items to hash.</param>
		/// <param name="comparer">Optional equality comparer (default is <see cref="EqualityComparer{T}.Default"/>).</param>
		/// <returns>An order-independent hash code.</returns>
		public static int GetOrderIndependentHashCode<T>(this IEnumerable<T> items, IEqualityComparer<T> comparer = null)
		{
			if (items is null)
			{
				return 0;
			}

			comparer ??= EqualityComparer<T>.Default;

			unchecked
			{
				int hash = 0;

				foreach (var item in items)
				{
					hash ^= comparer.GetHashCode(item);
				}

				return hash;
			}
		}
	}
}
