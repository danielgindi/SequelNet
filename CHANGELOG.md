# Changelog

## 3.0.164 - 2026-10-04

### Fixed

- Restricted aggregate wildcard expressions to non-distinct `COUNT(*)`.
  Invalid forms such as `AVG(*)` and `COUNT(DISTINCT *)` now throw an
  `InvalidOperationException` before connector-specific SQL rendering.

## 3.0.163 - 2026-10-04

### Changed

- Added `Query.SelectAs(string columnName, string alias)` as the explicit way to
  select a column with an alias.
- Deprecated `Query.Select(string columnName, string alias)`. Its behavior is
  unchanged in 3.0.163, but the two-string overload is planned to represent a
  qualified column in SequelNet 4.0.
- Updated the deprecated `AddSelect(string columnName, string alias)` method to
  direct callers to `SelectAs`.

### Migration

Replace the deprecated alias overload:

```csharp
query.Select("column", "alias");
```

with:

```csharp
query.SelectAs("column", "alias");
```

Both forms generate the same SQL in 3.0.163. Migrating now prevents the call from
changing meaning when `Select(string, string)` becomes the qualified-column
overload in SequelNet 4.0.
