# Advanced Topics

## Caching

The Categories API provides built-in caching capabilities to improve performance when reading data frequently.

### Overview

The caching system consists of two main components:

- **CategoriesCache**: In-memory cache of all categories data
- **CategoriesObserver**: Automatically updates the cache when data changes

### Using the Static Cache

The simplest way to use caching is with the static cache:

```csharp
using Skyline.DataMiner.Utils.Categories.API.Caching;
using Skyline.DataMiner.Utils.Categories.API.Extensions;

// Get or create the static cache
var staticCache = connection.GetStaticCategoriesCache();

// Access the cache
var cache = staticCache.Cache;

// Get a scope by name (O(1) lookup)
if (cache.TryGetScope("Network Infrastructure", out var scope))
{
    Console.WriteLine($"Found scope: {scope.Name}");
}

// Get categories for a scope
var categories = cache.GetCategoriesForScope(scope);

// Get category by ID
var category = cache.GetCategory(categoryId);
```

### Creating Your Own Cache

For more control, create your own cache instance:

```csharp
var api = new CategoriesApi(connection);
var cache = new CategoriesCache();

// Load initial data
cache.LoadInitialData(api);

// Now use the cache
var scopes = cache.Scopes.Values;
var categories = cache.Categories.Values;
```

### Using the Observer for Auto-Updates

The `CategoriesObserver` keeps the cache synchronized with database changes:

```csharp
var api = new CategoriesApi(connection);
var cache = new CategoriesCache();
var observer = new CategoriesObserver(api, cache);

// Subscribe to changes
observer.Subscribe();

// Handle events (optional)
observer.ScopesChanged += (sender, e) =>
{
	Console.WriteLine($"Scopes changed: {e.Created.Count} created, " +
		$"{e.Updated.Count} updated, {e.Deleted.Count} deleted");
};

observer.CategoriesChanged += (sender, e) =>
{
	Console.WriteLine($"Categories changed: {e.Created.Count} created");
};

observer.CategoryItemsChanged += (sender, e) =>
{
	Console.WriteLine($"Items changed: {e.Created.Count} created");
};

// Don't forget to dispose when done
observer.Dispose();
```

### Cache Queries

The cache provides optimized query methods:

#### Scope Queries

```csharp
// Get scope by ID
var scope = cache.GetScope(scopeId);

// Get scope by name (fast O(1) lookup)
var scope = cache.GetScope("Network Infrastructure");

// Check if scope exists
if (cache.TryGetScope("MyScope", out var scope))
{
    // Use scope
}

// Get all scopes
var allScopes = cache.Scopes.Values;
```

#### Category Queries

```csharp
// Get category by ID
var category = cache.GetCategory(categoryId);

// Get all categories in a scope
var categories = cache.GetCategoriesForScope(scopeId);
var categories = cache.GetCategoriesForScope("Network Infrastructure");

// Get root categories only
var rootCategories = cache.GetRootCategoriesForScope(scopeId);

// Get child categories
var children = cache.GetChildCategories(parentCategoryId);

// Get all descendants (recursive)
var descendants = cache.GetDescendantCategories(categoryId);

// Get ancestor path
var path = cache.GetAncestorPath(categoryId);
```

#### Category Item Queries

```csharp
// Get category item by ID
var item = cache.GetCategoryItem(itemId);

// Get items in a category
var items = cache.GetChildItems(categoryId);

// Get all items in category and descendants
var allItems = cache.GetDescendantItems(categoryId);

// Check if category contains an item
var identifier = new CategoryItemIdentifier("My Module", "Instance123");
bool hasItem = cache.ContainsItem(categoryId, identifier);

// Check if category or any descendant contains an item
bool hasItemInTree = cache.ContainsDescendantItem(categoryId, identifier);
```

#### Tree Queries

```csharp
// Get a subtree starting from a category
var subtree = cache.GetSubtree(categoryId);

// Check if category has children
bool hasChildren = cache.HasChildCategories(categoryId);

// Check if category has items
bool hasItems = cache.HasChildItems(categoryId);

// Check if category or descendants have items
bool hasAnyItems = cache.HasDescendantItems(categoryId);
```

### Manual Cache Updates

If not using the observer, you can manually update the cache:

```csharp
// After creating/updating scopes
var updatedScopes = api.Scopes.CreateOrUpdate(newScopes);
cache.UpdateScopes(updatedScopes, []);

// After deleting scopes
var deletedScopes = existingScopes;
cache.UpdateScopes([], deletedScopes);

// Same for categories and items
cache.UpdateCategories(updated, deleted);
cache.UpdateCategoryItems(updated, deleted);
```

### Performance Benefits

Using the cache provides significant performance improvements:

| Operation | Without Cache | With Cache |
|-----------|---------------|------------|
| Get scope by name | Database query | O(1) dictionary lookup |
| Get child categories | Database query | O(1) dictionary lookup |
| Get descendant items | Multiple queries | In-memory traversal |
| Get ancestor path | Multiple queries | In-memory traversal |

### Caching Best Practices

1. **Use the Static Cache** for most scenarios:
   ```csharp
   var cache = connection.GetStaticCategoriesCache();
   ```

2. **Subscribe to updates** if data changes frequently:
   ```csharp
   var observer = new CategoriesObserver(api, cache);
   observer.Subscribe();
   ```

3. **Dispose properly** to avoid memory leaks:
   ```csharp
   using var staticCache = connection.GetStaticCategoriesCache();
   // Use cache
   // Automatically disposed at end of scope
   ```

4. **Load initial data** when creating your own cache:
   ```csharp
   cache.LoadInitialData(api);
   ```

### Thread Safety

The `CategoriesCache` is thread-safe for read operations and updates. Internal locks ensure consistency when multiple threads access the cache.

### Example: Complete Caching Setup

```csharp
using Skyline.DataMiner.Utils.Categories.API;
using Skyline.DataMiner.Utils.Categories.API.Caching;
using Skyline.DataMiner.Utils.Categories.API.Extensions;

public class CategoryService
{
    private readonly CategoriesCache _cache;
    
    public CategoryService(IConnection connection)
    {
        _cache = connection.GetStaticCategoriesCache().Cache;
    }
    
    public IEnumerable<Category> GetCategoriesForScope(string scopeName)
    {
        return _cache.GetCategoriesForScope(scopeName);
    }
    
    public CategoryNode GetTree(string scopeName)
    {
    	if (_cache.TryGetScope(scopeName, out var scope))
		{
			var categories = _cache.GetCategoriesForScope(scope);
			return categories.ToTree();
		}

		return null;
    }
}
```

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
