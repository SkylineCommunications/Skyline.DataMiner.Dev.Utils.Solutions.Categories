namespace Skyline.DataMiner.Utils.Categories.API.Caching
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Skyline.DataMiner.Utils.Categories.API.Objects;
	using Skyline.DataMiner.Utils.Categories.Tools;

	public class CategoriesCache
	{
		private readonly object _lock = new();

		private readonly ConcurrentDictionary<ApiObjectReference<Scope>, Scope> _scopes = new();
		private readonly ConcurrentDictionary<string, Scope> _scopesByName = new();
		private readonly ConcurrentDictionary<ApiObjectReference<Category>, Category> _categories = new();

		private readonly OneToManyMapping<ApiObjectReference<Scope>, Category> _scopeCategoriesMapping = new();
		private readonly OneToManyMapping<ApiObjectReference<Category>, Category> _parentCategoriesMapping = new();

		public IReadOnlyDictionary<ApiObjectReference<Scope>, Scope> Scopes => _scopes;

		public IReadOnlyDictionary<string, Scope> ScopesByName => _scopesByName;

		public IReadOnlyDictionary<ApiObjectReference<Category>, Category> Categories => _categories;

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

		public IReadOnlyCollection<Category> GetCategoriesForScope(ApiObjectReference<Scope> scopeId)
		{
			lock (_lock)
			{
				return _scopeCategoriesMapping.GetChildren(scopeId).ToList();
			}
		}

		public IReadOnlyCollection<Category> GetChildCategories(ApiObjectReference<Category> parentCategoryId)
		{
			lock (_lock)
			{
				return _parentCategoriesMapping.GetChildren(parentCategoryId).ToList();
			}
		}

		public void LoadInitialData(CategoriesApi api)
		{
			if (api is null)
			{
				throw new ArgumentNullException(nameof(api));
			}

			var scopesTask = Task.Run(api.Scopes.ReadAll);
			var categoriesTask = Task.Run(api.Categories.ReadAll);

			Task.WaitAll(scopesTask, categoriesTask);

			lock (_lock)
			{
				UpdateScopes(scopesTask.Result, []);
				UpdateCategories(categoriesTask.Result, []);
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
					if (item.ParentCategory.HasValue)
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
	}
}
