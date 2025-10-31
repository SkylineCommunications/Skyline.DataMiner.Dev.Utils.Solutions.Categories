namespace Skyline.DataMiner.Utils.Categories.API.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.Categories.API.Extensions;
	using Skyline.DataMiner.Utils.Categories.API.Objects;
	using Skyline.DataMiner.Utils.Categories.API.Tools;
	using Skyline.DataMiner.Utils.Categories.DOM.Helpers;
	using Skyline.DataMiner.Utils.Categories.DOM.Model;
	using Skyline.DataMiner.Utils.Categories.DOM.Tools;
	using Skyline.DataMiner.Utils.DOM.Extensions;

	using SLDataGateway.API.Types.Querying;

	public class CategoryRepository : Repository<Category>
	{
		internal CategoryRepository(SlcCategoriesHelper helper, CategoryItemRepository itemRepository, IConnection connection)
			: base(helper, connection)
		{
			ItemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
		}

		protected internal override DomDefinitionId DomDefinition => Category.DomDefinition;

		public CategoryItemRepository ItemRepository { get; }

		public IEnumerable<Category> GetByScope(ApiObjectReference<Scope> scope)
		{
			if (scope == ApiObjectReference<Scope>.Empty)
			{
				return [];
			}

			var filter = new ANDFilterElement<DomInstance>(
				DomInstanceExposers.DomDefinitionId.Equal(SlcCategoriesIds.Definitions.Category.Id),
				DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.CategoryInfo.Scope).Equal(scope.ID));

			return Read(filter);
		}

		public IEnumerable<Category> GetByRootCategory(ApiObjectReference<Category> rootCategory)
		{
			if (rootCategory == ApiObjectReference<Category>.Empty)
			{
				return [];
			}

			var filter = new ANDFilterElement<DomInstance>(
				DomInstanceExposers.DomDefinitionId.Equal(SlcCategoriesIds.Definitions.Category.Id),
				DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.CategoryInfo.RootCategory).Equal(rootCategory.ID));

			return Read(filter);
		}

		public IEnumerable<Category> GetChildCategories(ApiObjectReference<Category> parentCategory)
		{
			if (parentCategory == ApiObjectReference<Category>.Empty)
			{
				return [];
			}

			var filter = new ANDFilterElement<DomInstance>(
				DomInstanceExposers.DomDefinitionId.Equal(SlcCategoriesIds.Definitions.Category.Id),
				DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.CategoryInfo.ParentCategory).Equal(parentCategory.ID));

			return Read(filter);
		}

		public IEnumerable<Category> GetDescendantCategories(ApiObjectReference<Category> parentCategory)
		{
			var stack = new Stack<Category>();
			var visited = new HashSet<Category>();

			foreach (var child in GetChildCategories(parentCategory).Reverse())
			{
				if (visited.Add(child))
					stack.Push(child);
			}

			while (stack.Count > 0)
			{
				var current = stack.Pop();
				yield return current;

				foreach (var child in GetChildCategories(current).Reverse())
				{
					if (visited.Add(child))
						stack.Push(child);
				}
			}
		}

		public IEnumerable<Category> GetAncestorPath(ApiObjectReference<Category> category)
		{
			var pathToRoot = new LinkedList<Category>();
			var visited = new HashSet<ApiObjectReference<Category>>();

			while (category != ApiObjectReference<Category>.Empty)
			{
				// Check for circular reference before reading
				if (!visited.Add(category))
				{
					throw new InvalidOperationException($"Circular reference detected in category hierarchy at category ID '{category.ID}'.");
				}

				var current = Read(category);
				if (current == null)
				{
					throw new InvalidOperationException($"Category with ID '{category.ID}' does not exist.");
				}

				pathToRoot.AddFirst(current);

				if (!current.ParentCategory.HasValue ||
					current.ParentCategory == ApiObjectReference<Category>.Empty)
				{
					break;
				}

				category = current.ParentCategory.Value;
			}

			return pathToRoot;
		}

		public CategoryNode GetTree()
		{
			return ReadAll().ToTree();
		}

		public CategoryNode GetTree(ApiObjectReference<Scope> scope)
		{
			return GetByScope(scope).ToTree();
		}

		public override void Delete(IEnumerable<Category> instances)
		{
			var instancesCollection = instances as ICollection<Category> ?? instances.ToList();

			// First delete all child item links
			var childItems = ItemRepository.GetChildItems(instancesCollection.Select(x => x.Reference));
			ItemRepository.Delete(childItems);

			// Then delete the categories themselves
			base.Delete(instancesCollection);
		}

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
