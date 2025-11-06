namespace Skyline.DataMiner.Utils.Categories.Tests
{
	using Skyline.DataMiner.Utils.Categories.API.Caching;

	[TestClass]
	[DoNotParallelize]
	public class TestBase
	{
		[TestInitialize]
		public void TestInitialize()
		{
			// Clear the cached data before each test
			StaticCategoriesCache.Reset();
		}
	}
}
