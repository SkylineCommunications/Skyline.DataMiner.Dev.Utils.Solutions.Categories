# Skyline.DataMiner.Utils.Categories

## About

The Categories API provides a flexible way to organize and categorize items using a hierarchical tree structure. It consists of three main components:
This API is part of the [DataMiner Categories](https://catalog.dataminer.services/details/c9666f3a-be26-42fd-83f2-6ee7fab4f11e) application, and can be used by other solutions to manage categories via code.

- **Scopes**: Define contexts for categories (e.g., "Resource Studio", "Virtual Signal Groups")
- **Categories**: Provides hierarchical structure for grouping items. Categories can be nested to form a tree structure
- **Category Items**: Link external items to categories using ModuleId and InstanceId

> [!NOTE]
> ModuleId and InstanceId are strings that uniquely identify items within DataMiner.
> These typically refer to DOM instances, but can represent any item type.

### Key Features

- Hierarchical category trees with unlimited depth
- Multiple scopes for different contexts
- LINQ-based querying
- Built-in caching for optimal performance
- Real-time subscriptions to data changes
- Batch operations for efficiency
- Strongly-typed API

## Quick Start

```csharp
using Skyline.DataMiner.Utils.Categories.API;
using Skyline.DataMiner.Utils.Categories.API.Objects;

// Initialize the API
var api = new CategoriesApi(connection);

// Create a scope
var scope = new Scope { Name = "Network Devices" };
api.Scopes.CreateOrUpdate([scope]);

// Create categories
var routers = new Category { Name = "Routers", Scope = scope };
api.Categories.CreateOrUpdate([routers]);

// Add items
var items = new[]
{
	new CategoryItemIdentifier("Protocol", "Router-01"),
    new CategoryItemIdentifier("Protocol", "Router-02")
};
routers.AddChildItems(api.CategoryItems, items);

// Query with LINQ
var results = api.Categories.Query()
    .Where(c => c.Name.Contains("Router"))
    .ToList();
```

## Documentation

- **[Getting Started](Documentation/Getting%20Started.md)** - Installation and basic usage
- **[Quick Reference](Documentation/Quick%20Reference.md)** - Cheat sheet with common code snippets
- **[Core Concepts](Documentation/Core%20Concepts.md)** - Understanding the data model
- **[Examples](Documentation/Examples.md)** - Common usage patterns
- **[Caching](Documentation/Caching.md)** - Improve performance with caching
- **[Advanced Topics](Documentation/Advanced%20Topics.md)** - Subscriptions, validation, and more
- **[Data Model](Documentation/Data%20Model.md)** - Entity relationship diagram

## Installation

```bash
dotnet add package Skyline.DataMiner.Utils.Categories
```

## Basic Examples

### Creating a Category Hierarchy

```csharp
var devices = new Category { Name = "Devices", Scope = scope };
var routers = new Category 
{ 
    Name = "Routers",
    Scope = scope,
    ParentCategory = devices,
    RootCategory = devices
};

api.Categories.CreateOrUpdate([devices, routers]);
```

### Querying Categories

```csharp
// Get all categories in a scope
var categories = api.Categories.GetByScope(scope);

// Get category tree
var tree = api.Categories.GetTree(scope);

// Get ancestor path
var path = api.Categories.GetAncestorPath(category);
```

### Using Cache for Performance

```csharp
using Skyline.DataMiner.Utils.Categories.API.Extensions;

var cache = connection.GetStaticCategoriesCache().Cache;

// Fast O(1) lookup by name
var scope = cache.GetScope("Network Devices");

// Get categories for scope
var categories = cache.Cache.GetCategoriesForScope(scope);
```

## License

See the LICENSE file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.