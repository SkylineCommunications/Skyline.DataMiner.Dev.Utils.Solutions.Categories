namespace Skyline.DataMiner.Utils.Categories.API.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Utils.Categories.API.Objects;

	public static class CategoryExtensions
	{
		public static CategoryWithChildren ToTree(this IEnumerable<Category> categories)
		{
			if (categories is null)
			{
				throw new ArgumentNullException(nameof(categories));
			}

			var categoriesCollection = categories as ICollection<Category> ?? categories.ToList();

			// Build parent-child map
			var parentChildMap = new Dictionary<ApiObjectReference<Category>, List<Category>>();

			foreach (var category in categoriesCollection)
			{
				if (!category.ParentCategory.HasValue)
				{
					continue;
				}

				if (!parentChildMap.TryGetValue(category.ParentCategory.Value, out var childrenList))
				{
					childrenList = new List<Category>();
					parentChildMap[category.ParentCategory.Value] = childrenList;
				}

				childrenList.Add(category);
			}

			// Create CategoryWithChildren instances
			var categoriesWithChildren = categoriesCollection
				.Select(c =>
				{
					parentChildMap.TryGetValue(c, out var children);
					return new CategoryWithChildren(c, children ?? Enumerable.Empty<Category>());
				})
				.ToList();

			// Find the root categories (those without a parent)
			var rootCategories = categoriesWithChildren
				.Where(c => !c.ParentCategory.HasValue)
				.ToList();

			// If there's only one root category, return it directly
			if (rootCategories.Count == 1)
			{
				return rootCategories[0];
			}

			// Create a dummy root category to hold multiple root categories
			return new CategoryWithChildren(Category.DefaultRootCategory, rootCategories);
		}
	}
}
