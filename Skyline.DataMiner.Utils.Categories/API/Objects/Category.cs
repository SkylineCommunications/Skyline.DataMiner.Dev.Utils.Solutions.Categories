namespace Skyline.DataMiner.Utils.Categories.API.Objects
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.Categories.API.Repositories;
	using Skyline.DataMiner.Utils.Categories.API.Tools;
	using Skyline.DataMiner.Utils.Categories.API.Validation;
	using Skyline.DataMiner.Utils.Categories.DOM.Model;
	using Skyline.DataMiner.Utils.Categories.Tools;

	public class Category : ApiObject<Category>
	{
		public static readonly Category DefaultRootCategory = new() { Name = "Root" };

		private readonly CategoryInstance _domInstance;

		private readonly WrappedList<CategoryItemSection, CategoryItem> _wrappedItems;

		public Category() : this(new CategoryInstance())
		{
		}

		internal Category(CategoryInstance domInstance) : base(domInstance)
		{
			_domInstance = domInstance ?? throw new ArgumentNullException(nameof(domInstance));

			_wrappedItems = new WrappedList<CategoryItemSection, CategoryItem>(
				_domInstance.CategoryItem,
				x => new CategoryItem(x),
				x => x.DomSection);
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
				_domInstance.CategoryInfo.ParentCategory = value;
			}
		}

		public ApiObjectReference<Scope>? Scope
		{
			get
			{
				return _domInstance.CategoryInfo.Scope;
			}

			set
			{
				_domInstance.CategoryInfo.Scope = value;
			}
		}

		public IList<CategoryItem> Items
		{
			get
			{
				return _wrappedItems;
			}

			set
			{
				_wrappedItems.Clear();
				_wrappedItems.AddRange(value);
			}
		}

		public bool ContainsItem(string moduleId, string instanceId)
		{
			return Items.Any(x => x.ModuleId == moduleId && x.InstanceId == instanceId);
		}

		public bool AddItem(string moduleId, string instanceId)
		{
			if (String.IsNullOrWhiteSpace(moduleId))
			{
				throw new ArgumentException($"'{nameof(moduleId)}' cannot be null or whitespace.", nameof(moduleId));
			}

			if (String.IsNullOrWhiteSpace(instanceId))
			{
				throw new ArgumentException($"'{nameof(instanceId)}' cannot be null or whitespace.", nameof(instanceId));
			}

			if (ContainsItem(moduleId, instanceId))
			{
				return false;
			}

			Items.Add(new CategoryItem
			{
				ModuleId = moduleId,
				InstanceId = instanceId,
			});

			return true;
		}

		public bool RemoveItem(string moduleId, string instanceId)
		{
			var found = false;

			foreach (var item in Items.ToList())
			{
				if (item.ModuleId == moduleId && item.InstanceId == instanceId)
				{
					found = true;
					Items.Remove(item);
				}
			}

			return found;
		}

		public IEnumerable<Category> GetChildCategories(CategoryRepository categoryRepository)
		{
			if (categoryRepository is null)
			{
				throw new ArgumentNullException(nameof(categoryRepository));
			}

			return categoryRepository.GetChildCategories(this);
		}

		public ValidationResult Validate()
		{
			var result = new ValidationResult();

			if (!NameUtil.Validate(Name, out var error))
			{
				result.AddError(error, this, x => x.Name);
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
