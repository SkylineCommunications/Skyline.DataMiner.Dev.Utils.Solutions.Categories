namespace Skyline.DataMiner.Utils.Categories.DOM.Definitions
{
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel.Concatenation;
	using Skyline.DataMiner.Net.Apps.Sections.SectionDefinitions;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.Categories.DOM.Interfaces;
	using Skyline.DataMiner.Utils.Categories.DOM.Model;

	internal class ScopeDefinition: IDomDefinitionInfo
	{
		public DomDefinition Definition { get; } = new DomDefinition("Scope")
		{
			ID = SlcCategoriesIds.Definitions.Scope,
			SectionDefinitionLinks =
			{
				new SectionDefinitionLink(SlcCategoriesIds.Sections.ScopeInfo.Id),
			},
			ModuleSettingsOverrides = new ModuleSettingsOverrides
			{
				NameDefinition = new DomInstanceNameDefinition
				{
					ConcatenationItems =
					{
						new FieldValueConcatenationItem(SlcCategoriesIds.Sections.ScopeInfo.Name),
					},
				},
			},
		};

		public IEnumerable<CustomSectionDefinition> SectionDefinitions { get; } = new[]
		{
			GetScopeInfoSectionDefinition(),
		};

		private static CustomSectionDefinition GetScopeInfoSectionDefinition()
		{
			var sectionDefinition = new CustomSectionDefinition
			{
				ID = SlcCategoriesIds.Sections.ScopeInfo.Id,
				Name = "Scope Info",
			};

			sectionDefinition.AddOrReplaceFieldDescriptor(
				new FieldDescriptor
				{
					FieldType = typeof(string),
					ID = SlcCategoriesIds.Sections.ScopeInfo.Name,
					Name = "Name",
					IsOptional = false,
				});

			return sectionDefinition;
		}
	}
}
