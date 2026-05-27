namespace Skyline.DataMiner.Solutions.Categories.API
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;

	using Skyline.DataMiner.Solutions.Categories.DOM.Model;

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

		IReadOnlyCollection<Category> Read(Scope scope, string name);

		IDictionary<string, IReadOnlyCollection<Category>> Read(Scope scope, IEnumerable<string> names);

		void AddChildItems(ApiObjectReference<Category> category, ICollection<CategoryItemIdentifier> items);

		void ClearChildItems(ApiObjectReference<Category> category);

		void RemoveChildItems(ApiObjectReference<Category> category, ICollection<CategoryItemIdentifier> items);

		void ReplaceChildItems(ApiObjectReference<Category> category, ICollection<CategoryItemIdentifier> items);
	}
}