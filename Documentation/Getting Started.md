# Getting Started

This guide will help you get started with the Skyline Categories API, a hierarchical categorization system for organizing items in DataMiner.

## Installation

Add the NuGet package to your project:

```bash
dotnet add package Skyline.DataMiner.Utils.Categories
```

## Basic Concepts

The Categories API provides three main entities:

- **Scope**: A context or domain for categories (e.g., "Devices", "Services")
- **Category**: A hierarchical classification within a scope
- **Category Item**: Links external items to categories using ModuleId and InstanceId

## Creating Your First Scope and Category

```csharp
using Skyline.DataMiner.Utils.Categories.API;
using Skyline.DataMiner.Utils.Categories.API.Objects;

// Initialize the API
var api = new CategoriesApi(connection);

// Create a scope
var scope = new Scope { Name = "Network Devices" };
api.Scopes.CreateOrUpdate([scope]);

// Create a root category
var category = new Category 
{ 
	Name = "Routers", 
	Scope = scope 
};
api.Categories.CreateOrUpdate([category]);

// Create a subcategory
var subcategory = new Category 
{ 
	Name = "Core Routers",
	Scope = scope,
	ParentCategory = category,
	RootCategory = category
};
api.Categories.CreateOrUpdate([subcategory]);
```

## Adding Items to Categories

```csharp
// Add items to a category
var items = new[]
{
	new CategoryItemIdentifier("MyModule", "Instance1"),
	new CategoryItemIdentifier("MyModule", "Instance2")
};

category.AddChildItems(api.CategoryItems, items);
```

## Querying Categories

```csharp
// Get all categories in a scope
var categories = api.Categories.GetByScope(scope);

// Query with LINQ
var results = api.Categories.Query()
	.Where(c => c.Name.Contains("Router"))
	.ToList();

// Get category hierarchy
var tree = api.Categories.GetTree(scope);
```

