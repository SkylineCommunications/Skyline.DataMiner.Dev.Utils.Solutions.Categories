namespace Skyline.DataMiner.Utils.Categories.API.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.Categories.API.Objects;
	using Skyline.DataMiner.Utils.Categories.API.Tools;
	using Skyline.DataMiner.Utils.Categories.DOM.Helpers;
	using Skyline.DataMiner.Utils.Categories.DOM.Model;
	using Skyline.DataMiner.Utils.Categories.DOM.Tools;

	using SLDataGateway.API.Types.Querying;

	public class CategoryItemRepository : Repository<CategoryItem>
	{
		internal CategoryItemRepository(SlcCategoriesHelper helper, IConnection connection) : base(helper, connection)
		{
		}

		protected internal override DomDefinitionId DomDefinition => CategoryItem.DomDefinition;

		public IEnumerable<CategoryItem> GetChildItems(ApiObjectReference<Category> parentCategory)
		{
			if (parentCategory == ApiObjectReference<Category>.Empty)
			{
				return [];
			}

			var filter = new ANDFilterElement<DomInstance>(
				DomInstanceExposers.DomDefinitionId.Equal(SlcCategoriesIds.Definitions.CategoryItem.Id),
				DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.CategoryItemInfo.Category).Equal(parentCategory.ID));

			return Read(filter);
		}

		public IEnumerable<CategoryItem> GetChildItems(IEnumerable<ApiObjectReference<Category>> parentCategories)
		{
			static FilterElement<DomInstance> CreateFilter(ApiObjectReference<Category> parentCategory) =>
				new ANDFilterElement<DomInstance>(
					DomInstanceExposers.DomDefinitionId.Equal(SlcCategoriesIds.Definitions.CategoryItem.Id),
					DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.CategoryItemInfo.Category).Equal(parentCategory.ID));

			return FilterQueryExecutor.RetrieveFilteredItems(parentCategories, CreateFilter, Read);
		}

		public void ReplaceChildItems(ApiObjectReference<Category> category, ICollection<CategoryItem> newItems)
		{
			if (category == ApiObjectReference<Category>.Empty)
			{
				throw new ArgumentException("Category reference cannot be empty.", nameof(category));
			}

			if (newItems == null)
			{
				throw new ArgumentNullException(nameof(newItems));
			}

			foreach (var item in newItems.Where(x => x.Category == ApiObjectReference<Category>.Empty))
			{
				item.Category = category;
			}

			if (newItems.Any(x => x.Category != category))
			{
				throw new InvalidOperationException("All new items must belong to the specified category.");
			}

			var newItemsLookup = newItems.ToLookup(item => (item.ModuleId, item.InstanceId));

			var existingItems = GetChildItems(category).ToList();
			var itemsToDelete = existingItems.Where(item => !newItemsLookup.Contains((item.ModuleId, item.InstanceId))).ToList();

			Delete(itemsToDelete);
			CreateOrUpdate(newItems);
		}

		public void AddChildItems(ApiObjectReference<Category> category, ICollection<CategoryItem> itemsToAdd)
		{
			if (category == ApiObjectReference<Category>.Empty)
			{
				throw new ArgumentException("Category reference cannot be empty.", nameof(category));
			}

			if (itemsToAdd == null)
			{
				throw new ArgumentNullException(nameof(itemsToAdd));
			}

			foreach (var item in itemsToAdd.Where(x => x.Category == ApiObjectReference<Category>.Empty))
			{
				item.Category = category;
			}

			if (itemsToAdd.Any(x => x.Category != category))
			{
				throw new InvalidOperationException("All items to add must belong to the specified category.");
			}

			// Get existing items to avoid duplicates
			var existingItems = GetChildItems(category).ToList();
			var existingItemsLookup = existingItems.ToLookup(item => (item.ModuleId, item.InstanceId));

			// Filter out items that already exist in the category
			var newItemsToAdd = itemsToAdd.Where(item => !existingItemsLookup.Contains((item.ModuleId, item.InstanceId))).ToList();

			if (newItemsToAdd.Any())
			{
				CreateOrUpdate(newItemsToAdd);
			}
		}

		public void RemoveChildItems(ApiObjectReference<Category> category, ICollection<CategoryItem> itemsToDelete)
		{
			if (category == ApiObjectReference<Category>.Empty)
			{
				throw new ArgumentException("Category reference cannot be empty.", nameof(category));
			}

			if (itemsToDelete == null)
			{
				throw new ArgumentNullException(nameof(itemsToDelete));
			}

			foreach (var item in itemsToDelete.Where(x => x.Category == ApiObjectReference<Category>.Empty))
			{
				item.Category = category;
			}

			if (itemsToDelete.Any(x => x.Category != category))
			{
				throw new InvalidOperationException("All items to delete must belong to the specified category.");
			}

			Delete(itemsToDelete);
		}

		public void ClearChildItems(ApiObjectReference<Category> category)
		{
			if (category == ApiObjectReference<Category>.Empty)
			{
				throw new ArgumentException("Category reference cannot be empty.", nameof(category));
			}

			var existingItems = GetChildItems(category).ToList();
			Delete(existingItems);
		}

		protected internal override CategoryItem CreateInstance(DomInstance domInstance)
		{
			return new CategoryItem(domInstance);
		}

		protected override void ValidateBeforeSave(ICollection<CategoryItem> instances)
		{
			// no checks needed
		}

		protected override void ValidateBeforeDelete(ICollection<CategoryItem> instances)
		{
			// no checks needed
		}

		protected internal override FilterElement<DomInstance> CreateFilter(string fieldName, Comparer comparer, object value)
		{
			switch (fieldName)
			{
				case nameof(CategoryItem.Category):
					return FilterElementFactory.Create<Guid>(DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.CategoryItemInfo.Category), comparer, value);
				case nameof(CategoryItem.ModuleId):
					return FilterElementFactory.Create<Guid>(DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.CategoryItemInfo.ModuleID), comparer, value);
				case nameof(CategoryItem.InstanceId):
					return FilterElementFactory.Create<Guid>(DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.CategoryItemInfo.InstanceID), comparer, value);
			}

			return base.CreateFilter(fieldName, comparer, value);
		}

		protected internal override IOrderByElement CreateOrderBy(string fieldName, SortOrder sortOrder, bool naturalSort = false)
		{
			switch (fieldName)
			{
				case nameof(CategoryItem.Category):
					return OrderByElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.CategoryItemInfo.Category), sortOrder, naturalSort);
				case nameof(CategoryItem.ModuleId):
					return OrderByElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.CategoryItemInfo.ModuleID), sortOrder, naturalSort);
				case nameof(CategoryItem.InstanceId):
					return OrderByElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.CategoryItemInfo.InstanceID), sortOrder, naturalSort);
			}

			return base.CreateOrderBy(fieldName, sortOrder, naturalSort);
		}
	}
}
