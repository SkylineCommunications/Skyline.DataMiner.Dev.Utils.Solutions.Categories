namespace Skyline.DataMiner.Solutions.Categories.API
{
	using System.Collections.Generic;

	public interface ICategoryItemRepository : IRepository<CategoryItem>
	{
		void AddChildItems(ApiObjectReference<Category> category, ICollection<CategoryItem> itemsToAdd);

		void AddChildItems(ApiObjectReference<Category> category, ICollection<CategoryItemIdentifier> itemIdentifiersToAdd);

		void ClearChildItems(ApiObjectReference<Category> category);

		IEnumerable<CategoryItem> GetChildItems(ApiObjectReference<Category> parentCategory);

		IEnumerable<CategoryItem> GetChildItems(IEnumerable<ApiObjectReference<Category>> parentCategories);

		void RemoveChildItems(ApiObjectReference<Category> category, ICollection<CategoryItem> itemsToDelete);

		void RemoveChildItems(ApiObjectReference<Category> category, ICollection<CategoryItemIdentifier> itemIdentifiersToDelete);

		void ReplaceChildItems(ApiObjectReference<Category> category, ICollection<CategoryItem> newItems);

		void ReplaceChildItems(ApiObjectReference<Category> category, ICollection<CategoryItemIdentifier> newItemIdentifiers);
	}
}