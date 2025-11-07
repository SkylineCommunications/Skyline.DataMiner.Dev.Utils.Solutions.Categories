namespace Skyline.DataMiner.Utils.Categories.Tests.API
{
	using FluentAssertions;

	using Skyline.DataMiner.Utils.Categories.API.Objects;

	[TestClass]
	public sealed class Api_CategoryItems
	{
		[TestMethod]
		public void Api_CategoryItems_GetChildItems()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			var category1 = new Category { Name = "Category 1", Scope = scope };
			api.Categories.CreateOrUpdate([category1]);

			var item11 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 1", Category = category1 };
			var item12 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 2", Category = category1 };
			api.CategoryItems.CreateOrUpdate([item11, item12]);

			api.CategoryItems.GetChildItems(category1).Should().BeEquivalentTo([item11, item12]);

			// other way to do the same
			category1.GetChildItems(api.CategoryItems).Should().BeEquivalentTo([item11, item12]);
		}

		[TestMethod]
		public void Api_CategoryItems_ReplaceChildItems()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			var category1 = new Category { Name = "Category 1", Scope = scope };
			api.Categories.CreateOrUpdate([category1]);

			// Initial items
			var item11 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 1" };
			var item12 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 2" };
			api.CategoryItems.ReplaceChildItems(category1, [item11, item12]);

			api.CategoryItems.GetChildItems(category1).Should().BeEquivalentTo([item11, item12]);

			// Update items: remove item11, update item12, add item13
			var item12Updated = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 2 updated" };
			var item13 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 3" };
			api.CategoryItems.ReplaceChildItems(category1, [item12Updated, item13]);

			api.CategoryItems.GetChildItems(category1).Should().BeEquivalentTo([item12Updated, item13]);
		}

		[TestMethod]
		public void Api_CategoryItems_AddChildItems()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			var category1 = new Category { Name = "Category 1", Scope = scope };
			api.Categories.CreateOrUpdate([category1]);

			// Initial items
			var item11 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 1" };
			var item12 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 2" };
			api.CategoryItems.AddChildItems(category1, [item11, item12]);

			api.CategoryItems.GetChildItems(category1).Should().BeEquivalentTo([item11, item12]);

			// Add more items
			var item13 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 3" };
			api.CategoryItems.AddChildItems(category1, [item13]);

			api.CategoryItems.GetChildItems(category1).Should().BeEquivalentTo([item11, item12, item13]);
		}

		[TestMethod]
		public void Api_CategoryItems_RemoveChildItems()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			var category1 = new Category { Name = "Category 1", Scope = scope };
			api.Categories.CreateOrUpdate([category1]);

			// Initial items
			var item11 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 1" };
			var item12 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 2" };
			var item13 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 3" };
			api.CategoryItems.AddChildItems(category1, [item11, item12, item13]);

			api.CategoryItems.GetChildItems(category1).Should().BeEquivalentTo([item11, item12, item13]);

			// Remove item12
			api.CategoryItems.RemoveChildItems(category1, [item12]);
			api.CategoryItems.GetChildItems(category1).Should().BeEquivalentTo([item11, item13]);

			// Remove all items
			api.CategoryItems.ClearChildItems(category1);
			api.CategoryItems.GetChildItems(category1).Should().BeEmpty();
		}
	}
}
