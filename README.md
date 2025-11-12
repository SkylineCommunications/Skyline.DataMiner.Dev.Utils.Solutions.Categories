# Skyline.DataMiner.Utils.Categories

## About

The Categories API provides a flexible way to organize and categorize items using a hierarchical tree structure. It consists of three main components:
This API is part of the [DataMiner Categories](https://catalog.dataminer.services/details/c9666f3a-be26-42fd-83f2-6ee7fab4f11e) application, and can be used by other solutions to manage categories via code.

## Main Components

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
api.CategoryItems.AddChildItems(router, items);

// Query with LINQ
var results = api.Categories.Query()
    .Where(c => c.Name.Contains("Router"))
    .ToList();
```

## Documentation

- **[Getting Started](Documentation/Getting Started.md)** - Installation and basic usage
- **[Quick Reference](Documentation/Quick Reference.md)** - Cheat sheet with common code snippets
- **[Core Concepts](Documentation/Core Concepts.md)** - Understanding the data model
- **[Examples](Documentation/Examples.md)** - Common usage patterns
- **[Advanced Topics](Documentation/Advanced Topics.md)** - Subscriptions, validation, and more

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
var categories = cache.GetCategoriesForScope(scope);
```

## About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

The foundation of DataMiner is its powerful and versatile data acquisition and control layer. With DataMiner, there are no restrictions to what data users can access. Data sources may reside on premises, in the cloud, or in a hybrid setup.

A unique catalog of 7000+ connectors already exists. In addition, you can leverage DataMiner Development Packages to build your own connectors (also known as "protocols" or "drivers").

> **Note**
> See also: [About DataMiner](https://aka.dataminer.services/about-dataminer).

## About Skyline Communications

At Skyline Communications, we deal with world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.
