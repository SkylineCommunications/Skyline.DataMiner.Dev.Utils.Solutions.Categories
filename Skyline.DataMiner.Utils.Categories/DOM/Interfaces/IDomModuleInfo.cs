namespace Skyline.DataMiner.Solutions.Categories.DOM.Interfaces
{
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Apps.Modules;

	internal interface IDomModuleInfo
	{
		string ModuleId { get; }

		ModuleSettings ModuleSettings { get; }

		IEnumerable<IDomDefinitionInfo> Definitions { get; }
	}
}
