namespace Skyline.DataMiner.Utils.Categories.API.Objects
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Utils.Categories.DOM.Model;

	public sealed class CategoryItem : IEquatable<CategoryItem>
	{
		public CategoryItem()
		{
			DomSection = new CategoryItemSection();
		}

		public CategoryItem(string moduleId, string instanceId) : this()
		{
			if (String.IsNullOrWhiteSpace(moduleId))
			{
				throw new ArgumentException($"'{nameof(moduleId)}' cannot be null or whitespace.", nameof(moduleId));
			}

			if (String.IsNullOrWhiteSpace(instanceId))
			{
				throw new ArgumentException($"'{nameof(instanceId)}' cannot be null or whitespace.", nameof(instanceId));
			}

			ModuleId = moduleId;
			InstanceId = instanceId;
		}

		internal CategoryItem(CategoryItemSection domSection)
		{
			DomSection = domSection ?? throw new ArgumentNullException(nameof(domSection));
		}

		internal CategoryItemSection DomSection { get; }

		public string ModuleId
		{
			get
			{
				return DomSection.ModuleID;
			}

			set
			{
				DomSection.ModuleID = value;
			}
		}

		public string InstanceId
		{
			get
			{
				return DomSection.InstanceID;
			}

			set
			{
				DomSection.InstanceID = value;
			}
		}

		public void Validate()
		{
			if (String.IsNullOrWhiteSpace(ModuleId))
			{
				throw new InvalidOperationException($"{nameof(ModuleId)} cannot be null or whitespace.");
			}

			if (String.IsNullOrWhiteSpace(InstanceId))
			{
				throw new InvalidOperationException($"{nameof(InstanceId)} cannot be null or whitespace.");
			}
		}

		public override string ToString()
		{
			return $"{InstanceId} ({ModuleId})";
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as CategoryItem);
		}

		public bool Equals(CategoryItem other)
		{
			return other is not null &&
				   EqualityComparer<CategoryItemSection>.Default.Equals(DomSection, other.DomSection);
		}

		public override int GetHashCode()
		{
			return EqualityComparer<CategoryItemSection>.Default.GetHashCode(DomSection);
		}

		public static bool operator ==(CategoryItem left, CategoryItem right)
		{
			return EqualityComparer<CategoryItem>.Default.Equals(left, right);
		}

		public static bool operator !=(CategoryItem left, CategoryItem right)
		{
			return !(left == right);
		}
	}
}
