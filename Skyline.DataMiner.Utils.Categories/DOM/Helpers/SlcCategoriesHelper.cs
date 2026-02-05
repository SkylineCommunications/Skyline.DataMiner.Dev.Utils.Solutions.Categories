namespace Skyline.DataMiner.Solutions.Categories.DOM.Helpers
{
	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Solutions.Categories.DOM.Model;

	internal class SlcCategoriesHelper : DomModuleHelperBase
	{
		public SlcCategoriesHelper(IConnection connection) : base(SlcCategoriesIds.ModuleId, connection)
		{
		}
	}
}
