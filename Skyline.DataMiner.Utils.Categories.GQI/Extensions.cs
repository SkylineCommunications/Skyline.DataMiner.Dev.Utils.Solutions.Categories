namespace Skyline.DataMiner.Solutions.Categories.GQI
{
	using System;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Solutions.Categories.API;

	public static class Extensions
	{
		public static ICategoriesApi GetCategoriesApi(this GQIDMS gqiDms)
		{
			if (gqiDms is null)
			{
				throw new ArgumentNullException(nameof(gqiDms));
			}

			return gqiDms.GetConnection().GetCategoriesApi();
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
