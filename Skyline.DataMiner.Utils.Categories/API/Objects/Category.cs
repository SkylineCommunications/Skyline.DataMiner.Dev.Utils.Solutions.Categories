namespace Skyline.DataMiner.Utils.Categories.API.Objects
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.Categories.API.Repositories;
	using Skyline.DataMiner.Utils.Categories.API.Tools;
	using Skyline.DataMiner.Utils.Categories.API.Validation;
	using Skyline.DataMiner.Utils.Categories.DOM.Model;

	public class Category : ApiObject<Category>
	{
		public static readonly Category DefaultRootCategory = new(new Guid("33bb33bb-33bb-33bb-33bb-33bb33bb33bb"))
		{
			Name = "Root",
		};

		private readonly CategoryInstance _domInstance;

		public Category() : this(new CategoryInstance())
		{
		}

		public Category(Guid id) : this(new CategoryInstance(id))
		{
		}

		internal Category(CategoryInstance domInstance) : base(domInstance)
		{
			_domInstance = domInstance ?? throw new ArgumentNullException(nameof(domInstance));
		}

		internal Category(DomInstance domInstance) : this(new CategoryInstance(domInstance))
		{
		}

		internal static DomDefinitionId DomDefinition => SlcCategoriesIds.Definitions.Category;

		public string Name
		{
			get
			{
				return _domInstance.CategoryInfo.Name;
			}

			set
			{
				_domInstance.CategoryInfo.Name = value;
			}
		}

		public ApiObjectReference<Category>? ParentCategory
		{
			get
			{
				return _domInstance.CategoryInfo.ParentCategory;
			}

			set
			{
				_domInstance.CategoryInfo.ParentCategory = value != ApiObjectReference<Category>.Empty ? value : null;
			}
		}

		public ApiObjectReference<Scope> Scope
		{
			get
			{
				if (_domInstance.CategoryInfo.Scope.HasValue)
				{
					return _domInstance.CategoryInfo.Scope.Value;
				}

				return ApiObjectReference<Scope>.Empty;
			}

			set
			{
				_domInstance.CategoryInfo.Scope = value != ApiObjectReference<Scope>.Empty ? value : null;
			}
		}

		public bool IsRootCategory => ParentCategory == ApiObjectReference<Category>.Empty;

		public IEnumerable<Category> GetChildCategories(CategoryRepository categoryRepository)
		{
			if (categoryRepository is null)
			{
				throw new ArgumentNullException(nameof(categoryRepository));
			}

			return categoryRepository.GetChildCategories(this);
		}

		public IEnumerable<Category> GetDescendantCategories(CategoryRepository categoryRepository)
		{
			if (categoryRepository is null)
			{
				throw new ArgumentNullException(nameof(categoryRepository));
			}

			return categoryRepository.GetDescendantCategories(this);
		}

		public IEnumerable<Category> GetAncestorPath(CategoryRepository categoryRepository)
		{
			if (categoryRepository is null)
			{
				throw new ArgumentNullException(nameof(categoryRepository));
			}

			return categoryRepository.GetAncestorPath(this);
		}

		public IEnumerable<CategoryItem> GetChildItems(CategoryItemRepository categoryItemRepository)
		{
			if (categoryItemRepository is null)
			{
				throw new ArgumentNullException(nameof(categoryItemRepository));
			}

			return categoryItemRepository.GetChildItems(this);
		}

		public void AddChildItems(CategoryItemRepository categoryItemRepository, ICollection<CategoryItemIdentifier> items)
		{
			if (categoryItemRepository is null)
			{
				throw new ArgumentNullException(nameof(categoryItemRepository));
			}

			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			categoryItemRepository.AddChildItems(this, items);
		}

		public void RemoveChildItems(CategoryItemRepository categoryItemRepository, ICollection<CategoryItemIdentifier> items)
		{
			if (categoryItemRepository is null)
			{
				throw new ArgumentNullException(nameof(categoryItemRepository));
			}

			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			categoryItemRepository.RemoveChildItems(this, items);
		}

		public void ReplaceChildItems(CategoryItemRepository categoryItemRepository, ICollection<CategoryItemIdentifier> items)
		{
			if (categoryItemRepository is null)
			{
				throw new ArgumentNullException(nameof(categoryItemRepository));
			}

			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			categoryItemRepository.ReplaceChildItems(this, items);
		}

		public void ClearChildItems(CategoryItemRepository categoryItemRepository)
		{
			if (categoryItemRepository is null)
			{
				throw new ArgumentNullException(nameof(categoryItemRepository));
			}

			categoryItemRepository.ClearChildItems(this);
		}

		public ValidationResult Validate()
		{
			var result = new ValidationResult();

			if (!NameUtil.Validate(Name, out var error))
			{
				result.AddError(error, this, x => x.Name);
			}

			if (Scope == ApiObjectReference<Scope>.Empty)
			{
				result.AddError($"Scope is mandatory.", this, x => x.Scope);
			}

			return result;
		}
	}

	public static class CategoryExposers
	{
		public static readonly Exposer<Category, Guid> ID = new(x => x.ID, nameof(Category.ID));
		public static readonly Exposer<Category, string> Name = new(x => x.Name, nameof(Category.Name));
		public static readonly Exposer<Category, ApiObjectReference<Category>?> ParentCategory = new(x => x.ParentCategory, nameof(Category.ParentCategory));
		public static readonly Exposer<Category, ApiObjectReference<Scope>?> Scope = new(x => x.Scope, nameof(Category.Scope));
	}
}
