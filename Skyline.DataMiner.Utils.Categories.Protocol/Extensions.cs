namespace Skyline.DataMiner.Solutions.Categories.Protocol
{
	using System;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Solutions.Categories.API;

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

		// TODO: Check if we need interface
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
