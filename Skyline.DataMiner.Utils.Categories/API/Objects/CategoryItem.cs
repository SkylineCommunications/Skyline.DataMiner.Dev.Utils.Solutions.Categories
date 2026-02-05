namespace Skyline.DataMiner.Solutions.Categories.API
{
	using System;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Solutions.Categories.DOM.Model;

	public class CategoryItem : ApiObject<CategoryItem>
	{
		private readonly CategoryItemInstance _domInstance;

		public CategoryItem() : this(new CategoryItemInstance())
		{
		}

		internal CategoryItem(CategoryItemInstance domInstance) : base(domInstance)
		{
			_domInstance = domInstance ?? throw new ArgumentNullException(nameof(domInstance));
		}

		internal CategoryItem(DomInstance domInstance) : this(new CategoryItemInstance(domInstance))
		{
		}

		internal static DomDefinitionId DomDefinition => SlcCategoriesIds.Definitions.CategoryItem;

		public ApiObjectReference<Category> Category
		{
			get
			{
				if (_domInstance.CategoryItemInfo.Category.HasValue)
				{
					return _domInstance.CategoryItemInfo.Category.Value;
				}

				return ApiObjectReference<Category>.Empty;
			}

			set
			{
				_domInstance.CategoryItemInfo.Category = value;
			}
		}

		public string ModuleId
		{
			get
			{
				return _domInstance.CategoryItemInfo.ModuleID;
			}

			set
			{
				_domInstance.CategoryItemInfo.ModuleID = value;
			}
		}

		public string InstanceId
		{
			get
			{
				return _domInstance.CategoryItemInfo.InstanceID;
			}

			set
			{
				_domInstance.CategoryItemInfo.InstanceID = value;
			}
		}

		public CategoryItemIdentifier ToIdentifier()
		{
			return new CategoryItemIdentifier(ModuleId, InstanceId);
		}

		public static implicit operator CategoryItemIdentifier(CategoryItem item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			return item.ToIdentifier();
		}
	}

	public static class CategoryItemExposers
	{
		public static readonly Exposer<CategoryItem, Guid> ID = new(x => x.ID, nameof(CategoryItem.ID));
		public static readonly Exposer<CategoryItem, ApiObjectReference<Category>?> Category = new(x => x.Category, nameof(CategoryItem.Category));
		public static readonly Exposer<CategoryItem, string> ModuleId = new(x => x.ModuleId, nameof(CategoryItem.ModuleId));
		public static readonly Exposer<CategoryItem, string> InstanceId = new(x => x.InstanceId, nameof(CategoryItem.InstanceId));
	}
}
