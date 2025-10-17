namespace Skyline.DataMiner.Utils.Categories.Tests.API
{
	using FluentAssertions;

	using Skyline.DataMiner.Utils.Categories.API;
	using Skyline.DataMiner.Utils.Categories.DOM.Definitions;
	using Skyline.DataMiner.Utils.Categories.DOM.Tools;
	using Skyline.DataMiner.Utils.Categories.Tools;
	using Skyline.DataMiner.Utils.Categories.UnitTesting;
	using Skyline.DataMiner.Utils.DOM.UnitTesting;

	[TestClass]
	public sealed class Api_Generic
	{
		[TestMethod]
		public void Api_IsInstalled()
		{
			var api = new CategoriesApiMock();

			Assert.IsFalse(api.IsInstalled());

			var domModule = new SlcCategoriesDomModule();
			DomModuleInstaller.Install(api.Connection.HandleMessages, domModule, x => { });

			Assert.IsTrue(api.IsInstalled());
		}

		[TestMethod]
		public void Api_Version()
		{
			var api = new CategoriesApiMock();
			var version = api.GetVersion();

			version.Should().NotBeNullOrEmpty();
		}

		[TestMethod]
		public void Api_ConstructorDoesNotExecuteRequest()
		{
			var connection = new DomConnectionMock();

			var interceptedConnection = new ConnectionInterceptor(connection);

			using (var connectionMetrics = new ConnectionMetrics(interceptedConnection))
			{
				new CategoriesApi(interceptedConnection);

				// constructor should not execute any requests
				Assert.AreEqual(0UL, connectionMetrics.NumberOfRequests);
				Assert.AreEqual(0UL, connectionMetrics.NumberOfDomRequests);
				Assert.AreEqual(0UL, connectionMetrics.NumberOfDomInstancesRetrieved);
			}
		}
	}
}
