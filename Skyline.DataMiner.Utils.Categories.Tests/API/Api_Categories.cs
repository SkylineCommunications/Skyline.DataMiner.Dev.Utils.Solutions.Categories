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
		public void Api_Categories_GetChildCategories()
		{
			var api = new CategoriesApiMock();

			var category1 = new Category { Name = "Category 1" };
			var category11 = new Category { Name = "Category 1.1", ParentCategory = category1 };
			var category12 = new Category { Name = "Category 1.2", ParentCategory = category1 };
			api.Categories.CreateOrUpdate([category1, category11, category12]);

			api.Categories.GetChildCategories(category1).Should().BeEquivalentTo([category11, category12]);

			// other way to do the same
			category1.GetChildCategories(api.Categories).Should().BeEquivalentTo([category11, category12]);
		}

		[TestMethod]
		public void Api_Categories_GetTree()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			var scope2 = new Scope { Name = "Scope 2" };
			api.Scopes.CreateOrUpdate([scope1, scope2]);

			var category1 = new Category { Name = "Category 1", Scope = scope1 };
			var category11 = new Category { Name = "Category 1.1", ParentCategory = category1, Scope = scope1 };
			var category12 = new Category { Name = "Category 1.2", ParentCategory = category1, Scope = scope1 };
			api.Categories.CreateOrUpdate([category1, category11, category12]);

			api.Categories.GetTree().Should().BeEquivalentTo(
				new CategoryWithChildren(
					category1,
					[
						new CategoryWithChildren(category11, []),
						new CategoryWithChildren(category12, []),
					]));

			api.Categories.GetTree(scope1).Should().BeEquivalentTo(
				new CategoryWithChildren(
					category1,
					[
						new CategoryWithChildren(category11, []),
						new CategoryWithChildren(category12, []),
					]));

			api.Categories.GetTree(scope2).Should().BeEquivalentTo(
				new CategoryWithChildren(Category.DefaultRootCategory, []));
		}
	}
}
