namespace Skyline.DataMiner.Utils.Categories.API
{
	using System;

	using Skyline.DataMiner.Net;

	public class CategoriesApi
	{
		public CategoriesApi(IConnection connection)
		{
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));
		}

		protected internal IConnection Connection { get; }
	}
}
