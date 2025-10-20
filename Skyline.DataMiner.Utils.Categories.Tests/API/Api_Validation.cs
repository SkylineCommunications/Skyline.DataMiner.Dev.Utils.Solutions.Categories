namespace Skyline.DataMiner.Utils.Categories.Tests.API
{
	using System;

	using Skyline.DataMiner.Utils.Categories.API.Objects;

	[TestClass]
	public sealed class Api_Validation
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
			Assert.AreEqual("One or more scopes are still in use", ex.Message);
		}

		[TestMethod]
		public void Api_Validation_Categories_CheckDuplicates()
		{
			var api = new CategoriesApiMock();

			// doesn't throw exception
			var category = new Category { Name = "Category 1" };
			api.Categories.Create(category);

			category.Name = "Category 2";
			api.Categories.Update(category);

			// create item with same name
			var ex = Assert.Throws<InvalidOperationException>(
				() => { api.Categories.Create(new Category { Name = "Category 2" }); });
			Assert.AreEqual("Cannot save categories. The following names are already in use: Category 2", ex.Message);
		}
	}
}
