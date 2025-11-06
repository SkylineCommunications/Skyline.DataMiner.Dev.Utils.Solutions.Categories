namespace Skyline.DataMiner.Utils.Categories.Tests.API
{
	using System;

	using Skyline.DataMiner.Utils.Categories.API.Objects;

	[TestClass]
	public sealed class Api_Validation : TestBase
	{
		[TestMethod]
		public void Api_Validation_Scopes_CheckDuplicates()
		{
			var api = new CategoriesApiMock();

			// doesn't throw exception
			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.Create(scope);

			scope.Name = "Scope 2";
			api.Scopes.Update(scope);

			// create item with same name
			var ex = Assert.Throws<InvalidOperationException>(
				() => { api.Scopes.Create(new Scope { Name = "Scope 2" }); });
			Assert.AreEqual("Cannot save scopes. The following names are already in use: Scope 2", ex.Message);
		}

		[TestMethod]
		public void Api_Validation_Scopes_CheckStillInUse()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.Create(scope);

			var category = new Category { Name = "Category 1", Scope = scope };
			api.Categories.Create(category);

			// deleting scope that is still in use throws exception
			var ex = Assert.Throws<InvalidOperationException>(
				() => { api.Scopes.Delete(scope); });
			Assert.AreEqual("Cannot delete scopes: One or more scopes are still in use: Scope 1", ex.Message);

			// deleting scope that is not in use doesn't throw exception
			api.Categories.Delete(category);
			api.Scopes.Delete(scope);
		}

		[TestMethod]
		public void Api_Validation_Categories_CheckDuplicates()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			// doesn't throw exception
			var category1 = new Category { Name = "Category 1", Scope = scope };
			var category2 = new Category { Name = "Category 2", Scope = scope };
			api.Categories.CreateOrUpdate([category1, category2]);

			// create item with same name
			var ex = Assert.Throws<InvalidOperationException>(
				() => { api.Categories.Create(new Category { Name = "Category 2", Scope = scope }); });
			Assert.AreEqual("Cannot save categories. The following names are already in use: Category 2", ex.Message);

			// update category1 to have same name as category2
			category1.Name = "Category 2";
			ex = Assert.Throws<InvalidOperationException>(
				() => { api.Categories.Update(category1); });
			Assert.AreEqual("Cannot save categories. The following names are already in use: Category 2", ex.Message);

			// create category with same name with a different parent - should be allowed
			var category3 = new Category { Name = "Category 2", Scope = scope, ParentCategory = category2, RootCategory = category2 };
			api.Categories.Create(category3);
		}
	}
}
