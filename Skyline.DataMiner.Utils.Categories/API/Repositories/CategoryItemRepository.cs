namespace Skyline.DataMiner.Utils.Categories.API.Repositories
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.Categories.API.Objects;
	using Skyline.DataMiner.Utils.Categories.API.Tools;
	using Skyline.DataMiner.Utils.Categories.DOM.Helpers;
	using Skyline.DataMiner.Utils.Categories.DOM.Model;

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
