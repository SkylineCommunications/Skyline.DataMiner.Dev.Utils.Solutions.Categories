namespace Skyline.DataMiner.Utils.Categories.Tests
{
	using Skyline.DataMiner.Utils.Categories.API;
	using Skyline.DataMiner.Utils.DOM.UnitTesting;

	public class CategoriesApiMock : CategoriesApi
	{
		public CategoriesApiMock() : base(new DomConnectionMock())
		{
		}
	}
}
