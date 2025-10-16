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

	public class CategoryRepository : Repository<Category>
	{
		internal CategoryRepository(SlcCategoriesHelper helper, IConnection connection) : base(helper, connection)
		{
		}

		protected internal override DomDefinitionId DomDefinition => Category.DomDefinition;

		protected internal override Category CreateInstance(DomInstance domInstance)
		{
			return new Category(domInstance);
		}

		protected override void ValidateBeforeSave(ICollection<Category> instances)
		{
			foreach (var instance in instances)
			{
				instance.Validate().ThrowIfInvalid();
			}

			CheckDuplicatesBeforeSave(instances);
		}

		protected override void ValidateBeforeDelete(ICollection<Category> instances)
		{
			// no checks needed
		}

		protected internal override FilterElement<DomInstance> CreateFilter(string fieldName, Comparer comparer, object value)
		{
			switch (fieldName)
			{
				case nameof(Category.Name):
					return FilterElementFactory.Create<string>(DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.CategoryInfo.Name), comparer, value);
				case nameof(Category.ParentCategory):
					return FilterElementFactory.Create<Guid>(DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.CategoryInfo.ParentCategory), comparer, value);
			}

			return base.CreateFilter(fieldName, comparer, value);
		}

		protected internal override IOrderByElement CreateOrderBy(string fieldName, SortOrder sortOrder, bool naturalSort = false)
		{
			switch (fieldName)
			{
				case nameof(Category.Name):
					return OrderByElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.CategoryInfo.Name), sortOrder, naturalSort);
				case nameof(Category.ParentCategory):
					return OrderByElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.CategoryInfo.ParentCategory), sortOrder, naturalSort);
			}

			return base.CreateOrderBy(fieldName, sortOrder, naturalSort);
		}

		private void CheckDuplicatesBeforeSave(ICollection<Category> instances)
		{
			FilterElement<DomInstance> CreateFilter(Category c) =>
				new ANDFilterElement<DomInstance>(
					DomInstanceExposers.Id.NotEqual(c.ID),
					DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.CategoryInfo.Name).Equal(c.Name));

			var conflicts = FilterQueryExecutor.RetrieveFilteredItems(instances, CreateFilter, Read).ToList();

			if (conflicts.Count > 0)
			{
				var names = String.Join(", ", conflicts
					.Select(x => x.Name)
					.OrderBy(x => x, new NaturalSortComparer()));

				throw new InvalidOperationException($"Cannot save categories. The following names are already in use: {names}");
			}
		}
	}
}
