# Core Concepts

## Data Model Overview

The Categories API is built around three main entities that work together to create a hierarchical categorization system.

### Scope

A **Scope** defines the context in which categories exist. It's the top-level organizational unit.

```csharp
var scope = new Scope { Name = "Network Infrastructure" };
```

**Properties:**
- `Name` (string) - Unique name of the scope

### Category

A **Category** provides hierarchical structure for grouping items. Categories can be nested to form a tree structure.

```csharp
var rootCategory = new Category 
{ 
    Name = "Devices",
    Scope = scope 
};

var childCategory = new Category 
{ 
    Name = "Routers",
    Scope = scope,
ParentCategory = rootCategory,
    RootCategory = rootCategory
};
```

**Properties:**
- `Name` (string) - Name of the category
- `Scope` (ApiObjectReference<Scope>) - The scope this category belongs to
- `ParentCategory` (ApiObjectReference<Category>?) - Reference to parent category (null for root categories)
- `RootCategory` (ApiObjectReference<Category>) - Reference to the top-level category in the hierarchy
- `IsRootCategory` (bool) - Whether this is a root category (has no parent)

**Key Methods:**
- `GetChildCategories()` - Get direct children
- `GetDescendantCategories()` - Get all descendants
- `GetAncestorPath()` - Get path from root to this category
- `GetChildItems()` - Get items directly under this category

### Category Item

A **Category Item** links external items (instances) to categories. It serves only as a reference.

```csharp
var item = new CategoryItem
{
    Category = category,
    ModuleId = "MyModule",
    InstanceId = "Instance123"
};
```

**Properties:**
- `Category` (ApiObjectReference<Category>) - The category this item belongs to
- `ModuleId` (string) - External module identifier
- `InstanceId` (string) - External instance identifier

### Category Item Identifier

A lightweight struct for identifying items without creating full CategoryItem objects:

```csharp
var identifier = new CategoryItemIdentifier("MyModule", "Instance123");
```

## Hierarchical Structure

Categories form a tree structure:

```
Scope: "Network Infrastructure"
??? Category: "Devices" (root)
?   ??? Category: "Routers"
?   ?   ??? Item: "Router-01"
?   ?   ??? Item: "Router-02"
?   ??? Category: "Switches"
?  ??? Item: "Switch-01"
??? Category: "Services" (root)
    ??? Category: "Monitoring"
```

## References vs Objects

The API uses `ApiObjectReference<T>` for relationships:

```csharp
// Get a reference to an object
ApiObjectReference<Category> categoryRef = category.Reference;

// Or implicitly
ApiObjectReference<Category> categoryRef = category;

// Check if a reference is empty
if (categoryRef == ApiObjectReference<Category>.Empty)
{
    // Handle empty reference
}
```

## Working with Trees

The `CategoryNode` class represents a node in the category tree:

```csharp
CategoryNode tree = api.Categories.GetTree(scope);

// Navigate the tree
foreach (var childNode in tree.ChildCategories)
{
    Console.WriteLine($"Category: {childNode.Category.Name}");
    Console.WriteLine($"  Items: {childNode.ChildItems.Count}");
}

// Get all descendants
var allDescendants = tree.GetDescendantCategories();
var allItems = tree.GetDescendantItems();
```
