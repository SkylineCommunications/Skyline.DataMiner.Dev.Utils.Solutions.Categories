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
	using Skyline.DataMiner.Utils.Categories.DOM.Tools;

	using SLDataGateway.API.Types.Querying;

	public class ScopeRepository : Repository<Scope>
	{
		internal ScopeRepository(SlcCategoriesHelper helper, IConnection connection) : base(helper, connection)
		{
		}

		protected internal override DomDefinitionId DomDefinition => Scope.DomDefinition;

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
			try
			{
				CheckIfStillInUse(instances);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Cannot delete scopes: {ex.Message}", ex);
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
			FilterElement<DomInstance> CreateFilter(Scope c) =>
				new ANDFilterElement<DomInstance>(
					DomInstanceExposers.Id.NotEqual(c.ID),
					DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.ScopeInfo.Name).Equal(c.Name));

			var conflicts = FilterQueryExecutor.RetrieveFilteredItems(instances, CreateFilter, Read).ToList();

			if (conflicts.Count > 0)
			{
				var names = String.Join(", ", conflicts
					.Select(x => x.Name)
					.OrderBy(x => x, new NaturalSortComparer()));

				throw new InvalidOperationException($"Cannot save scopes. The following names are already in use: {names}");
			}
		}

		private void CheckIfStillInUse(ICollection<Scope> instances)
		{
			FilterElement<DomInstance> CreateFilter(Scope s) =>
				new ANDFilterElement<DomInstance>(
					DomInstanceExposers.DomDefinitionId.Equal(SlcCategoriesIds.Definitions.Category.Id),
					DomInstanceExposers.FieldValues.DomInstanceField(SlcCategoriesIds.Sections.CategoryInfo.Scope).Equal(s.ID));

			var filter = new ORFilterElement<DomInstance>(instances.Select(CreateFilter).ToArray());
			var count = Helper.DomInstances.Count(filter);

			if (count > 0)
			{
				throw new InvalidOperationException("One or more scopes are still in use");
			}
		}
	}
}
