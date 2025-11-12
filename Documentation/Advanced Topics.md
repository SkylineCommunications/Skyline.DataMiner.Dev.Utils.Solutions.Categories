# Advanced Topics

## Subscriptions

Subscribe to real-time changes in the repository:

```csharp
// Subscribe to all categories
using var subscription = api.Categories.Subscribe();

subscription.Changed += (sender, e) =>
{
    Console.WriteLine($"Categories changed:");
    Console.WriteLine($"  Created: {e.Created.Count}");
    Console.WriteLine($"  Updated: {e.Updated.Count}");
    Console.WriteLine($"  Deleted: {e.Deleted.Count}");
    
    foreach (var category in e.Created)
    {
    Console.WriteLine($"  New category: {category.Name}");
    }
};

// Keep subscription alive
Thread.Sleep(60000);
```

### Filtered Subscriptions

Subscribe only to specific items:

```csharp
// Subscribe to categories in a specific scope
var filter = CategoryExposers.Scope.Equal(scopeId);
using var subscription = api.Categories.Subscribe(filter);

subscription.Changed += (sender, e) =>
{
    // Handle changes for this scope only
};
```

## Validation

### Custom Validation

```csharp
var category = new Category 
{ 
    Name = "Test Category",
    Scope = scope
};

var result = category.Validate();

if (!result.IsValid)
{
    // Get all errors
  foreach (var error in result.Errors)
    {
      Console.WriteLine($"{error.PropertyName}: {error.Message}");
    }
    
    // Get errors for specific property
    var nameErrors = result.ForProperty(c => c.Name);
    if (!nameErrors.IsValid)
    {
        Console.WriteLine($"Name is invalid: {nameErrors.Errors.First().Message}");
    }
}
```

### Validation Rules

The API enforces these validation rules:

**Scope:**
- Name is required
- Name must be unique

**Category:**
- Name is required
- Scope is required
- If ParentCategory is set, RootCategory must also be set
- Category names must be unique within the same parent

**Category Item:**
- Category is required
- ModuleId is required

## Pagination

For large datasets, use pagination:

```csharp
// Read all categories in pages of 100
foreach (var page in api.Categories.ReadAllPaged(pageSize: 100))
{
    foreach (var category in page)
    {
 Console.WriteLine($"Category: {category.Name}");
    }
}

// Paginate with a filter
var filter = CategoryExposers.Scope.Equal(scopeId);
foreach (var page in api.Categories.ReadPaged(filter, pageSize: 50))
{
    ProcessPage(page);
}

// Paginate with a query
var query = CategoryExposers.Scope.Equal(scopeId).ToQuery()
    .OrderBy(CategoryExposers.Name.Ascending());

foreach (var page in api.Categories.ReadPaged(query, pageSize: 50))
{
    ProcessPage(page);
}
```

## Batch Operations

Perform operations in batch for better performance:

```csharp
// Create multiple categories at once
var categories = new[]
{
new Category { Name = "Category 1", Scope = scope },
    new Category { Name = "Category 2", Scope = scope },
    new Category { Name = "Category 3", Scope = scope }
};

var created = api.Categories.CreateOrUpdate(categories);

// Delete multiple categories
api.Categories.Delete(categories);
```

## Using Exposers

Exposers provide strongly-typed access to fields for filtering and sorting:

```csharp
using Skyline.DataMiner.Utils.Categories.API.Objects;

// Filter using exposers
var filter = CategoryExposers.Scope.Equal(scopeId)
    .AND(CategoryExposers.Name.Contains("Router"));

var categories = api.Categories.Read(filter);

// Sort using exposers
var query = new TRUEFilterElement<Category>().ToQuery()
    .OrderBy(CategoryExposers.Name.Ascending())
 .ThenBy(CategoryExposers.ID.Ascending());

var sorted = api.Categories.Read(query);
```

## Installation Check

Check if the DOM modules are installed:

```csharp
var api = new CategoriesApi(connection);

if (!api.IsInstalled())
{
    Console.WriteLine("Installing DOM modules...");
    api.InstallDomModules();
    Console.WriteLine("Installation complete.");
}
else
{
Console.WriteLine("Already installed.");
    var version = api.GetVersion();
    Console.WriteLine($"Version: {version}");
}
```

## Working with References

### Converting Between Types

```csharp
// From object to reference
Category category = GetCategory();
ApiObjectReference<Category> reference = category;
// or explicitly: category.Reference

// From reference to ID
Guid id = reference.ID;

// From ID to reference
ApiObjectReference<Category> reference = new ApiObjectReference<Category>(id);
// or implicitly: ApiObjectReference<Category> reference = id;

// Check for empty reference
if (reference == ApiObjectReference<Category>.Empty)
{
    Console.WriteLine("Reference is empty");
}
```

### Nullable References

```csharp
// ParentCategory is nullable
ApiObjectReference<Category>? parentRef = category.ParentCategory;

if (parentRef.HasValue && parentRef.Value != ApiObjectReference<Category>.Empty)
{
    var parent = api.Categories.Read(parentRef.Value.ID);
}

// Or more concise
if (category.ParentCategory != ApiObjectReference<Category>.Empty)
{
    var parent = api.Categories.Read(category.ParentCategory.Value.ID);
}
```

## Extension Methods

### Connection Extensions

```csharp
using Skyline.DataMiner.Utils.Categories.API.Extensions;

// Get API directly from connection
var api = connection.GetCategoriesApi();

// Get static cache from connection
var cache = connection.GetStaticCategoriesCache();
```

### Category Extensions

```csharp
using Skyline.DataMiner.Utils.Categories.API.Extensions;

// Sort categories hierarchically
var sorted = categories.SortHierarchically();

// Convert to tree
var tree = categories.ToTree();
```

## Error Handling

```csharp
try
{
    // Validation errors
    var category = new Category { Name = "", Scope = scope };
    api.Categories.CreateOrUpdate([category]);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Validation error: {ex.Message}");
}

try
{
    // Duplicate name error
    api.Categories.CreateOrUpdate([category1, category2]);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Duplicate names: {ex.Message}");
}

try
{
    // Cannot delete scope with categories
    api.Scopes.Delete(scope);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Cannot delete: {ex.Message}");
}

try
{
    // Item not found
    var category = cache.GetCategory(nonExistentId);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Not found: {ex.Message}");
}
```

## Performance Tips

1. **Use caching** for read-heavy workloads
2. **Use batch operations** when creating/updating multiple items
3. **Use pagination** for large result sets
4. **Use LINQ queries** instead of loading all items and filtering in memory
5. **Use references** instead of full objects when you only need the ID
6. **Subscribe with filters** to reduce subscription overhead

## Troubleshooting

### Circular References

The API detects circular references in the hierarchy:

```csharp
try
{
    var path = api.Categories.GetAncestorPath(category);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Circular reference detected: {ex.Message}");
}
```

### Missing Categories

```csharp
// Check if category exists before using it
if (cache.TryGetCategory(categoryId, out var category))
{
    // Use category
}
else
{
    Console.WriteLine("Category not found");
}
```
