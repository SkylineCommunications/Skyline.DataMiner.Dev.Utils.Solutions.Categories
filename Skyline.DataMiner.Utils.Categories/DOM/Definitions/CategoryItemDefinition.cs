namespace Skyline.DataMiner.Utils.Categories.DOM.Definitions
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Apps.Sections.SectionDefinitions;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.Categories.DOM.Interfaces;
	using Skyline.DataMiner.Utils.Categories.DOM.Model;

	internal class CategoryItemDefinition : IDomDefinitionInfo
	{
		public DomDefinition Definition { get; } = new DomDefinition("Category Item")
		{
			ID = SlcCategoriesIds.Definitions.CategoryItem,
			SectionDefinitionLinks =
			{
				new SectionDefinitionLink(SlcCategoriesIds.Sections.CategoryItemInfo.Id),
			},
		};

		public IEnumerable<CustomSectionDefinition> SectionDefinitions { get; } = new[]
		{
			GetCategoryItemInfoSectionDefinition(),
		};

		private static CustomSectionDefinition GetCategoryItemInfoSectionDefinition()
		{
			var sectionDefinition = new CustomSectionDefinition
			{
				ID = SlcCategoriesIds.Sections.CategoryItemInfo.Id,
				Name = "Category Item Info",
			};

			sectionDefinition.AddOrReplaceFieldDescriptor(
				new DomInstanceFieldDescriptor(SlcCategoriesIds.ModuleId)
				{
					FieldType = typeof(Guid),
					ID = SlcCategoriesIds.Sections.CategoryItemInfo.Category,
					Name = "Category",
					DomDefinitionIds = { SlcCategoriesIds.Definitions.Category },
					IsOptional = false,
				});

			sectionDefinition.AddOrReplaceFieldDescriptor(
				new FieldDescriptor
				{
					FieldType = typeof(string),
					ID = SlcCategoriesIds.Sections.CategoryItemInfo.ModuleID,
					Name = "Module ID",
					IsOptional = false,
				});

			sectionDefinition.AddOrReplaceFieldDescriptor(
				new FieldDescriptor
				{
					FieldType = typeof(string),
					ID = SlcCategoriesIds.Sections.CategoryItemInfo.InstanceID,
					Name = "Instance ID",
					IsOptional = false,
				});

			return sectionDefinition;
		}
	}
}
