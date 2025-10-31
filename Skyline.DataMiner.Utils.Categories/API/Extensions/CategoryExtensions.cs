namespace Skyline.DataMiner.Utils.Categories.API.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Utils.Categories.API.Objects;
	using Skyline.DataMiner.Utils.Categories.Tools;

	public static class CategoryExtensions
	{
		private static readonly NaturalSortComparer _naturalSortComparer = new();

		public static CategoryNode ToTree(this IEnumerable<Category> categories)
		{
			if (categories is null)
			{
				throw new ArgumentNullException(nameof(categories));
			}

			var categoriesCollection = categories as ICollection<Category> ?? categories.ToList();

			// Build parent-child map
			var parentChildMap = new OneToManyMapping<ApiObjectReference<Category>, Category>();

			foreach (var category in categoriesCollection)
			{
				if (category.ParentCategory != ApiObjectReference<Category>.Empty)
				{
					parentChildMap.Add(category.ParentCategory.Value, category);
				}
			}

			// Recursive local function to build the tree structure
			CategoryNode BuildTreeNode(Category category)
			{
				var childNodes = parentChildMap.GetChildren(category)
					.OrderBy(c => c.Name, _naturalSortComparer)
					.Select(BuildTreeNode)
					.ToList();

				return new CategoryNode(category, childNodes);
			}

			// Find the root categories (those without a parent)
			var rootCategories = categoriesCollection
				.Where(c => !c.ParentCategory.HasValue || !parentChildMap.ContainsParent(c.ParentCategory.Value))
				.OrderBy(c => c.Name, _naturalSortComparer)
				.Select(BuildTreeNode)
				.ToList();

			// If there's only one root category, return it directly
			if (rootCategories.Count == 1)
			{
				return rootCategories[0];
			}

			// Create a dummy root category to hold multiple root categories
			return new CategoryNode(Category.DefaultRootCategory, rootCategories);
		}

		/// <summary>
		/// Sorts categories hierarchically, with parents appearing before their children,
		/// and siblings sorted naturally by name.
		/// </summary>
		/// <param name="categories">The collection of categories to sort.</param>
		/// <returns>A hierarchically sorted list of categories.</returns>
		public static IList<Category> SortHierarchically(this IEnumerable<Category> categories)
		{
			var sorted = new List<Category>();

			var childrenByParent = categories.ToLookup(c => c.ParentCategory);
			var visited = new HashSet<Category>();

			// Local recursive function
			void AddChildren(Guid? parentId)
			{
				var children = childrenByParent[parentId]
					.OrderBy(c => c.Name, _naturalSortComparer);

				foreach (var child in children)
				{
					if (visited.Add(child))
					{
						sorted.Add(child);
						AddChildren(child.ID);
					}
				}
			}

			// Start with root categories (those without a parent in the provided collection)
			var allCategoryIds = new HashSet<Guid>(categories.Select(c => c.ID));
			var roots = categories.Where(c => !c.ParentCategory.HasValue || !allCategoryIds.Contains(c.ParentCategory.Value));

			foreach (var root in roots.OrderBy(c => c.Name, _naturalSortComparer))
			{
				if (visited.Add(root))
				{
					sorted.Add(root);
					AddChildren(root.ID);
				}
			}

			return sorted;
		}
	}
}
