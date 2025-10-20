namespace Skyline.DataMiner.Utils.Categories.API.Objects
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class CategoryWithChildren : Category
	{
		internal CategoryWithChildren(Category category, IEnumerable<Category> children) : base(category.DomInstance)
		{
			if (category is null)
			{
				throw new ArgumentNullException(nameof(category));
			}

			if (children is null)
			{
				throw new ArgumentNullException(nameof(children));
			}

			Children = children.ToList();
		}

		public IReadOnlyCollection<Category> Children { get; }
	}
}
