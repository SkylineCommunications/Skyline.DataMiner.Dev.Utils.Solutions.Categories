namespace Skyline.DataMiner.Solutions.Categories.API
{
	using System;

	using Skyline.DataMiner.Net;

	internal static class ConnectionExtensions
	{
		public static CategoriesApi GetCategoriesApi(this IConnection connection)
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
