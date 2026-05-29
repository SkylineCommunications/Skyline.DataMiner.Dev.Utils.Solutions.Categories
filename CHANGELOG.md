# Changelog

## v1.2.7

This release improves how categories can be found and filtered, especially in environments where multiple categories share the same name across different scopes or parent categories.

Users can now retrieve categories more reliably by combining name, scope, and parent-category context. Filtering with category exposers has also been improved, making it easier to find:

- categories in a specific scope,
- child categories under a specific parent,
- and top-level categories created directly at the root of a scope.

### What's improved

- Category searches now better support scenarios where the same category name exists more than once.
- Querying with exposers is more flexible and now supports category and scope comparisons cleanly.
- Root-level categories can now be identified correctly using the new `CategoryExposers.IsRootCategory` exposer, making it easier to distinguish between top-level and nested categories.

### Breaking change

`CategoryRepository.Read(string name)` and `CategoryRepository.Read(IEnumerable<string> names)` now throw `NotSupportedException`.

These methods were ambiguous when duplicate category names existed across scopes, which could lead to incorrect or unreliable behavior. To avoid this, category reads must now be done with scope-aware overloads:

- `Read(Scope scope, string name)`
- `Read(Scope scope, IEnumerable<string> names)`

This is a breaking change for consumers that previously relied on name-only reads.

### Why this change was made

The original implementation did not correctly handle categories with identical names in different scopes. In addition, several filtering scenarios did not behave as expected:

- retrieving a category by name, scope, and parent category,
- reading categories created at the root of a scope,
- and querying top-level categories where no parent field value exists.

This update resolves those limitations and makes category retrieval behavior more explicit and predictable.

### Compatibility note

If your code currently reads categories by name only, you will need to update it to use the new scope-based overloads.
