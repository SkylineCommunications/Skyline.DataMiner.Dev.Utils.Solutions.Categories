namespace Skyline.DataMiner.Solutions.Categories.Tests.API
{
	using FluentAssertions;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Solutions.Categories.API;

	using SLDataGateway.API.Querying;

	[TestClass]
	public sealed class Api_Categories
	{
		[TestMethod]
		public void Api_Categories_Exposers_ReadFromRoot()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope1]);

			var group1 = new Category { Name = "Group 1", Scope = scope1 };
			var category1_1 = new Category { Name = "Category 1", Scope = scope1, ParentCategory = group1 };
			var category1_2 = new Category { Name = "Category 1", Scope = scope1 };

			api.Categories.CreateOrUpdate([group1, category1_1, category1_2]);

			api.Categories.Read(CategoryExposers.Name.Equal("Category 1").AND(CategoryExposers.Scope.Equal(scope1)).AND(CategoryExposers.ParentCategory.Equal(null))).Should().BeEquivalentTo([category1_2]);
		}

		[TestMethod]
		public void Api_Categories_Exposers_ReadFromParent()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope1]);

			var group1 = new Category { Name = "Group 1", Scope = scope1 };
			var group2 = new Category { Name = "Group 2", Scope = scope1 };
			var category1_1 = new Category { Name = "Category 1", Scope = scope1, ParentCategory = group1 };
			var category1_2 = new Category { Name = "Category 1", Scope = scope1, ParentCategory = group2 };

			api.Categories.CreateOrUpdate([group1, group2, category1_1, category1_2]);

			api.Categories.Read(CategoryExposers.Name.Equal("Category 1").AND(CategoryExposers.Scope.Equal(scope1)).AND(CategoryExposers.ParentCategory.Equal(group1))).SingleOrDefault().Should().BeEquivalentTo(category1_1);
		}

		[TestMethod]
		public void Api_Categories_Query()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			var scope2 = new Scope { Name = "Scope 2" };
			api.Scopes.CreateOrUpdate([scope1, scope2]);

			var category1_1 = new Category { Name = "Category 1", Scope = scope1 };
			var category1_2 = new Category { Name = "Category 1", Scope = scope2 };
			api.Categories.CreateOrUpdate([category1_1, category1_2]);

			api.Categories.Query().Single(x => x.Name == "Category 1" && x.Scope == scope2)
				.Should().Be(category1_2);
		}

		[TestMethod]
		public void Api_Categories_ReadByName()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			var scope2 = new Scope { Name = "Scope 2" };
			api.Scopes.CreateOrUpdate([scope1, scope2]);

			var category1_1 = new Category { Name = "Category 1", Scope = scope1 };
			var category1_2 = new Category { Name = "Category 1", Scope = scope2 };
			api.Categories.CreateOrUpdate([category1_1, category1_2]);

			api.Categories.Read("Category 1").Should().NotBeNull();
		}

		[TestMethod]
		public void Api_Categories_CreateWithDuplicateNameInSameScopeAndRoot()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope1]);

			var category1_1 = new Category { Name = "Category 1", Scope = scope1 };
			var category1_2 = new Category { Name = "Category 1", Scope = scope1 };

			try
			{
				api.Categories.CreateOrUpdate([category1_1, category1_2]);

			}
			catch (InvalidOperationException exception)
			{
				exception.Message.Should().Contain("The following names are already in use: Category 1");
				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		public void Api_Categories_CreateWithDuplicateNameInSameScopeAndSameParent()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope1]);

			var group1 = new Category { Name = "Group 1", Scope = scope1 };
			var category1_1 = new Category { Name = "Category 1", Scope = scope1, ParentCategory = group1 };
			var category1_2 = new Category { Name = "Category 1", Scope = scope1, ParentCategory = group1 };

			try
			{
				api.Categories.CreateOrUpdate([group1, category1_1, category1_2]);

			}
			catch (InvalidOperationException exception)
			{
				exception.Message.Should().Contain("The following names are already in use: Category 1");
				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		public void Api_Categories_CreateWithDuplicateNameInSameScopeAndDifferentParent()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope1]);

			var group1 = new Category { Name = "Group 1", Scope = scope1 };
			var group2 = new Category { Name = "Group 2", Scope = scope1 };
			var category1_1 = new Category { Name = "Category 1", Scope = scope1, ParentCategory = group1 };
			var category1_2 = new Category { Name = "Category 1", Scope = scope1, ParentCategory = group2 };

			api.Categories.CreateOrUpdate([group1, group2, category1_1, category1_2]);
		}

		[TestMethod]
		public void Api_Categories_ReadWithEmptyIdArray()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope1]);

			var category1_1 = new Category { Name = "Category 1.1", Scope = scope1 };
			var category1_2 = new Category { Name = "Category 1.2", Scope = scope1 };
			api.Categories.CreateOrUpdate([category1_1, category1_2]);

			CategoryRepository categoriesRepository = (CategoryRepository)api.Categories;
			categoriesRepository.Read(Enumerable.Empty<Guid>()).Should().BeEmpty();
		}

		[TestMethod]
		public void Api_Categories_ReadWithEmptyNameArray()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope1]);

			var category1_1 = new Category { Name = "Category 1.1", Scope = scope1 };
			var category1_2 = new Category { Name = "Category 1.2", Scope = scope1 };
			api.Categories.CreateOrUpdate([category1_1, category1_2]);

			CategoryRepository categoriesRepository = (CategoryRepository)api.Categories;
			categoriesRepository.Read(Enumerable.Empty<string>()).Should().BeEmpty();
		}

		[TestMethod]
		public void Api_Categories_ReadWithEmptyFilter()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope1]);

			var category1_1 = new Category { Name = "Category 1.1", Scope = scope1 };
			var category1_2 = new Category { Name = "Category 1.2", Scope = scope1 };
			api.Categories.CreateOrUpdate([category1_1, category1_2]);

			api.Categories.Read(new ORFilterElement<Category>()).Should().BeEmpty();
		}

		[TestMethod]
		public void Api_Categories_ReadWithEmptyQuery()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope1]);

			var category1_1 = new Category { Name = "Category 1.1", Scope = scope1 };
			var category1_2 = new Category { Name = "Category 1.2", Scope = scope1 };
			api.Categories.CreateOrUpdate([category1_1, category1_2]);

			api.Categories.Read(new ORFilterElement<Category>().ToQuery()).Should().BeEmpty();
		}

		[TestMethod]
		public void Api_Categories_ReadPagedWithEmptyFilter()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope1]);

			var category1_1 = new Category { Name = "Category 1.1", Scope = scope1 };
			var category1_2 = new Category { Name = "Category 1.2", Scope = scope1 };
			api.Categories.CreateOrUpdate([category1_1, category1_2]);

			CategoryRepository categoriesRepository = (CategoryRepository)api.Categories;
			categoriesRepository.ReadPaged(new ORFilterElement<Category>()).Should().BeEmpty();
		}

		[TestMethod]
		public void Api_Categories_ReadPagedWithEmptyQuery()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope1]);

			var category1_1 = new Category { Name = "Category 1.1", Scope = scope1 };
			var category1_2 = new Category { Name = "Category 1.2", Scope = scope1 };
			api.Categories.CreateOrUpdate([category1_1, category1_2]);

			CategoryRepository categoriesRepository = (CategoryRepository)api.Categories;
			categoriesRepository.ReadPaged(new ORFilterElement<Category>().ToQuery()).Should().BeEmpty();
		}

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

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			var category1 = new Category { Name = "Category 1", Scope = scope };
			var category11 = new Category { Name = "Category 1.1", Scope = scope, ParentCategory = category1 };
			var category12 = new Category { Name = "Category 1.2", Scope = scope, ParentCategory = category1 };
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
			var category11 = new Category { Name = "Category 1.1", Scope = scope, ParentCategory = category1 };
			var category12 = new Category { Name = "Category 1.2", Scope = scope, ParentCategory = category1 };
			var category121 = new Category { Name = "Category 1.2.1", Scope = scope, ParentCategory = category12 };
			api.Categories.CreateOrUpdate([category1, category11, category12, category121]);

			api.Categories.GetDescendantCategories(category1).Should().BeEquivalentTo([category11, category12, category121]);

			// other way to do the same
			category1.GetDescendantCategories(api.Categories).Should().BeEquivalentTo([category11, category12, category121]);
		}

		[TestMethod]
		public void Api_Categories_GetAncestorPath()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			var category1 = new Category { Name = "Category 1", Scope = scope };
			var category11 = new Category { Name = "Category 1.1", Scope = scope, ParentCategory = category1 };
			var category111 = new Category { Name = "Category 1.1.1", Scope = scope, ParentCategory = category11 };
			api.Categories.CreateOrUpdate([category1, category11, category111]);

			api.Categories.GetAncestorPath(category1).Should().BeEquivalentTo([category1], options => options.WithStrictOrdering());
			api.Categories.GetAncestorPath(category11).Should().BeEquivalentTo([category1, category11], options => options.WithStrictOrdering());
			api.Categories.GetAncestorPath(category111).Should().BeEquivalentTo([category1, category11, category111], options => options.WithStrictOrdering());

			// other way to do the same
			category111.GetAncestorPath(api.Categories).Should().BeEquivalentTo([category1, category11, category111], options => options.WithStrictOrdering());
		}

		[TestMethod]
		public void Api_Categories_GetTree()
		{
			var api = new CategoriesApiMock();

			var scope1 = new Scope { Name = "Scope 1" };
			var scope2 = new Scope { Name = "Scope 2" };
			api.Scopes.CreateOrUpdate([scope1, scope2]);

			var category1 = new Category { Name = "Category 1", Scope = scope1 };
			var category11 = new Category { Name = "Category 1.1", Scope = scope1, ParentCategory = category1 };
			var category12 = new Category { Name = "Category 1.2", Scope = scope1, ParentCategory = category1 };
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

		[TestMethod]
		public void Api_Categories_DeleteAlsoRemovesChildItems()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			var category1 = new Category { Name = "Category 1", Scope = scope };
			var category2 = new Category { Name = "Category 2", Scope = scope };
			api.Categories.CreateOrUpdate([category1, category2]);

			var item11 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 1.1", Category = category1 };
			var item21 = new CategoryItem { ModuleId = "My Module", InstanceId = "My Instance 2.1", Category = category2 };
			api.CategoryItems.CreateOrUpdate([item11, item21]);

			api.Categories.Delete([category1, category2]);

			api.Categories.ReadAll().Should().BeEmpty();
			api.CategoryItems.ReadAll().Should().BeEmpty();
		}

		[TestMethod]
		public void Api_Categories_DeleteMovesChildCategoriesToRoot()
		{
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.CreateOrUpdate([scope]);

			var category1 = new Category { Name = "Category 1", Scope = scope };
			var category11 = new Category { Name = "Category 1.1", Scope = scope, ParentCategory = category1 };
			api.Categories.CreateOrUpdate([category1, category11]);

			api.Categories.Delete([category1]);

			category11 = api.Categories.Read(category11.ID);
			category11.ParentCategory.Should().BeNull();
		}
	}
}
