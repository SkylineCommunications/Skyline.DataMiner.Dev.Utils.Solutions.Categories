namespace Skyline.DataMiner.Utils.Categories.DOM.Definitions
{
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel.Settings;
	using Skyline.DataMiner.Net.Apps.Modules;
	using Skyline.DataMiner.Utils.Categories.DOM.Interfaces;
	using Skyline.DataMiner.Utils.Categories.DOM.Model;

	public class SlcCategoriesDomModule : IDomModuleInfo
	{
		public ModuleSettings ModuleSettings { get; } = new ModuleSettings(SlcCategoriesIds.ModuleId)
		{
			DomManagerSettings = new DomManagerSettings
			{
				DomInstanceHistorySettings = new DomInstanceHistorySettings
				{
					StorageBehavior = DomInstanceHistoryStorageBehavior.Disabled,
				},
				ScriptSettings = new ExecuteScriptOnDomInstanceActionSettings
				{
					ScriptType = OnDomInstanceActionScriptType.FullCrudMeta,
				},
			},
		};

		public string ModuleId => ModuleSettings.ModuleId;

		public IEnumerable<IDomDefinitionInfo> Definitions { get; } = new IDomDefinitionInfo[]
		{
			new ScopeDefinition(),
			new CategoryDefinition(),
			new CategoryItemDefinition(),
		};
	}
}
