namespace Skyline.DataMiner.Utils.Categories.Protocol
{
	using System;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Categories.API;
	using Skyline.DataMiner.Utils.Categories.API.Caching;

	public static class Extensions
	{
		public static CategoriesApi GetCategoriesApi(this SLProtocol protocol)
		{
			if (protocol is null)
			{
				throw new ArgumentNullException(nameof(protocol));
			}

			return new CategoriesApi(protocol.GetUserConnection());
		}

		public static StaticCategoriesCache GetStaticCategoriesCache(this SLProtocol protocol)
		{
			if (protocol is null)
			{
				throw new ArgumentNullException(nameof(protocol));
			}

			return StaticCategoriesCache.GetOrCreate(protocol.GetUserConnection);
		}
	}
}
