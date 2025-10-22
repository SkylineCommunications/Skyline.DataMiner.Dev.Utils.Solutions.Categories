namespace Skyline.DataMiner.Utils.Categories.Tests.API
{
	using FluentAssertions;

	using Skyline.DataMiner.Utils.Categories.API.Caching;
	using Skyline.DataMiner.Utils.Categories.API.Objects;
	using Skyline.DataMiner.Utils.Categories.API.Subscriptions;

	[TestClass]
	public sealed class Api_Caching
	{
		[TestMethod]
		public void Api_Caching_GetScopeByName()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope1]);

			var cache = new CategoriesCache();
			cache.LoadInitialData(api);

			cache.GetScope("Scope 1").Should().Be(scope1);
		}

		[TestMethod]
		public void Api_Caching_GetCategoriesForScope()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			var scope2 = new Scope { Name = "Scope 2" };
			api.Scopes.CreateOrUpdate([scope1, scope2]);

			var category1 = new Category { Name = "Category 1", Scope = scope1 };
			var category2 = new Category { Name = "Category 2", Scope = scope2 };
			api.Categories.CreateOrUpdate([category1, category2]);

			var cache = new CategoriesCache();
			cache.LoadInitialData(api);

			cache.GetCategoriesForScope(scope1).Should().BeEquivalentTo([category1]);
			cache.GetCategoriesForScope(scope2).Should().BeEquivalentTo([category2]);
		}

		[TestMethod]
		public void Api_Caching_GetChildCategories()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			var category1 = new Category { Name = "Category 1", Scope = scope };
			var category11 = new Category { Name = "Category 1.1", Scope = scope, ParentCategory = category1, RootCategory = category1 };
			var category12 = new Category { Name = "Category 1.2", Scope = scope, ParentCategory = category1, RootCategory = category1 };
			var category2 = new Category { Name = "Category 2", Scope = scope };
			var category21 = new Category { Name = "Category 2.1", Scope = scope, ParentCategory = category2, RootCategory = category2 };
			api.Categories.CreateOrUpdate([category1, category11, category12, category2, category21]);

			var cache = new CategoriesCache();
			cache.LoadInitialData(api);

			cache.GetChildCategories(category1).Should().BeEquivalentTo([category11, category12]);
			cache.GetChildCategories(category2).Should().BeEquivalentTo([category21]);
		}

		[TestMethod]
		public void Api_Caching_Subscription()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope1]);

			var category1 = new Category { Name = "Category 1", Scope = scope1 };
			api.Categories.CreateOrUpdate([category1]);

			var observer = new CategoriesObserver(api);
			var cache = observer.Cache;

			// Subscribe and load initial data
			observer.Subscribe();
			observer.LoadInitialData();

			// Verify initial data is loaded
			cache.GetScope("Scope 1").Should().Be(scope1);
			cache.GetCategoriesForScope(scope1).Should().BeEquivalentTo([category1]);

			// Add new scope and verify cache is updated automatically
			var scope2 = new Scope { Name = "Scope 2" };
			api.Scopes.CreateOrUpdate([scope2]);

			cache.TryGetScope("Scope 2", out var retrievedScope2).Should().BeTrue();
			retrievedScope2.Should().Be(scope2);

			// Add new category and verify cache is updated automatically
			var category2 = new Category { Name = "Category 2", Scope = scope2 };
			api.Categories.CreateOrUpdate([category2]);

			cache.GetCategoriesForScope(scope2).Should().BeEquivalentTo([category2]);

			// Test unsubscribe
			observer.Unsubscribe();

			// After unsubscribing, new data should not automatically update the cache
			var scope3 = new Scope { Name = "Scope 3" };
			api.Scopes.CreateOrUpdate([scope3]);

			cache.TryGetScope("Scope 3", out _).Should().BeFalse();
		}
	}
}
