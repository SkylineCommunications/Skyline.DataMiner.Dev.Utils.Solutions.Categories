namespace Skyline.DataMiner.Utils.Categories.API.Objects
{
	using System;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.Categories.API.Tools;
	using Skyline.DataMiner.Utils.Categories.API.Validation;
	using Skyline.DataMiner.Utils.Categories.DOM.Model;

	public class Category : ApiObject<Category>
	{
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
	}
}
