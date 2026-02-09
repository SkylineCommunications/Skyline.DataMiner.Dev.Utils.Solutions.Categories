namespace Skyline.DataMiner.Solutions.Categories
{
	using System;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Solutions.Categories.API;

	public static class IConnectionExtensions
	{
		public static ICategoriesApi GetCategoriesApi(this IConnection connection)
		{
			if (connection is null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			return new CategoriesApi(connection);
		}

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
