namespace Skyline.DataMiner.Utils.Categories.Tests.API
{
	using FluentAssertions;

	using Skyline.DataMiner.Utils.Categories.API.Objects;

	[TestClass]
	public sealed class Api_Scopes
	{
		[TestMethod]
		public void Api_Scopes_GetCategoriesTree()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			var scope2 = new Scope { Name = "Scope 2" };
			api.Scopes.CreateOrUpdate([scope1, scope2]);

			var category1 = new Category { Name = "Category 1", Scope = scope1 };
			var category11 = new Category { Name = "Category 1.1", Scope = scope1, ParentCategory = category1, RootCategory = category1 };
			var category12 = new Category { Name = "Category 1.2", Scope = scope1, ParentCategory = category1, RootCategory = category1 };
			api.Categories.CreateOrUpdate([category1, category11, category12]);

			scope1.GetCategoriesTree(api.Categories).Should().BeEquivalentTo(
				new CategoryWithChildren(
					category1,
					[
						new CategoryWithChildren(category11, []),
						new CategoryWithChildren(category12, []),
					]));

			scope2.GetCategoriesTree(api.Categories).Should().BeEquivalentTo(
				new CategoryWithChildren(Category.DefaultRootCategory, []));
		}
	}
}
