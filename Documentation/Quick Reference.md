# Quick Reference

## API Initialization

```csharp
// Basic initialization
var api = new CategoriesApi(connection);

// Using extension method
var api = connection.GetCategoriesApi();

// Check installation
if (!api.IsInstalled())
{
    api.InstallDomModules();
}
```

## Creating Objects

```csharp
// Scope
var scope = new Scope { Name = "My Scope" };

// Category (root)
var category = new Category 
{ 
    Name = "My Category", 
 Scope = scope 
};

// Category (child)
var child = new Category 
{ 
    Name = "Child Category",
    Scope = scope,
    ParentCategory = category,
    RootCategory = category
};

// Category Item
var item = new CategoryItem
{
    Category = category,
    ModuleId = "MyModule",
    InstanceId = "Instance123"
};

// Save
api.Scopes.CreateOrUpdate([scope]);
api.Categories.CreateOrUpdate([category, child]);
api.CategoryItems.CreateOrUpdate([item]);
```

## Reading Objects

```csharp
// By ID
var scope = api.Scopes.Read(id);
var category = api.Categories.Read(id);

// By name
var scope = api.Scopes.Read("My Scope");
var category = api.Categories.Read("My Category");

// Multiple by IDs
var scopes = api.Scopes.Read(new[] { id1, id2, id3 });

// All
var allScopes = api.Scopes.ReadAll();

// With filter
var filter = ScopeExposers.Name.Contains("Network");
var scopes = api.Scopes.Read(filter);

// LINQ query
var results = api.Categories.Query()
    .Where(c => c.Name.StartsWith("A"))
 .OrderBy(c => c.Name)
    .Take(10)
 .ToList();
```

## Updating & Deleting

```csharp
// Update
scope.Name = "New Name";
api.Scopes.CreateOrUpdate([scope]);

// Delete
api.Categories.Delete(category);
api.Categories.Delete([cat1, cat2, cat3]);
```

## Category Hierarchies

```csharp
// Get child categories
var children = api.Categories.GetChildCategories(parent);
var children = parent.GetChildCategories(api.Categories);

// Get all descendants
var descendants = api.Categories.GetDescendantCategories(parent);

// Get path from root
var path = api.Categories.GetAncestorPath(category);

// Get tree
var tree = api.Categories.GetTree(scope);

// Navigate tree
foreach (var node in tree.ChildCategories)
{
    Console.WriteLine(node.Category.Name);
}
```

## Category Items

```csharp
// Add items
category.AddChildItems(api.CategoryItems, new[]
{
    new CategoryItemIdentifier("Module", "Instance1"),
    new CategoryItemIdentifier("Module", "Instance2")
});

// Remove items
category.RemoveChildItems(api.CategoryItems, items);

// Replace all items
category.ReplaceChildItems(api.CategoryItems, newItems);

// Clear all items
category.ClearChildItems(api.CategoryItems);

// Get items
var items = category.GetChildItems(api.CategoryItems);
var items = api.CategoryItems.GetChildItems(category);
```

## Caching

```csharp
// Get static cache
var cache = connection.GetStaticCategoriesCache().Cache;

// Queries (fast!)
var scope = cache.GetScope("My Scope");
var category = cache.GetCategory(categoryId);
var categories = cache.GetCategoriesForScope(scope);
var children = cache.GetChildCategories(categoryId);
var items = cache.GetChildItems(categoryId);
var tree = cache.GetSubtree(categoryId);

// Checks
bool hasChildren = cache.HasChildCategories(categoryId);
bool hasItems = cache.HasChildItems(categoryId);
bool containsItem = cache.ContainsItem(categoryId, itemIdentifier);
```

## Subscriptions

```csharp
// Subscribe to all changes
using var subscription = api.Categories.Subscribe();
subscription.Changed += (sender, e) => 
{
    // Handle changes
};

// Subscribe with filter
var filter = CategoryExposers.Scope.Equal(scopeId);
using var subscription = api.Categories.Subscribe(filter);
```

## Validation

```csharp
// Validate manually
var result = category.Validate();
if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine(error.Message);
    }
}

// Automatic validation on save
try 
{
    api.Categories.CreateOrUpdate([category]);
}
catch (InvalidOperationException ex)
{
    // Validation failed
}
```

## References

```csharp
// Get reference from object
ApiObjectReference<Category> ref = category.Reference;
ApiObjectReference<Category> ref = category; // implicit

// Create from ID
ApiObjectReference<Category> ref = categoryId;
ApiObjectReference<Category> ref = new ApiObjectReference<Category>(id);

// Check if empty
if (ref == ApiObjectReference<Category>.Empty) { }

// Get ID
Guid id = ref.ID;
```

## Exposers

```csharp
using Skyline.DataMiner.Utils.Categories.API.Objects;

// Available exposers
ScopeExposers.ID
ScopeExposers.Name

CategoryExposers.ID
CategoryExposers.Name
CategoryExposers.Scope
CategoryExposers.ParentCategory
CategoryExposers.RootCategory

CategoryItemExposers.ID
CategoryItemExposers.Category
CategoryItemExposers.ModuleId
CategoryItemExposers.InstanceId

// Usage in filters
var filter = CategoryExposers.Name.Contains("Router")
    .AND(CategoryExposers.Scope.Equal(scopeId));

var categories = api.Categories.Read(filter);

// Usage in ordering
var query = new TRUEFilterElement<Category>().ToQuery()
    .OrderBy(CategoryExposers.Name.Ascending());
```

## Common Patterns

### Check if exists before reading
```csharp
if (cache.TryGetCategory(id, out var category))
{
    // Use category
}
```

### Batch operations
```csharp
var items = new[] { item1, item2, item3 };
api.CategoryItems.CreateOrUpdate(items);
```

### Pagination
```csharp
foreach (var page in api.Categories.ReadAllPaged(pageSize: 100))
{
    ProcessPage(page);
}
```

### Safe parent access
```csharp
if (category.ParentCategory != ApiObjectReference<Category>.Empty)
{
    var parent = api.Categories.Read(category.ParentCategory.Value.ID);
}
```
