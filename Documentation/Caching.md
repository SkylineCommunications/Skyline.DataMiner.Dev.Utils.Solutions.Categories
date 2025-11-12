# Caching

The Categories API provides built-in caching capabilities to improve performance when reading data frequently.

## Overview

The caching system consists of two main components:

- **CategoriesCache**: In-memory cache of all categories data
- **CategoriesObserver**: Automatically updates the cache when data changes

## Using the Static Cache

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

## Creating Your Own Cache

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

## Using the Observer for Auto-Updates

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

## Cache Queries

The cache provides optimized query methods:

### Scope Queries

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

### Category Queries

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

### Category Item Queries

```csharp
// Get category item by ID
var item = cache.GetCategoryItem(itemId);

// Get items in a category
var items = cache.GetChildItems(categoryId);

// Get all items in category and descendants
var allItems = cache.GetDescendantItems(categoryId);

// Check if category contains an item
var identifier = new CategoryItemIdentifier("Protocol", "Element123");
bool hasItem = cache.ContainsItem(categoryId, identifier);

// Check if category or any descendant contains an item
bool hasItemInTree = cache.ContainsDescendantItem(categoryId, identifier);
```

### Tree Queries

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

## Manual Cache Updates

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

## Performance Benefits

Using the cache provides significant performance improvements:

| Operation | Without Cache | With Cache |
|-----------|---------------|------------|
| Get scope by name | Database query | O(1) dictionary lookup |
| Get child categories | Database query | O(1) dictionary lookup |
| Get descendant items | Multiple queries | In-memory traversal |
| Get ancestor path | Multiple queries | In-memory traversal |

## Best Practices

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

## Thread Safety

The `CategoriesCache` is thread-safe for read operations and updates. Internal locks ensure consistency when multiple threads access the cache.

## Example: Complete Caching Setup

```csharp
using Skyline.DataMiner.Utils.Categories.API;
using Skyline.DataMiner.Utils.Categories.API.Caching;
using Skyline.DataMiner.Utils.Categories.API.Extensions;

public class CategoryService
{
    private readonly StaticCategoriesCache _staticCache;
    
    public CategoryService(IConnection connection)
    {
      _staticCache = connection.GetStaticCategoriesCache();
    }
    
    public IEnumerable<Category> GetCategoriesForScope(string scopeName)
    {
        return _staticCache.Cache.GetCategoriesForScope(scopeName);
    }
    
    public CategoryNode GetTree(string scopeName)
    {
        if (_staticCache.Cache.TryGetScope(scopeName, out var scope))
        {
            var categories = _staticCache.Cache.GetCategoriesForScope(scope);
            return categories.ToTree();
        }
        
        return null;
    }
}
```
