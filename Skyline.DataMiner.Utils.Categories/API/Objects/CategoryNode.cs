namespace Skyline.DataMiner.Utils.Categories.API.Objects
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Utils.Categories.Tools;

	public class CategoryNode : IEquatable<CategoryNode>
	{
		public CategoryNode(Category category, IEnumerable<CategoryNode> children)
		{
			Category = category ?? throw new ArgumentNullException(nameof(category));
			Children = (children ?? []).ToList();
		}

		public CategoryNode(Category category) : this(category, [])
		{
		}

		public Category Category { get; }

		public IReadOnlyCollection<CategoryNode> Children { get; }

		public override string ToString()
		{
			return $"Category: {Category}, Children count: {Children.Count}";
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as CategoryNode);
		}

		public bool Equals(CategoryNode other)
		{
			return other is not null &&
				   EqualityComparer<Category>.Default.Equals(Category, other.Category) &&
				   Children.ToHashSet().SetEquals(other.Children);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = -17;

				hash = (hash * -31) + EqualityComparer<Category>.Default.GetHashCode(Category);
				hash = (hash * -31) + HashCode.GetOrderIndependentHashCode(Children);

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
	}
}
