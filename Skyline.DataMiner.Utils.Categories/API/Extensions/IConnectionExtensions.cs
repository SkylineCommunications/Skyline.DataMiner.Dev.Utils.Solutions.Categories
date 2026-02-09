namespace Skyline.DataMiner.Solutions.Categories.API
{
	using System;

	using Skyline.DataMiner.Net;

	/// <summary>
	/// Provides extension methods for the <see cref="IConnection"/> interface to work with categories.
	/// </summary>
	public static class IConnectionExtensions
	{
		/// <summary>
		/// Creates a new <see cref="ICategoriesApi"/> instance for the specified <see cref="IConnection"/>.
		/// </summary>
		/// <param name="connection">The connection for which the categories API should be created.</param>
		/// <returns>An <see cref="ICategoriesApi"/> instance associated with the specified connection.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="connection"/> is <c>null</c>.</exception>
		public static ICategoriesApi GetCategoriesApi(this IConnection connection)
		{
			if (connection is null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			return new CategoriesApi(connection);
		}

		/// <summary>
		/// Gets a shared <see cref="StaticCategoriesCache"/> instance for the specified <see cref="IConnection"/>,
		/// creating it if it does not already exist.
		/// </summary>
		/// <param name="connection">The connection for which the static categories cache should be retrieved.</param>
		/// <returns>A <see cref="StaticCategoriesCache"/> instance associated with the specified connection.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="connection"/> is <c>null</c>.</exception>
		public static StaticCategoriesCache GetStaticCategoriesCache(this IConnection connection)
		{
			if (connection is null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			return StaticCategoriesCache.GetOrCreate(connection);
		}
	}
}
