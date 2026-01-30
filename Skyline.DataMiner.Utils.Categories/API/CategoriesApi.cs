namespace Skyline.DataMiner.Solutions.Categories.API
{
	using System;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Solutions.Categories.Logging;
	using Skyline.DataMiner.Solutions.Categories.Tools;
	using Skyline.DataMiner.Solutions.Categories.API.Repositories;
	using Skyline.DataMiner.Solutions.Categories.DOM.Definitions;
	using Skyline.DataMiner.Solutions.Categories.DOM.Helpers;
	using Skyline.DataMiner.Solutions.Categories.DOM.Tools;

	internal class CategoriesApi : ICategoriesApi
	{
		private readonly InstalledAppPackageCache installedAppPackages;

		private readonly Lazy<ICategoryRepository> lazyCategoryRepository;
		private readonly Lazy<ICategoryItemRepository> lazyCategoryItemRepository;
		private readonly Lazy<IScopeRepository> lazyScopeRepository;

		public CategoriesApi(IConnection connection)
		{
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));

			installedAppPackages = new InstalledAppPackageCache(connection);
			SlcCategoriesHelper = new SlcCategoriesHelper(connection);

			lazyCategoryItemRepository = new Lazy<ICategoryItemRepository>(() => new CategoryItemRepository(SlcCategoriesHelper, connection));
			lazyCategoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(SlcCategoriesHelper, CategoryItems, connection));
			lazyScopeRepository = new Lazy<IScopeRepository>(() => new ScopeRepository(SlcCategoriesHelper, Categories, connection));
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

		public void InstallDomModules(Action<string> logAction = null)
		{
			// When no logging action is provided, use a no-op.
			logAction ??= x => { };

			DomModuleInstaller.Install(Connection.HandleMessages, new SlcCategoriesDomModule(), logAction);
		}

		public bool IsInstalled(out string version)
		{
			var isInstalled = installedAppPackages.IsInstalled("SLC-LCA-Categories-Package", out var installedAppInfo);
			version = isInstalled ? installedAppInfo?.AppInfo?.Version : null;
			return isInstalled;
		}

		public bool IsInstalled()
		{
			return IsInstalled(out _);
		}
	}
}
