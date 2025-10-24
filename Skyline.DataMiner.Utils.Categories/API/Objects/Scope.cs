namespace Skyline.DataMiner.Utils.Categories.API.Objects
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.Categories.API.Repositories;
	using Skyline.DataMiner.Utils.Categories.API.Tools;
	using Skyline.DataMiner.Utils.Categories.API.Validation;
	using Skyline.DataMiner.Utils.Categories.DOM.Model;

	public class Scope : ApiObject<Scope>
	{
		private readonly ScopeInstance _domInstance;

		public Scope() : this(new ScopeInstance())
		{
		}

		internal Scope(ScopeInstance domInstance) : base(domInstance)
		{
			_domInstance = domInstance ?? throw new ArgumentNullException(nameof(domInstance));
		}

		internal Scope(DomInstance domInstance) : this(new ScopeInstance(domInstance))
		{
		}

		internal static DomDefinitionId DomDefinition => SlcCategoriesIds.Definitions.Scope;

		public string Name
		{
			get
			{
				return _domInstance.ScopeInfo.Name;
			}

			set
			{
				_domInstance.ScopeInfo.Name = value;
			}
		}

		public IEnumerable<Category> GetCategories(CategoryRepository categoryRepository)
		{
			if (categoryRepository is null)
			{
				throw new ArgumentNullException(nameof(categoryRepository));
			}

			return categoryRepository.GetByScope(this);
		}

		public CategoryNode GetCategoriesTree(CategoryRepository categoryRepository)
		{
			if (categoryRepository is null)
			{
				throw new ArgumentNullException(nameof(categoryRepository));
			}

			return categoryRepository.GetTree(this);
		}

		public ValidationResult Validate()
		{
			var result = new ValidationResult();

			if (!NameUtil.Validate(Name, out var error))
			{
				result.AddError(error, this, x => x.Name);
			}

			return result;
		}
	}

	public static class ScopeExposers
	{
		public static readonly Exposer<Scope, Guid> ID = new(x => x.ID, nameof(Scope.ID));
		public static readonly Exposer<Scope, string> Name = new(x => x.Name, nameof(Scope.Name));
	}
}
