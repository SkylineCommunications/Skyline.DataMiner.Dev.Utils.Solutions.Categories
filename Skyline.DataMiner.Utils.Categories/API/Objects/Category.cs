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
		public static readonly Category DefaultRootCategory = new() { Name = "Root" };

		private readonly CategoryInstance _domInstance;

		public Category() : this(new CategoryInstance())
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

		public ApiObjectReference<Category> RootCategory
		{
			get
			{
				if (_domInstance.CategoryInfo.RootCategory.HasValue)
				{
					return _domInstance.CategoryInfo.RootCategory.Value;
				}

				return ApiObjectReference<Category>.Empty;
			}

			set
			{
				_domInstance.CategoryInfo.RootCategory = value;
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
				_domInstance.CategoryInfo.ParentCategory = value;
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
				_domInstance.CategoryInfo.Scope = value;
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

			if (!IsRootCategory && RootCategory == ApiObjectReference<Category>.Empty)
			{
				result.AddError("A root category is required when a parent category is assigned.", this, x => x.RootCategory);
			}

			return result;
		}
	}

	public static class CategoryExposers
	{
		public static readonly Exposer<Category, Guid> ID = new(x => x.ID, nameof(Category.ID));
		public static readonly Exposer<Category, string> Name = new(x => x.Name, nameof(Category.Name));
		public static readonly Exposer<Category, ApiObjectReference<Category>?> RootCategory = new(x => x.RootCategory, nameof(Category.RootCategory));
		public static readonly Exposer<Category, ApiObjectReference<Category>?> ParentCategory = new(x => x.ParentCategory, nameof(Category.ParentCategory));
		public static readonly Exposer<Category, ApiObjectReference<Scope>?> Scope = new(x => x.Scope, nameof(Category.Scope));
	}
}
