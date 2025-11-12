# Examples

## Basic CRUD Operations

### Creating Scopes and Categories

```csharp
var api = new CategoriesApi(connection);

// Create a scope
var scope = new Scope { Name = "IT Infrastructure" };
api.Scopes.CreateOrUpdate([scope]);

// Create categories in batch
var categories = new[]
{
    new Category { Name = "Servers", Scope = scope },
    new Category { Name = "Network", Scope = scope },
    new Category { Name = "Storage", Scope = scope }
};
api.Categories.CreateOrUpdate(categories);
```

### Building a Category Hierarchy

```csharp
// Create root category
var devices = new Category { Name = "Devices", Scope = scope };
api.Categories.CreateOrUpdate([devices]);

// Create subcategories
var routers = new Category 
{ 
    Name = "Routers",
    Scope = scope,
 ParentCategory = devices,
    RootCategory = devices
};

var switches = new Category 
{ 
    Name = "Switches",
    Scope = scope,
    ParentCategory = devices,
    RootCategory = devices
};

api.Categories.CreateOrUpdate([routers, switches]);

// Create nested subcategory
var coreRouters = new Category 
{ 
    Name = "Core Routers",
    Scope = scope,
    ParentCategory = routers,
    RootCategory = devices
};
api.Categories.CreateOrUpdate([coreRouters]);
```

## Managing Category Items

### Adding Items to Categories

```csharp
// Add single item
var item = new CategoryItem
{
    Category = category,
    ModuleId = "Protocol",
    InstanceId = "Element123"
};
api.CategoryItems.CreateOrUpdate([item]);

// Add multiple items using identifiers
var items = new[]
{
    new CategoryItemIdentifier("Protocol", "Element1"),
    new CategoryItemIdentifier("Protocol", "Element2"),
    new CategoryItemIdentifier("Protocol", "Element3")
};

category.AddChildItems(api.CategoryItems, items);
```

### Removing Items

```csharp
// Remove specific items
var itemsToRemove = new[]
{
    new CategoryItemIdentifier("Protocol", "Element1")
};

category.RemoveChildItems(api.CategoryItems, itemsToRemove);

// Clear all items from a category
category.ClearChildItems(api.CategoryItems);
```

### Replacing Items

```csharp
// Replace all items in a category
var newItems = new[]
{
    new CategoryItemIdentifier("Protocol", "NewElement1"),
    new CategoryItemIdentifier("Protocol", "NewElement2")
};

category.ReplaceChildItems(api.CategoryItems, newItems);
```

## Querying

### Basic Queries

```csharp
// Get all categories
var allCategories = api.Categories.ReadAll();

// Get category by ID
var category = api.Categories.Read(categoryId);

// Get category by name
var category = api.Categories.Read("Routers");

// Get multiple by IDs
var ids = new[] { id1, id2, id3 };
var categories = api.Categories.Read(ids);
```

### LINQ Queries

```csharp
// Query with Where
var results = api.Categories.Query()
    .Where(c => c.Name.StartsWith("Router"))
    .ToList();

// Count categories
var count = api.Categories.Query()
    .Where(c => c.Scope == scope)
  .Count();

// Order categories
var sorted = api.Categories.Query()
    .OrderBy(c => c.Name)
    .ToList();

// Complex query
var results = api.Categories.Query()
    .Where(c => c.Scope == scope)
    .Where(c => c.ParentCategory != null)
  .OrderBy(c => c.Name)
    .Take(10)
    .ToList();
```

### Using Filters

```csharp
using Skyline.DataMiner.Utils.Categories.API.Objects;

// Filter by scope
var categories = api.Categories.GetByScope(scope);

// Filter by root category
var descendants = api.Categories.GetByRootCategory(rootCategory);

// Get child categories
var children = api.Categories.GetChildCategories(parentCategory);

// Get all descendants (recursive)
var allDescendants = api.Categories.GetDescendantCategories(parentCategory);
```

## Working with Hierarchies

### Navigate Category Tree

```csharp
// Get full tree for a scope
var tree = api.Categories.GetTree(scope);

// Get child categories
foreach (var childNode in tree.ChildCategories)
{
    Console.WriteLine($"Category: {childNode.Category.Name}");
    
    // Get items in this category
 foreach (var item in childNode.ChildItems)
    {
        Console.WriteLine($"  - {item.ModuleId}/{item.InstanceId}");
    }
}

// Find a specific category in the tree
if (tree.TryFindCategory(categoryId, out var node))
{
    Console.WriteLine($"Found: {node.Category.Name}");
}
```

### Get Ancestor Path

```csharp
// Get path from root to category
var path = api.Categories.GetAncestorPath(category);

// Display path
var pathString = string.Join(" > ", path.Select(c => c.Name));
Console.WriteLine($"Path: {pathString}");
// Output: "Devices > Routers > Core Routers"

// Or use the extension method
var path = category.GetAncestorPath(api.Categories);
```

## Validation

### Validate Before Saving

```csharp
var category = new Category 
{ 
    Name = "Test",
    Scope = scope 
};

// Validate manually
var validationResult = category.Validate();
if (!validationResult.IsValid)
{
    foreach (var error in validationResult.Errors)
    {
        Console.WriteLine($"Error: {error.Message}");
    }
    return;
}

// Or let CreateOrUpdate validate automatically
try
{
    api.Categories.CreateOrUpdate([category]);
}
catch (Exception ex)
{
    Console.WriteLine($"Validation failed: {ex.Message}");
}
```

## Extension Methods

### Convert to Tree

```csharp
using Skyline.DataMiner.Utils.Categories.API.Extensions;

// Convert a list of categories to a tree structure
var categories = api.Categories.GetByScope(scope);
var tree = categories.ToTree();
```

### Sort Hierarchically

```csharp
// Sort categories in hierarchical order (parents before children)
var sortedCategories = categories.SortHierarchically();
```

## Deleting Objects

### Delete Categories

```csharp
// Delete a single category
api.Categories.Delete(category);

// Delete multiple categories
api.Categories.Delete([category1, category2, category3]);

// Note: Deleting a category automatically deletes all its child items
```

### Delete Scopes

```csharp
// Can only delete scopes that have no categories
try
{
    api.Scopes.Delete(scope);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Cannot delete: {ex.Message}");
    // First delete all categories in the scope
}
```

## Reading Category Items

```csharp
// Get all items in a category
var items = category.GetChildItems(api.CategoryItems);

// Get items from repository
var items = api.CategoryItems.GetChildItems(category);

// Query items
var items = api.CategoryItems.Query()
    .Where(i => i.Category == category)
    .Where(i => i.ModuleId == "Protocol")
    .ToList();
```
