namespace Skyline.DataMiner.Solutions.Categories.API
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Represents a lightweight identifier for a category item using ModuleId and InstanceId.
	/// </summary>
	public readonly struct CategoryItemIdentifier : IEquatable<CategoryItemIdentifier>
	{
		public CategoryItemIdentifier(string moduleId, string instanceId)
		{
			ModuleId = moduleId;
			InstanceId = instanceId;
		}

		public string ModuleId { get; }

		public string InstanceId { get; }

		public CategoryItem ToCategoryItem(ApiObjectReference<Category> category)
		{
			return new CategoryItem
			{
				Category = category,
				ModuleId = ModuleId,
				InstanceId = InstanceId,
			};
		}

		public override bool Equals(object obj)
		{
			return obj is CategoryItemIdentifier other && Equals(other);
		}

		public bool Equals(CategoryItemIdentifier other)
		{
			return EqualityComparer<string>.Default.Equals(ModuleId, other.ModuleId) &&
				   EqualityComparer<string>.Default.Equals(InstanceId, other.InstanceId);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;

				hash = (hash * 31) + EqualityComparer<string>.Default.GetHashCode(ModuleId);
				hash = (hash * 31) + EqualityComparer<string>.Default.GetHashCode(InstanceId);

				return hash;
			}
		}

		public static bool operator ==(CategoryItemIdentifier left, CategoryItemIdentifier right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(CategoryItemIdentifier left, CategoryItemIdentifier right)
		{
			return !left.Equals(right);
		}
	}
}