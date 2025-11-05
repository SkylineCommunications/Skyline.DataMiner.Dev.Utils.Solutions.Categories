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

			cache.GetCategoriesForScope("Scope 1").Should().BeEquivalentTo([category1]);
			cache.GetCategoriesForScope("Scope 2").Should().BeEquivalentTo([category2]);
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
		public void Api_Caching_GetChildItems()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			var category1 = new Category { Name = "Category 1", Scope = scope };
			api.Categories.CreateOrUpdate([category1]);

			var item11 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 1", Category = category1 };
			var item12 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 2", Category = category1 };
			api.CategoryItems.CreateOrUpdate([item11, item12]);

			var cache = new CategoriesCache();
			cache.LoadInitialData(api);

			cache.GetChildItems(category1).Should().BeEquivalentTo([item11, item12]);
		}

		[TestMethod]
		public void Api_Caching_GetDescendantItems()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			// Create a category hierarchy: Category 1 -> Category 1.1 -> Category 1.1.1
			var category1 = new Category { Name = "Category 1", Scope = scope };
			var category11 = new Category { Name = "Category 1.1", Scope = scope, ParentCategory = category1, RootCategory = category1 };
			var category111 = new Category { Name = "Category 1.1.1", Scope = scope, ParentCategory = category11, RootCategory = category1 };
			api.Categories.CreateOrUpdate([category1, category11, category111]);

			// Create items at different levels
			var item1 = new CategoryItem { ModuleId = "My Module", InstanceId = "Instance 1", Category = category1 };
			var item11 = new CategoryItem { ModuleId = "My Module", InstanceId = "Instance 1.1", Category = category11 };
			var item111 = new CategoryItem { ModuleId = "My Module", InstanceId = "Instance 1.1.1", Category = category111 };
			api.CategoryItems.CreateOrUpdate([item1, item11, item111]);

			var cache = new CategoriesCache();
			cache.LoadInitialData(api);

			// GetDescendantItems should return ALL items from category1 and its descendants
			cache.GetDescendantItems(category1).Should().BeEquivalentTo([item1, item11, item111]);

			// GetDescendantItems on category11 should only return items from category11 and its descendants
			cache.GetDescendantItems(category11).Should().BeEquivalentTo([item11, item111]);

			// GetDescendantItems on leaf category should only return its own items
			cache.GetDescendantItems(category111).Should().BeEquivalentTo([item111]);
		}

		[TestMethod]
		public void Api_Caching_GetSubtree()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			var category1 = new Category { Name = "Category 1", Scope = scope };
			var category11 = new Category { Name = "Category 1.1", Scope = scope, ParentCategory = category1, RootCategory = category1 };
			var category12 = new Category { Name = "Category 1.2", Scope = scope, ParentCategory = category1, RootCategory = category1 };
			api.Categories.CreateOrUpdate([category1, category11, category12]);

			var item11 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 1", Category = category1 };
			var item111 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 11", Category = category11 };
			var item112 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 12", Category = category11 };
			api.CategoryItems.CreateOrUpdate([item11, item111, item112]);

			var cache = new CategoriesCache();
			cache.LoadInitialData(api);

			var subtree = cache.GetSubtree(category1);

			var expectedSubtree = new CategoryNode(
				category1,
				[
					new CategoryNode(
						category11,
						[],
						[item111, item112]
					),
					new CategoryNode(
						category12,
						[],
						[]
					)
				],
				[item11]);

			subtree.Should().BeEquivalentTo(expectedSubtree);
			subtree.GetDescendantCategories().Select(x => x.Category).Should().BeEquivalentTo([category11, category12]);
			subtree.GetDescendantItems().Should().BeEquivalentTo([item11, item111, item112]);
		}

		[TestMethod]
		public void Api_Caching_GetAncestorPath()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			var category1 = new Category { Name = "Category 1", Scope = scope };
			var category11 = new Category { Name = "Category 1.1", Scope = scope, ParentCategory = category1, RootCategory = category1 };
			var category111 = new Category { Name = "Category 1.1.1", Scope = scope, ParentCategory = category11, RootCategory = category1 };
			api.Categories.CreateOrUpdate([category1, category11, category111]);

			var cache = new CategoriesCache();
			cache.LoadInitialData(api);

			cache.GetAncestorPath(category1).Should().BeEquivalentTo([category1], options => options.WithStrictOrdering());
			cache.GetAncestorPath(category11).Should().BeEquivalentTo([category1, category11], options => options.WithStrictOrdering());
			cache.GetAncestorPath(category111).Should().BeEquivalentTo([category1, category11, category111], options => options.WithStrictOrdering());
		}

		[TestMethod]
		public void Api_Caching_CategoryContainsItem()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			var category1 = new Category { Name = "Category 1", Scope = scope };
			var category11 = new Category { Name = "Category 1.1", Scope = scope, ParentCategory = category1, RootCategory = category1 };
			api.Categories.CreateOrUpdate([category1, category11]);

			var item11 = new CategoryItemIdentifier( "My Module", "My Instance 1");
			api.Categories.AddChildItems(category11, [item11]);

			var cache = new CategoriesCache();
			cache.LoadInitialData(api);

			cache.CategoryContainsItem(category1, item11).Should().BeFalse();
			cache.CategoryContainsItem(category11, item11).Should().BeTrue();

			cache.CategoryContainsDescendantItem(category1, item11).Should().BeTrue();
			cache.CategoryContainsDescendantItem(category11, item11).Should().BeTrue();

			var itemNotInCategory = new CategoryItemIdentifier("My Module", "Nonexistent Instance");
			cache.CategoryContainsItem(category1, itemNotInCategory).Should().BeFalse();
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
