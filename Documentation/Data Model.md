# Data Model

This document describes the data model for the Category and Category Item entities.

- **Scope**: defines the context in which categories exist.
- **Categories**: provide a hierarchical structure for grouping items within a specific scope. Categories can be nested to form a tree structure, with each category having a reference to its parent.
- **Category Items**: links specific items to categories. It serves only as a reference to a module/instance and does not store additional data.
- **Instance**: external module/instance, maintained outside the system.

```mermaid
erDiagram
	"Scope" {
		string Name
	}
	"Category" {
		string Name
		Category ParentCategory
		Category RootCategory
		Scope Scope
	}
	"Category Item" {
		Category Category
		string ModuleID
		string InstanceID
	}

	"Instance" {
	}

	"Scope" ||--o{ "Category" : ""
	"Category" ||--o{ "Category" : ""
	"Category" ||--o{ "Category Item" : ""
	"Category Item" |o..o| "Instance" : "external module, maintained elsewhere"
```