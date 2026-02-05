namespace Skyline.DataMiner.Solutions.Categories.API
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.ManagerStore;
	using Skyline.DataMiner.Solutions.Categories.DOM.Definitions;
	using Skyline.DataMiner.Solutions.Categories.DOM.Helpers;
	using Skyline.DataMiner.Solutions.Categories.DOM.Model;
	using Skyline.DataMiner.Solutions.Categories.DOM.Tools;
	using Skyline.DataMiner.Solutions.Categories.Logging;

	internal class CategoriesApi : ICategoriesApi
	{
		private readonly Lazy<CategoryRepository> lazyCategoryRepository;
		private readonly Lazy<CategoryItemRepository> lazyCategoryItemRepository;
		private readonly Lazy<ScopeRepository> lazyScopeRepository;

		public CategoriesApi(IConnection connection)
		{
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));
			SlcCategoriesHelper = new SlcCategoriesHelper(connection);

			lazyCategoryItemRepository = new Lazy<CategoryItemRepository>(() => new CategoryItemRepository(SlcCategoriesHelper, connection));
			lazyCategoryRepository = new Lazy<CategoryRepository>(() => new CategoryRepository(SlcCategoriesHelper, lazyCategoryItemRepository.Value, connection));
			lazyScopeRepository = new Lazy<ScopeRepository>(() => new ScopeRepository(SlcCategoriesHelper, lazyCategoryRepository.Value, connection));
		}

		protected internal IConnection Connection { get; }

		internal ILogger Logger { get; private set; } = new NullLogger();

		internal SlcCategoriesHelper SlcCategoriesHelper { get; }

		public ICategoryRepository Categories => lazyCategoryRepository.Value;

		public ICategoryItemRepository CategoryItems => lazyCategoryItemRepository.Value;

		public IScopeRepository Scopes => lazyScopeRepository.Value;

		public void SetLogger(ILogger logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public void InstallDomModules()
		{
			var domInstaller = new DomModuleInstaller(this);
			domInstaller.Install(new SlcCategoriesDomModule());
		}

		public bool IsInstalled(out string version)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			version = !String.IsNullOrWhiteSpace(ThisAssembly.Git.Tag) ? ThisAssembly.Git.Tag : $"{ThisAssembly.Git.SemVer.Major}.{ThisAssembly.Git.SemVer.Minor}.{ThisAssembly.Git.SemVer.Patch}{ThisAssembly.Git.SemVer.DashLabel}";
#pragma warning restore CS0618 // Type or member is obsolete

			var definitions = SlcCategoriesHelper.DomHelper.DomDefinitions.ReadAll();

			return definitions
				.Select(x => x.ID)
				.Contains(SlcCategoriesIds.Definitions.Category);
		}

		public bool IsInstalled()
		{
			return IsInstalled(out _);
		}
	}
}
