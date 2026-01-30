namespace Skyline.DataMiner.Solutions.Categories.API
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Solutions.Categories.Tools;

	public class CategoryNode : IEquatable<CategoryNode>
	{
		public CategoryNode(Category category, IEnumerable<CategoryNode> childCategories, IEnumerable<CategoryItem> childItems)
		{
			Category = category ?? throw new ArgumentNullException(nameof(category));
			ChildCategories = (childCategories ?? []).ToList();
			ChildItems = (childItems ?? []).ToList();

			foreach (var childCategory in ChildCategories)
			{
				childCategory.SetParent(this);
			}
		}

		public CategoryNode(Category category, IEnumerable<CategoryNode> childCategories) : this(category, childCategories, [])
		{
		}

		public CategoryNode(Category category, IEnumerable<CategoryItem> childItems) : this(category, [], childItems)
		{
		}

		public CategoryNode(Category category) : this(category, [], [])
		{
		}

		public CategoryNode Parent { get; private set; }

		public Category Category { get; }

		public IReadOnlyCollection<CategoryNode> ChildCategories { get; }

		public IReadOnlyCollection<CategoryItem> ChildItems { get; }

		public IEnumerable<CategoryNode> GetDescendantCategories()
		{
			var visited = new HashSet<CategoryNode>();
			var stack = new Stack<CategoryNode>();
			stack.Push(this);

			while (stack.Count > 0)
			{
				var current = stack.Pop();
				var children = current.ChildCategories;

				foreach (var child in children.Reverse())
				{
					if (visited.Add(child))
					{
						yield return child;
						stack.Push(child);
					}
				}
			}
		}

		public IEnumerable<CategoryItem> GetDescendantItems()
		{
			return GetDescendantCategories().SelectMany(c => c.ChildItems)
				.Concat(ChildItems);
		}

		public bool TryFindCategory(ApiObjectReference<Category> categoryId, out CategoryNode category)
		{
			if (Category == categoryId)
			{
				category = this;
				return true;
			}

			foreach (var descendant in GetDescendantCategories())
			{
				if (descendant.Category == categoryId)
				{
					category = descendant;
					return true;
				}
			}

			category = null;
			return false;
		}

		public override string ToString()
		{
			return $"CategoryNode (Category: {Category.Name}, ChildCategories: {ChildCategories.Count}, ChildItems: {ChildItems.Count})";
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as CategoryNode);
		}

		public bool Equals(CategoryNode other)
		{
			return other is not null &&
				   EqualityComparer<Category>.Default.Equals(Category, other.Category) &&
				   CollectionEqualityHelper.Equals(ChildCategories, other.ChildCategories, ignoreOrder: true) &&
				   CollectionEqualityHelper.Equals(ChildItems, other.ChildItems, ignoreOrder: true);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;

				hash = (hash * 31) + EqualityComparer<Category>.Default.GetHashCode(Category);
				hash = (hash * 31) + CollectionEqualityHelper.GetHashCode(ChildCategories, ignoreOrder: true);
				hash = (hash * 31) + CollectionEqualityHelper.GetHashCode(ChildItems, ignoreOrder: true);

				return hash;
			}
		}

		public static bool operator ==(CategoryNode left, CategoryNode right)
		{
			return EqualityComparer<CategoryNode>.Default.Equals(left, right);
		}

		public static bool operator !=(CategoryNode left, CategoryNode right)
		{
			return !(left == right);
		}

		private void SetParent(CategoryNode parent)
		{
			if (Parent != null && Parent != parent)
			{
				throw new InvalidOperationException("Parent has already been set.");
			}

			Parent = parent;
		}
	}
}
