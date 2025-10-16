namespace Skyline.DataMiner.Utils.Categories.Automation
{
	using System;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.Categories.API;

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
	}
}
