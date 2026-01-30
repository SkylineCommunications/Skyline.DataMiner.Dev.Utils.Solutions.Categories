namespace Skyline.DataMiner.Solutions.Categories.API
{
	using Skyline.DataMiner.Solutions.Categories.Logging;

	public interface ICategoriesApi
	{
		/// <summary>
		/// Gets the repository for category items.
		/// </summary>
		ICategoryItemRepository CategoryItems { get; }

		/// <summary>
		/// Gets the repository for categories.
		/// </summary>
		ICategoryRepository Categories { get; }

		/// <summary>
		/// Gets the repository for scopes.
		/// </summary>
		IScopeRepository Scopes { get; }

		/// <summary>
		/// Determines whether the Categories application is installed on the DataMiner System.
		/// </summary>
		/// <param name="version">
		/// When this method returns <c>true</c>, contains the version of the installed application;
		/// otherwise, <c>null</c>.
		/// </param>
		/// <returns>
		/// <c>true</c> if the application is installed; otherwise, <c>false</c>.
		/// </returns>
		bool IsInstalled(out string version);

		/// <summary>
		/// Determines whether the Categories application is installed on the DataMiner System.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the application is installed; otherwise, <c>false</c>.
		/// </returns>
		bool IsInstalled();

		/// <summary>
		/// Sets the logger to be used by the API.
		/// </summary>
		/// <param name="logger">The logger.</param>
		void SetLogger(ILogger logger);
	}
}
