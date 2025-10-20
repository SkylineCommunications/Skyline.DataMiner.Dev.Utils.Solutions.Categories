namespace Skyline.DataMiner.Utils.Categories.DOM.Definitions
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel.Concatenation;
	using Skyline.DataMiner.Net.Apps.Sections.SectionDefinitions;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.Categories.DOM.Interfaces;
	using Skyline.DataMiner.Utils.Categories.DOM.Model;

	internal class CategoryDefinition : IDomDefinitionInfo
	{
		public DomDefinition Definition { get; } = new DomDefinition("Category")
		{
			ID = SlcCategoriesIds.Definitions.Category,
			SectionDefinitionLinks =
			{
				new SectionDefinitionLink(SlcCategoriesIds.Sections.CategoryInfo.Id),
			},
			ModuleSettingsOverrides = new ModuleSettingsOverrides
			{
				NameDefinition = new DomInstanceNameDefinition
				{
					ConcatenationItems =
					{
						new FieldValueConcatenationItem(SlcCategoriesIds.Sections.CategoryInfo.Name),
					},
				},
			},
		};

		public IEnumerable<CustomSectionDefinition> SectionDefinitions { get; } = new[]
		{
			GetCategoryInfoSectionDefinition(),
			GetCategoryItemSectionDefinition(),
		};

		private static CustomSectionDefinition GetCategoryInfoSectionDefinition()
		{
			var sectionDefinition = new CustomSectionDefinition
			{
				ID = SlcCategoriesIds.Sections.CategoryInfo.Id,
				Name = "Category Info",
			};

			sectionDefinition.AddOrReplaceFieldDescriptor(
				new FieldDescriptor
				{
					FieldType = typeof(string),
					ID = SlcCategoriesIds.Sections.CategoryInfo.Name,
					Name = "Name",
					IsOptional = false,
				});

			sectionDefinition.AddOrReplaceFieldDescriptor(
				new DomInstanceFieldDescriptor(SlcCategoriesIds.ModuleId)
				{
					FieldType = typeof(Guid),
					ID = SlcCategoriesIds.Sections.CategoryInfo.ParentCategory,
					Name = "Parent Category",
					DomDefinitionIds = { SlcCategoriesIds.Definitions.Category },
					IsOptional = true,
				});

			sectionDefinition.AddOrReplaceFieldDescriptor(
				new DomInstanceFieldDescriptor(SlcCategoriesIds.ModuleId)
				{
					FieldType = typeof(Guid),
					ID = SlcCategoriesIds.Sections.CategoryInfo.Scope,
					Name = "Scope",
					DomDefinitionIds = { SlcCategoriesIds.Definitions.Scope },
					IsOptional = true,
				});

			return sectionDefinition;
		}

		private static CustomSectionDefinition GetCategoryItemSectionDefinition()
		{
			var sectionDefinition = new CustomSectionDefinition
			{
				ID = SlcCategoriesIds.Sections.CategoryItem.Id,
				Name = "Category Item",
			};

			sectionDefinition.AddOrReplaceFieldDescriptor(
				new FieldDescriptor
				{
					FieldType = typeof(string),
					ID = SlcCategoriesIds.Sections.CategoryItem.ModuleID,
					Name = "Module ID",
					IsOptional = false,
				});

			sectionDefinition.AddOrReplaceFieldDescriptor(
				new FieldDescriptor
				{
					FieldType = typeof(string),
					ID = SlcCategoriesIds.Sections.CategoryItem.InstanceID,
					Name = "Instance ID",
					IsOptional = false,
				});

			return sectionDefinition;
		}
	}
}
