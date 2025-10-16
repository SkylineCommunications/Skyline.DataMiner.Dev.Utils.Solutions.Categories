namespace Skyline.DataMiner.Utils.Categories.DOM.Helpers
{
	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Utils.Categories.DOM.Model;

	internal class SlcCategoriesHelper : DomModuleHelperBase
	{
		public SlcCategoriesHelper(IConnection connection) : base(SlcCategoriesIds.ModuleId, connection)
		{
		}
	}
}
