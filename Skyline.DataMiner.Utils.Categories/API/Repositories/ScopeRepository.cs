namespace Skyline.DataMiner.Solutions.Categories.API.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Solutions.Categories.API;
	using Skyline.DataMiner.Solutions.Categories.API.Tools;
	using Skyline.DataMiner.Solutions.Categories.DOM.Helpers;
	using Skyline.DataMiner.Solutions.Categories.DOM.Model;
	using Skyline.DataMiner.Solutions.Categories.DOM.Tools;

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

		public override Scope CreateInstance(DomInstance domInstance)
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
			// Keep track of seen (name, id) pairs
			var seen = new HashSet<(string Name, Guid ID)>();

			// Retrieve existing scopes with the same names as the instances being saved
			var existingScopes = FilterQueryExecutor.RetrieveFilteredItems(
				instances.Select(x => x.Name).Distinct(),
				name => ScopeExposers.Name.Equal(name),
				Read);

			// Add both existing scopes and the instances being saved
			foreach (var existing in existingScopes.Concat(instances))
			{
				seen.Add((existing.Name, existing.ID));
			}

			// Now, find duplicates
			var duplicateNames = seen.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
				.Where(g => g.Count() > 1)
				.Select(g => g.Key)
				.ToList();

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

		public IEnumerable<Scope> Read()
		{
			return Read(new TRUEFilterElement<Scope>());
		}
	}
}
