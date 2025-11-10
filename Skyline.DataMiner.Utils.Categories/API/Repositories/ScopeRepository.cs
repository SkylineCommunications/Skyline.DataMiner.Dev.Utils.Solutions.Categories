namespace Skyline.DataMiner.Utils.Categories.API.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.Categories.API.Objects;
	using Skyline.DataMiner.Utils.Categories.API.Tools;
	using Skyline.DataMiner.Utils.Categories.DOM.Helpers;
	using Skyline.DataMiner.Utils.Categories.DOM.Model;

	using SLDataGateway.API.Types.Querying;

	public class ScopeRepository : Repository<Scope>
	{
		internal ScopeRepository(SlcCategoriesHelper helper, CategoryRepository categoryRepository, IConnection connection)
			: base(helper, connection)
		{
			CategoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
		}

		protected internal override DomDefinitionId DomDefinition => Scope.DomDefinition;

		private CategoryRepository CategoryRepository { get; }

		protected internal override Scope CreateInstance(DomInstance domInstance)
		{
			return new Scope(domInstance);
		}

		protected override void ValidateBeforeSave(ICollection<Scope> instances)
		{
			foreach (var instance in instances)
			{
				instance.Validate().ThrowIfInvalid();
			}

			CheckDuplicatesBeforeSave(instances);
		}

		protected override void ValidateBeforeDelete(ICollection<Scope> instances)
		{
			var stillInUse = GetStillInUse(instances);

			if (stillInUse.Count > 0)
			{
				var names = String.Join(", ", stillInUse
					.Select(x => x.Name)
					.OrderBy(x => x, new NaturalSortComparer()));

				throw new InvalidOperationException($"Cannot delete scopes: one or more scopes are still in use: {names}");
			}
		}

		protected internal override FilterElement<DomInstance> CreateFilter(string fieldName, Comparer comparer, object value)
		{
			switch (fieldName)
			{
				case nameof(Scope.Name):
					return FilterElementFactory.Create<string>(DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.ScopeInfo.Name), comparer, value);
			}

			return base.CreateFilter(fieldName, comparer, value);
		}

		protected internal override IOrderByElement CreateOrderBy(string fieldName, SortOrder sortOrder, bool naturalSort = false)
		{
			switch (fieldName)
			{
				case nameof(Scope.Name):
					return OrderByElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.ScopeInfo.Name), sortOrder, naturalSort);
			}

			return base.CreateOrderBy(fieldName, sortOrder, naturalSort);
		}

		private void CheckDuplicatesBeforeSave(ICollection<Scope> instances)
		{
			var duplicateNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			// Track already seen scope names
			var seen = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

			// First, check within the provided instances
			foreach (var instance in instances)
			{
				if (seen.TryGetValue(instance.Name, out var existingId) &&
					existingId != instance.ID)
				{
					duplicateNames.Add(instance.Name);
				}
				else
				{
					seen[instance.Name] = instance.ID;
				}
			}

			// Next, check against existing scopes in the repository, once per name
			foreach (var name in seen.Keys)
			{
				var existingScopes = Read(ScopeExposers.Name.Equal(name));

				foreach (var existing in existingScopes)
				{
					if (existing.ID != seen[name])
					{
						duplicateNames.Add(name);
					}
				}
			}

			// Finally, throw if any duplicates were found
			if (duplicateNames.Count > 0)
			{
				var names = String.Join(", ", duplicateNames.OrderBy(x => x, new NaturalSortComparer()));

				throw new InvalidOperationException($"Cannot save scopes. The following names are already in use: {names}");
			}
		}

		private ICollection<Scope> GetStillInUse(ICollection<Scope> instances)
		{
			var stillInUse = new HashSet<Scope>();

			foreach (var instance in instances)
			{
				if (CategoryRepository.GetByScope(instance).Any())
				{
					stillInUse.Add(instance);
				}
			}

			return stillInUse;
		}
	}
}
