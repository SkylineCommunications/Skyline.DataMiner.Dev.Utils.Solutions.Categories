namespace Skyline.DataMiner.Utils.Categories.API
{
	using System;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Utils.Categories.API.Repositories;
	using Skyline.DataMiner.Utils.Categories.DOM.Helpers;

	public class CategoriesApi
	{
		public CategoriesApi(IConnection connection)
		{
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));

			SlcCategoriesHelper = new SlcCategoriesHelper(connection);

			Categories = new CategoryRepository(SlcCategoriesHelper, connection);
		}

		protected internal IConnection Connection { get; }

		internal SlcCategoriesHelper SlcCategoriesHelper { get; }

		public CategoryRepository Categories { get; }
	}
}
