namespace Skyline.DataMiner.Utils.Categories.API
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.Apps.Modules;
	using Skyline.DataMiner.Net.ManagerStore;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.Categories.API.Repositories;
	using Skyline.DataMiner.Utils.Categories.DOM.Definitions;
	using Skyline.DataMiner.Utils.Categories.DOM.Helpers;
	using Skyline.DataMiner.Utils.Categories.DOM.Model;
	using Skyline.DataMiner.Utils.Categories.DOM.Tools;

	public class CategoriesApi
	{
		public CategoriesApi(IConnection connection)
		{
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));

			SlcCategoriesHelper = new SlcCategoriesHelper(connection);

			CategoryItems = new CategoryItemRepository(SlcCategoriesHelper, connection);
			Categories = new CategoryRepository(SlcCategoriesHelper, CategoryItems, connection);
			Scopes = new ScopeRepository(SlcCategoriesHelper, Categories, connection);
		}

		protected internal IConnection Connection { get; }

		internal SlcCategoriesHelper SlcCategoriesHelper { get; }

		public CategoryRepository Categories { get; }

		public CategoryItemRepository CategoryItems { get; }

		public ScopeRepository Scopes { get; }

		public void InstallDomModules(Action<string> logAction = null)
		{
			// When no logging action is provided, use a no-op.
			logAction ??= x => { };

			DomModuleInstaller.Install(Connection.HandleMessages, new SlcCategoriesDomModule(), logAction);
		}

		public bool IsInstalled()
		{
			var moduleSettingsHelper = new ModuleSettingsHelper(Connection.HandleMessages);

			var definitions = SlcCategoriesHelper.DomHelper.DomDefinitions.ReadAll();

			return definitions
				.Select(x => x.ID)
				.Contains(SlcCategoriesIds.Definitions.Category);
		}

		public string GetVersion()
		{
#pragma warning disable CS0618 // Type or member is obsolete
			if (!String.IsNullOrWhiteSpace(ThisAssembly.Git.Tag))
			{
				return ThisAssembly.Git.Tag;
			}

			return $"{ThisAssembly.Git.SemVer.Major}.{ThisAssembly.Git.SemVer.Minor}.{ThisAssembly.Git.SemVer.Patch}{ThisAssembly.Git.SemVer.DashLabel}";
#pragma warning restore CS0618 // Type or member is obsolete
		}
	}
}
