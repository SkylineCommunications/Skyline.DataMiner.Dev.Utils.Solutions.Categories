namespace Skyline.DataMiner.Solutions.Categories.API
{
	using System.Collections.Generic;

	public interface ICategoryRepository : IRepository<Category>
	{
		// Tree retrieval
		CategoryNode GetTree();

		CategoryNode GetTree(ApiObjectReference<Scope> scope);

		// Category retrieval
		IEnumerable<Category> GetAncestorPath(ApiObjectReference<Category> category);

		IEnumerable<Category> GetByScope(ApiObjectReference<Scope> scope);

		IEnumerable<Category> GetChildCategories(ApiObjectReference<Category> parentCategory);

		IEnumerable<Category> GetDescendantCategories(ApiObjectReference<Category> parentCategory);

		// Child items
		IEnumerable<CategoryItem> GetChildItems(ApiObjectReference<Category> category);

		void AddChildItems(ApiObjectReference<Category> category, ICollection<CategoryItemIdentifier> items);

		void ClearChildItems(ApiObjectReference<Category> category);

		void RemoveChildItems(ApiObjectReference<Category> category, ICollection<CategoryItemIdentifier> items);

		void ReplaceChildItems(ApiObjectReference<Category> category, ICollection<CategoryItemIdentifier> items);
	}
}