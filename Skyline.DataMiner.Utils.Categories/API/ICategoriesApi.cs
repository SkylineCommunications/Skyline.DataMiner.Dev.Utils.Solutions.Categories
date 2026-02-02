namespace Skyline.DataMiner.Solutions.Categories.API
{
	using Skyline.DataMiner.Solutions.Categories.Logging;

	public interface ICategoriesApi
	{
		ICategoryRepository Categories { get; }

		ICategoryItemRepository CategoryItems { get; }

		IScopeRepository Scopes { get; }

		void InstallDomModules();

		bool IsInstalled();

		bool IsInstalled(out string version);

		void SetLogger(ILogger logger);
	}
}