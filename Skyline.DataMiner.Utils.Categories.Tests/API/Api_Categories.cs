namespace Skyline.DataMiner.Utils.Categories.Tests.API
{
	using FluentAssertions;

	using Skyline.DataMiner.Utils.Categories.API.Objects;

	[TestClass]
	public sealed class Api_Categories
	{
		[TestMethod]
		public void Api_Categories_GetByScope()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			var scope2 = new Scope { Name = "Scope 2" };
			api.Scopes.CreateOrUpdate([scope1, scope2]);

			var category1 = new Category { Name = "Category 1", Scope = scope1 };
			var category2 = new Category { Name = "Category 2", Scope = scope2 };
			api.Categories.CreateOrUpdate([category1, category2]);

			api.Categories.GetByScope(scope1).Should().BeEquivalentTo([category1]);
			api.Categories.GetByScope(scope2).Should().BeEquivalentTo([category2]);
		}

		[TestMethod]
		public void Api_Categories_GetByRootCategory()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			var rootCategory1 = new Category { Name = "Root Category 1", Scope = scope };
			var rootCategory2 = new Category { Name = "Root Category 2", Scope = scope };
			api.Categories.CreateOrUpdate([rootCategory1, rootCategory2]);

			var category11 = new Category { Name = "Category 1.1", Scope = scope, ParentCategory = rootCategory1, RootCategory = rootCategory1 };
			var category111 = new Category { Name = "Category 1.1.1", Scope = scope, ParentCategory = category11, RootCategory = rootCategory1 };
			var category12 = new Category { Name = "Category 1.2", Scope = scope, ParentCategory = rootCategory1, RootCategory = rootCategory1 };
			var category21 = new Category { Name = "Category 2.1", Scope = scope, ParentCategory = rootCategory2, RootCategory = rootCategory2 };
			api.Categories.CreateOrUpdate([category11, category111, category12, category21]);

			api.Categories.GetByRootCategory(rootCategory1).Should().BeEquivalentTo([category11, category111, category12]);
			api.Categories.GetByRootCategory(rootCategory2).Should().BeEquivalentTo([category21]);
		}

		[TestMethod]
		public void Api_Categories_GetChildCategories()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			var category1 = new Category { Name = "Category 1", Scope = scope };
			var category11 = new Category { Name = "Category 1.1", Scope = scope, ParentCategory = category1, RootCategory = category1 };
			var category12 = new Category { Name = "Category 1.2", Scope = scope, ParentCategory = category1, RootCategory = category1 };
			api.Categories.CreateOrUpdate([category1, category11, category12]);

			api.Categories.GetChildCategories(category1).Should().BeEquivalentTo([category11, category12]);

			// other way to do the same
			category1.GetChildCategories(api.Categories).Should().BeEquivalentTo([category11, category12]);
		}

		[TestMethod]
		public void Api_Categories_GetDescendantCategories()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			var category1 = new Category { Name = "Category 1", Scope = scope };
			var category11 = new Category { Name = "Category 1.1", Scope = scope, ParentCategory = category1, RootCategory = category1 };
			var category12 = new Category { Name = "Category 1.2", Scope = scope, ParentCategory = category1, RootCategory = category1 };
			var category121 = new Category { Name = "Category 1.2.1", Scope = scope, ParentCategory = category12, RootCategory = category1 };
			api.Categories.CreateOrUpdate([category1, category11, category12, category121]);

			api.Categories.GetDescendantCategories(category1).Should().BeEquivalentTo([category11, category12, category121]);

			// other way to do the same
			category1.GetDescendantCategories(api.Categories).Should().BeEquivalentTo([category11, category12, category121]);
		}

		[TestMethod]
		public void Api_Categories_GetTree()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			var scope2 = new Scope { Name = "Scope 2" };
			api.Scopes.CreateOrUpdate([scope1, scope2]);

			var category1 = new Category { Name = "Category 1", Scope = scope1 };
			var category11 = new Category { Name = "Category 1.1", Scope = scope1, ParentCategory = category1, RootCategory = category1 };
			var category12 = new Category { Name = "Category 1.2", Scope = scope1, ParentCategory = category1, RootCategory = category1 };
			api.Categories.CreateOrUpdate([category1, category11, category12]);

			api.Categories.GetTree().Should().BeEquivalentTo(
				new CategoryNode(
					category1,
					[
						new CategoryNode(category11),
						new CategoryNode(category12),
					]));

			api.Categories.GetTree(scope1).Should().BeEquivalentTo(
				new CategoryNode(
					category1,
					[
						new CategoryNode(category11),
						new CategoryNode(category12),
					]));

			api.Categories.GetTree(scope2).Should().BeEquivalentTo(
				new CategoryNode(Category.DefaultRootCategory));
		}
	}
}
