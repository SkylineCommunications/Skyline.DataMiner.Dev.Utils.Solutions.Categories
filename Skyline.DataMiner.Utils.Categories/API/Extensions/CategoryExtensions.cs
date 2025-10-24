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
	}
}
