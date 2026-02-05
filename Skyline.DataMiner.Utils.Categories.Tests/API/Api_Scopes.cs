namespace Skyline.DataMiner.Solutions.Categories.Tests.API
{
	using FluentAssertions;

	using Skyline.DataMiner.Solutions.Categories.API;

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
			var category11 = new Category { Name = "Category 1.1", Scope = scope1, ParentCategory = category1 };
			var category12 = new Category { Name = "Category 1.2", Scope = scope1, ParentCategory = category1 };
			var category121 = new Category { Name = "Category 1.2.1", Scope = scope1, ParentCategory = category12 };
			api.Categories.CreateOrUpdate([category1, category11, category12, category121]);

			var scope1Tree = scope1.GetCategoriesTree(api.Categories);
			var scope2Tree = scope2.GetCategoriesTree(api.Categories);

			scope1Tree.Should().BeEquivalentTo(
				new CategoryNode(
					category1,
					[
						new CategoryNode(category11),
						new CategoryNode(category12,
						[
							new CategoryNode(category121),
						]),
					]));

			scope2Tree.Should().BeEquivalentTo(
				new CategoryNode(Category.DefaultRootCategory));
		}

		[TestMethod]
		public void Api_Scopes_DeleteStillInUse()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope1]);

			var category1 = new Category { Name = "Category 1", Scope = scope1 };
			api.Categories.CreateOrUpdate([category1]);

			var ex = Assert.Throws<InvalidOperationException>(
				() => { api.Scopes.Delete(scope1); });
			Assert.AreEqual("Cannot delete scopes: one or more scopes are still in use: Scope 1", ex.Message);
		}
	}
}
