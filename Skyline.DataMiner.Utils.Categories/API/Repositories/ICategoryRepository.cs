namespace Skyline.DataMiner.Solutions.Categories.API
{
	using System.Collections.Generic;

	public interface ICategoryRepository : IRepository<Category>
	{
		void AddChildItems(ApiObjectReference<Category> category, ICollection<CategoryItemIdentifier> items);

		void ClearChildItems(ApiObjectReference<Category> category);

		void Delete(IEnumerable<Category> instances);

		IEnumerable<Category> GetAncestorPath(ApiObjectReference<Category> category);

		IEnumerable<Category> GetByScope(ApiObjectReference<Scope> scope);

		IEnumerable<Category> GetChildCategories(ApiObjectReference<Category> parentCategory);

		IEnumerable<Category> GetDescendantCategories(ApiObjectReference<Category> parentCategory);

		CategoryNode GetTree();

		CategoryNode GetTree(ApiObjectReference<Scope> scope);

		void RemoveChildItems(ApiObjectReference<Category> category, ICollection<CategoryItemIdentifier> items);

		void ReplaceChildItems(ApiObjectReference<Category> category, ICollection<CategoryItemIdentifier> items);
	}
}