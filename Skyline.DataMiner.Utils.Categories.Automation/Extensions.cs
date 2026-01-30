namespace Skyline.DataMiner.Solutions.Categories.Automation
{
	using System;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Solutions.Categories.API;

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

		// TODO: Check if we need interface here as well
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
