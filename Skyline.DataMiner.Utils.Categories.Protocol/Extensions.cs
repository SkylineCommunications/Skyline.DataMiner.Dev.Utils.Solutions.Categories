namespace Skyline.DataMiner.Solutions.Categories.Protocol
{
	using System;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Solutions.Categories.API;

	public static class Extensions
	{
		public static ICategoriesApi GetCategoriesApi(this SLProtocol protocol)
		{
			if (protocol is null)
			{
				throw new ArgumentNullException(nameof(protocol));
			}

			return protocol.GetUserConnection().GetCategoriesApi();
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
