namespace Skyline.DataMiner.Solutions.Categories.Tests.API
{
	using FluentAssertions;

	using Skyline.DataMiner.Solutions.Categories.API;
	using Skyline.DataMiner.Solutions.Categories.Tools;
	using Skyline.DataMiner.Utils.DOM.UnitTesting;

	[TestClass]
	public sealed class Api_Generic
	{
		[TestMethod]
		public void Api_IsInstalled()
		{
			var api = new CategoriesApiMock(installDomModules: false);

			Assert.IsFalse(api.IsInstalled());

			api.InstallDomModules();

			Assert.IsTrue(api.IsInstalled());
		}

		[TestMethod]
		public void Api_Version()
		{
			var api = new CategoriesApiMock();
			api.IsInstalled(out var version);

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
