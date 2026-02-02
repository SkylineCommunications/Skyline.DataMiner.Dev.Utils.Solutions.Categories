namespace Skyline.DataMiner.Solutions.Categories.API
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Solutions.Categories.Tools;

	public class CategoriesCache
	{
		private readonly object _lock = new();
		private readonly NaturalSortComparer _naturalSortComparer = new();

		private readonly ConcurrentDictionary<ApiObjectReference<Scope>, Scope> _scopes = new();
		private readonly ConcurrentDictionary<string, Scope> _scopesByName = new();
		private readonly ConcurrentDictionary<ApiObjectReference<Category>, Category> _categories = new();
		private readonly ConcurrentDictionary<ApiObjectReference<CategoryItem>, CategoryItem> _categoryItems = new();

		private readonly OneToManyMapping<ApiObjectReference<Scope>, Category> _scopeCategoriesMapping = new();
		private readonly OneToManyMapping<ApiObjectReference<Category>, Category> _parentCategoriesMapping = new();
		private readonly OneToManyMapping<ApiObjectReference<Category>, CategoryItem> _categoryItemsMapping = new();
		private readonly ManyToManyMapping<ApiObjectReference<Category>, CategoryItemIdentifier> _categoryItemIdentifiersMapping = new();

		public IReadOnlyDictionary<ApiObjectReference<Scope>, Scope> Scopes => _scopes;

		public IReadOnlyDictionary<string, Scope> ScopesByName => _scopesByName;

		public IReadOnlyDictionary<ApiObjectReference<Category>, Category> Categories => _categories;

		public IReadOnlyDictionary<ApiObjectReference<CategoryItem>, CategoryItem> CategoryItems => _categoryItems;

		public Scope GetScope(ApiObjectReference<Scope> scopeId)
		{
			if (!TryGetScope(scopeId, out var scope))
			{
				throw new ArgumentException($"Couldn't find scope with ID {scopeId.ID}", nameof(scopeId));
			}

			return scope;
		}

		public Scope GetScope(string name)
		{
			if (!TryGetScope(name, out var scope))
			{
				throw new ArgumentException($"Couldn't find scope with name '{name}'", nameof(name));
			}

			return scope;
		}

		public bool TryGetScope(ApiObjectReference<Scope> scopeId, out Scope scope)
		{
			return _scopes.TryGetValue(scopeId, out scope);
		}

		public bool TryGetScope(string name, out Scope scope)
		{
			return _scopesByName.TryGetValue(name, out scope);
		}

		public Category GetCategory(ApiObjectReference<Category> categoryId)
		{
			if (!TryGetCategory(categoryId, out var category))
			{
				throw new ArgumentException($"Couldn't find category with ID {categoryId.ID}", nameof(categoryId));
			}

			return category;
		}

		public bool TryGetCategory(ApiObjectReference<Category> id, out Category category)
		{
			return _categories.TryGetValue(id, out category);
		}

		public CategoryItem GetCategoryItem(ApiObjectReference<CategoryItem> categoryItemId)
		{
			if (!TryGetCategoryItem(categoryItemId, out var categoryItem))
			{
				throw new ArgumentException($"Couldn't find category item with ID {categoryItemId.ID}", nameof(categoryItemId));
			}

			return categoryItem;
		}

		public bool TryGetCategoryItem(ApiObjectReference<CategoryItem> id, out CategoryItem categoryItem)
		{
			return _categoryItems.TryGetValue(id, out categoryItem);
		}

		public IReadOnlyCollection<Category> GetCategoriesForScope(ApiObjectReference<Scope> scopeId)
		{
			lock (_lock)
			{
				return _scopeCategoriesMapping.GetChildren(scopeId).ToList();
			}
		}

		public IReadOnlyCollection<Category> GetRootCategoriesForScope(ApiObjectReference<Scope> scopeId)
		{
			lock (_lock)
			{
				return GetCategoriesForScope(scopeId).Where(x => x.IsRootCategory).ToList();
			}
		}

		public IReadOnlyCollection<Category> GetCategoriesForScope(string scopeName)
		{
			if (String.IsNullOrWhiteSpace(scopeName))
			{
				throw new ArgumentException($"'{nameof(scopeName)}' cannot be null or whitespace.", nameof(scopeName));
			}

			lock (_lock)
			{
				var scope = GetScope(scopeName);
				return GetCategoriesForScope(scope);
			}
		}

		public IReadOnlyCollection<Category> GetRootCategoriesForScope(string scopeName)
		{
			if (String.IsNullOrWhiteSpace(scopeName))
			{
				throw new ArgumentException($"'{nameof(scopeName)}' cannot be null or whitespace.", nameof(scopeName));
			}

			lock (_lock)
			{
				return GetCategoriesForScope(scopeName).Where(x => x.IsRootCategory).ToList();
			}
		}

		public IReadOnlyCollection<Category> GetChildCategories(ApiObjectReference<Category> parentCategoryId)
		{
			lock (_lock)
			{
				return _parentCategoriesMapping.GetChildren(parentCategoryId).ToList();
			}
		}

		public IReadOnlyCollection<Category> GetDescendantCategories(ApiObjectReference<Category> parentCategoryId)
		{
			lock (_lock)
			{
				var visited = new HashSet<Category>();
				var stack = new Stack<Category>();

				if (TryGetCategory(parentCategoryId, out var parentCategory))
				{
					stack.Push(parentCategory);
				}

				var descendants = new List<Category>();

				while (stack.Count > 0)
				{
					var current = stack.Pop();
					var children = GetChildCategories(current);

					foreach (var child in children.Reverse())
					{
						if (visited.Add(child))
						{
							descendants.Add(child);
							stack.Push(child);
						}
					}
				}

				return descendants;
			}
		}

		public IReadOnlyCollection<CategoryItem> GetChildItems(ApiObjectReference<Category> parentCategoryId)
		{
			lock (_lock)
			{
				return _categoryItemsMapping.GetChildren(parentCategoryId).ToList();
			}
		}

		public IReadOnlyCollection<CategoryItem> GetDescendantItems(ApiObjectReference<Category> parentCategoryId)
		{
			lock (_lock)
			{
				return GetDescendantCategories(parentCategoryId)
					.SelectMany(c => GetChildItems(c))
					.Concat(GetChildItems(parentCategoryId))
					.ToList();
			}
		}

		public IReadOnlyCollection<Category> GetAncestorPath(ApiObjectReference<Category> categoryId)
		{
			lock (_lock)
			{
				if (categoryId == ApiObjectReference<Category>.Empty)
				{
					return [];
				}

				var pathToRoot = new LinkedList<Category>();
				var visited = new HashSet<ApiObjectReference<Category>>();

				while (categoryId != ApiObjectReference<Category>.Empty)
				{
					// Check for circular reference before processing
					if (!visited.Add(categoryId.ID))
					{
						throw new InvalidOperationException($"Circular reference detected in category hierarchy at category ID '{categoryId.ID}'.");
					}

					if (!TryGetCategory(categoryId, out var category))
					{
						break;
					}

					pathToRoot.AddFirst(category);

					if (!category.ParentCategory.HasValue ||
						category.ParentCategory == ApiObjectReference<Category>.Empty)
					{
						break;
					}

					categoryId = category.ParentCategory.Value;
				}

				return pathToRoot;
			}
		}

		public Category GetRootCategory(ApiObjectReference<Category> categoryId)
		{
			lock (_lock)
			{
				var path = GetAncestorPath(categoryId);
				return path.First();
			}
		}

		public bool ContainsItem(ApiObjectReference<Category> categoryId, CategoryItemIdentifier item)
		{
			lock (_lock)
			{
				return _categoryItemIdentifiersMapping.Contains(categoryId, item);
			}
		}

		public bool ContainsDescendantItem(ApiObjectReference<Category> categoryId, CategoryItemIdentifier item)
		{
			lock (_lock)
			{
				if (ContainsItem(categoryId, item))
				{
					return true;
				}

				return GetDescendantCategories(categoryId).Any(x => ContainsItem(x, item));
			}
		}

		public bool HasChildCategories(ApiObjectReference<Category> categoryId)
		{
			lock (_lock)
			{
				return _parentCategoriesMapping.GetChildren(categoryId).Any();
			}
		}

		public bool HasChildItems(ApiObjectReference<Category> categoryId)
		{
			lock (_lock)
			{
				return _categoryItemsMapping.GetChildren(categoryId).Any();
			}
		}

		public bool HasDescendantItems(ApiObjectReference<Category> categoryId)
		{
			lock (_lock)
			{
				if (HasChildItems(categoryId))
				{
					return true;
				}

				return GetDescendantCategories(categoryId).Any(x => HasChildItems(x));
			}
		}

		public CategoryNode GetSubtree(ApiObjectReference<Category> categoryId)
		{
			lock (_lock)
			{
				if (!TryGetCategory(categoryId, out var category))
				{
					throw new ArgumentException($"Couldn't find category with ID {categoryId.ID}", nameof(categoryId));
				}

				var childNodes = GetChildCategories(category)
					.OrderBy(c => c.Name, _naturalSortComparer)
					.Select(c => GetSubtree(c))
					.ToList();

				var childItems = GetChildItems(category);

				return new CategoryNode(category, childNodes, childItems);
			}
		}

		internal void LoadInitialData(CategoriesApi api)
		{
			if (api is null)
			{
				throw new ArgumentNullException(nameof(api));
			}

			var scopesTask = Task.Run(api.Scopes.ReadAll);
			var categoriesTask = Task.Run(api.Categories.ReadAll);
			var categoryItemsTask = Task.Run(api.CategoryItems.ReadAll);

			Task.WaitAll(scopesTask, categoriesTask, categoryItemsTask);

			lock (_lock)
			{
				UpdateScopes(scopesTask.Result, []);
				UpdateCategories(categoriesTask.Result, []);
				UpdateCategoryItems(categoryItemsTask.Result, []);
			}
		}

		public void UpdateScopes(IEnumerable<Scope> updated, IEnumerable<Scope> deleted)
		{
			if (updated is null)
			{
				throw new ArgumentNullException(nameof(updated));
			}

			if (deleted is null)
			{
				throw new ArgumentNullException(nameof(deleted));
			}

			lock (_lock)
			{
				foreach (var item in updated)
				{
					// Remove old name if it exists
					if (_scopes.TryGetValue(item.ID, out var existing))
					{
						_scopesByName.TryRemove(existing.Name, out _);
					}

					_scopes[item.ID] = item;
					_scopesByName[item.Name] = item;
				}

				foreach (var item in deleted)
				{
					_scopes.TryRemove(item.ID, out _);
					_scopesByName.TryRemove(item.Name, out _);

					_scopeCategoriesMapping.RemoveParent(item);
				}
			}
		}

		public void UpdateCategories(IEnumerable<Category> updated, IEnumerable<Category> deleted)
		{
			if (updated is null)
			{
				throw new ArgumentNullException(nameof(updated));
			}

			if (deleted is null)
			{
				throw new ArgumentNullException(nameof(deleted));
			}

			lock (_lock)
			{
				foreach (var item in updated)
				{
					// Remove from old mappings if it exists
					if (_categories.TryGetValue(item.ID, out var existing))
					{
						_scopeCategoriesMapping.RemoveChild(existing);
						_parentCategoriesMapping.RemoveChild(existing);
					}

					_categories[item.ID] = item;
					_scopeCategoriesMapping.AddOrUpdate(item.Scope, item);

					// Add to parent-child mapping if it has a parent category
					if (item.ParentCategory != ApiObjectReference<Category>.Empty)
					{
						_parentCategoriesMapping.AddOrUpdate(item.ParentCategory.Value, item);
					}
				}

				foreach (var item in deleted)
				{
					_categories.TryRemove(item.ID, out _);
					_scopeCategoriesMapping.RemoveChild(item);
					_parentCategoriesMapping.RemoveChild(item);
					_parentCategoriesMapping.RemoveParent(item);
				}
			}
		}

		public void UpdateCategoryItems(IEnumerable<CategoryItem> updated, IEnumerable<CategoryItem> deleted)
		{
			if (updated is null)
			{
				throw new ArgumentNullException(nameof(updated));
			}

			if (deleted is null)
			{
				throw new ArgumentNullException(nameof(deleted));
			}

			lock (_lock)
			{
				foreach (var item in updated)
				{
					// Remove from old mappings if it exists
					if (_categoryItems.TryGetValue(item.ID, out var existing))
					{
						_categoryItemsMapping.RemoveChild(existing);
						_categoryItemIdentifiersMapping.Remove(existing.Category, existing);
					}

					_categoryItems[item.ID] = item;
					_categoryItemsMapping.AddOrUpdate(item.Category, item);
					_categoryItemIdentifiersMapping.TryAdd(item.Category, item);
				}

				foreach (var item in deleted)
				{
					_categoryItems.TryRemove(item, out _);
					_categoryItemsMapping.RemoveChild(item);
					_categoryItemIdentifiersMapping.Remove(item.Category, item);
				}
			}
		}
	}
}
