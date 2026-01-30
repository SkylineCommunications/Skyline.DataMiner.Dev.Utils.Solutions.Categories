namespace Skyline.DataMiner.Solutions.Categories.Tests
{
	using Skyline.DataMiner.Solutions.Categories.API;
	using Skyline.DataMiner.Utils.DOM.UnitTesting;

	internal class CategoriesApiMock : CategoriesApi
	{
		public CategoriesApiMock(bool installDomModules = true)
			: base(new DomConnectionMock(validateAgainstDefinition: true))
		{
			if (installDomModules)
			{
				InstallDomModules();
			}
		}
	}
}
