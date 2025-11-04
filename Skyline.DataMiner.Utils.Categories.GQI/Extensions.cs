namespace Skyline.DataMiner.Utils.Categories.GQI
{
	using System;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Utils.Categories.API;
	using Skyline.DataMiner.Utils.Categories.API.Caching;

	public static class Extensions
	{
		public static CategoriesApi GetCategoriesApi(this GQIDMS gqiDms)
		{
			if (gqiDms is null)
			{
				throw new ArgumentNullException(nameof(gqiDms));
			}

			return new CategoriesApi(gqiDms.GetConnection());
		}

		public static StaticCategoriesCache GetStaticCategoriesCache(this GQIDMS gqiDms)
		{
			if (gqiDms is null)
			{
				throw new ArgumentNullException(nameof(gqiDms));
			}

			return StaticCategoriesCache.GetOrCreate(gqiDms.GetConnection);
		}
	}
}
