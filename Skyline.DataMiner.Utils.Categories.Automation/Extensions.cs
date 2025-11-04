namespace Skyline.DataMiner.Utils.Categories.Automation
{
	using System;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.Categories.API;
	using Skyline.DataMiner.Utils.Categories.API.Caching;

	public static class Extensions
	{
		public static CategoriesApi GetCategoriesApi(this IEngine engine)
		{
			if (engine is null)
			{
				throw new ArgumentNullException(nameof(engine));
			}

			return new CategoriesApi(engine.GetUserConnection());
		}

		public static StaticCategoriesCache GetStaticCategoriesCache(this IEngine engine)
		{
			if (engine is null)
			{
				throw new ArgumentNullException(nameof(engine));
			}

			return StaticCategoriesCache.GetOrCreate(engine.GetUserConnection);
		}
	}
}
