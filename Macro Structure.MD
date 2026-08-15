# SequelNet DAL macro generator

A SequelNet DAL macro is a concise schema description stored in a C# comment block or consecutive `//` comment lines. The generator turns it into an `AbstractRecord<T>` implementation, its schema, CRUD helpers, and (unless disabled) a matching collection.

The macro is the source of truth. Generated code is disposable.

## Generate and update safely

1. Keep the complete macro in one C# block comment or a consecutive group of `//` comment lines.
2. In either Visual Studio or VS Code, run the generation command with a selection, with the cursor inside a macro comment, or with exactly one valid macro comment in the file. Both recognize either a `/* ... */` block or consecutive `//` comment lines. A macro must have a class line, a table line, and at least one `Column: ...` line. If several macro comments exist, select one or place the cursor inside it.
3. The tool creates this region immediately after the selected macro:

```csharp
// <sequelnet-generated>
// generated code
// </sequelnet-generated>
```

4. Edit the macro when the schema changes, then run generation again. The tool first replaces the uniquely named region for that record anywhere in the file, then falls back to a legacy generic region directly after the macro. Legacy regions are upgraded to named markers.
5. Put hand-written behavior in a separate partial class or outside the marked region.

## Minimal macro

The first non-empty line is the class name. The second is the table name; use `owner.table_name` to supply a database owner. Each following line declares either a column or a directive.

```csharp
/*
 * Customer
 * crm.customers
 * Id: PRIMARY KEY; AUTO_INCREMENT; INT64; Customer identifier
 * Name: STRING(200); Customer display name
 * IsActive: BOOL; DEFAULT 1; Active flag
 * CreatedOn: DATETIME_UTC; Creation timestamp
 * @Index: NAME(IX_Customers_Name); [Name ASC]; UNIQUE
 */
```

Column keywords are case-insensitive and separated by semicolons. The final segment is treated as the column comment, so put it last.

## Column keywords

| Purpose | Keywords |
| --- | --- |
| Identity and nullability | `PRIMARY KEY`, `NULLABLE`, `AUTOINCREMENT` / `AUTO_INCREMENT` / `AUTO INCREMENT` |
| Names and defaults | `ColumnName value`, `PropertyName value`, `DEFAULT value`, `ActualDefault value` |
| Generated member behavior | `NoProperty`, `NoRead`, `NoSave`, `VirtualProp` (legacy alias `Virtual`), `QueryBoundary` |
| Custom conversion | `ActualType type`, `ToDB format` (use `{0}` for the value), `FromDB format` (use `{0}` for the value) |
| Schema relationships | `UNIQUE` / `Unique Index`, `Foreign OtherRecord.Id`, `Charset name`, `Collate name`, `SRID number` |
| Advanced schema | `Computed expression [STORED]`, `IsMutatedProperty propertyName` |

`QueryBoundary` marks a column that must be included in relevant query boundaries. `ActualType` is required with `LITERALTYPE`.

### Supported data-type keywords

All type keywords are case-insensitive. This is the complete current list.

| Family | Supported keywords |
| --- | --- |
| Integers | `INT`, `INTEGER`, `INT8`, `INT16`, `INT32`, `INT64`, `UINT8`, `UINT16`, `UINT32`, `UINT64` |
| Floating point and fixed-point | `FLOAT`, `DOUBLE`, `DECIMAL`, `MONEY`, `DECIMAL(precision)`, `MONEY(precision)`, `DECIMAL(precision,scale)`, `MONEY(precision,scale)`, `PRECISION(length)`, `SCALE(length)` |
| Text | `STRING(length)` / `STRING(MAX)`, `FIXEDSTRING(length)` / `FIXEDSTRING(MAX)`, `TEXT` / `TEXT(length)`, `MEDIUMTEXT` / `MEDIUMTEXT(length)`, `LONGTEXT` / `LONGTEXT(length)` |
| Binary and JSON | `BINARY(length)` / `BINARY(MAX)`, `VARBINARY(length)` / `VARBINARY(MAX)`, `JSON`, `JSON_BINARY` |
| Basic values | `BOOL`, `GUID` |
| Date and time | `DATETIME`, `DATETIME_UTC`, `DATETIME_LOCAL`, `DATETIMEOFFSET`, `DATE`, `TIME`, `DATEONLY` / `DATE_ONLY`, `TIMEONLY` / `TIME_ONLY` |
| Literal/custom | `LITERALTYPE database-type` together with `ActualType dotnet-type` |
| Geometry | `GEOMETRY`, `GEOMETRYCOLLECTION`, `POINT`, `LINESTRING`, `POLYGON`, `LINE`, `CURVE`, `SURFACE`, `LINEARRING`, `MULTIPOINT`, `MULTILINESTRING`, `MULTIPOLYGON`, `MULTICURVE`, `MULTISURFACE` |
| Geographic | `GEOGRAPHIC`, `GEOGRAPHICCOLLECTION`, `GEOGRAPHIC_POINT`, `GEOGRAPHIC_LINESTRING`, `GEOGRAPHIC_POLYGON`, `GEOGRAPHIC_LINE`, `GEOGRAPHIC_CURVE`, `GEOGRAPHIC_SURFACE`, `GEOGRAPHIC_LINEARRING`, `GEOGRAPHIC_MULTIPOINT`, `GEOGRAPHIC_MULTILINESTRING`, `GEOGRAPHIC_MULTIPOLYGON`, `GEOGRAPHIC_MULTICURVE`, `GEOGRAPHIC_MULTISURFACE` |

Use `DATEONLY` and `TIMEONLY` when generated records should expose .NET 6+ `DateOnly` and `TimeOnly` values. `DATE` and `TIME` retain their existing `DateTime` and `TimeSpan` generated types for backwards compatibility.

The generator continues to accept the historical misspelling `GEOGAPHIC` and its `GEOGAPHIC_*` variants for backward compatibility, but new macros should use the correctly spelled keywords above.
## Directives

| Directive | Effect |
| --- | --- |
| `@Index: NAME(name); [Column ASC, Other DESC]; UNIQUE/PRIMARYKEY/...` | Adds an index. Supported modes include `UNIQUE`, `PRIMARYKEY`, `SPATIAL`, `FULLTEXT`; supported types include `BTREE`, `RTREE`, `HASH`; clustering supports `CLUSTERED` and `NONCLUSTERED`. |
| `@ForeignKey: NAME(name); FOREIGNTABLE(Record); COLUMNS[Local]; FOREIGNCOLUMNS[Remote]; ONUPDATE(CASCADE); ONDELETE(RESTRICT)` | Adds a multi-column foreign key. |
| `@BeforeInsert: code` / `@AfterInsertQuery: code` | Adds custom generated insert behavior. |
| `@BeforeUpdate: code` / `@AfterUpdateQuery: code` | Adds custom generated update behavior. |
| `@AfterRead: code` | Adds code at the end of generated reads. |
| `@MySqlEngine: InnoDB` | Sets `InnoDB`, `MyISAM`, or `ARCHIVE`. |
| `@StaticColumns` | Emits writable static column fields rather than constants. |
| `@OmitCollection` | Does not emit the `...Collection` class. |
| `@AtomicUpdates` | Enables mutation tracking. |
| `@SnakeColumnNames` | Converts non-custom column names to snake case. |
| `@InsertAutoIncrement` | Allows inserting an auto-increment value. |
| `@NoCreatedOn` / `@NoModifiedOn` | Disables the corresponding automatic timestamp behavior. |
| `@NullableEnabled` or `@NullableEnable` | Emits nullable C# members. |
| `@ComponentModel` | Adds component-model annotations. |

A directive can span physical lines by ending a line with `\`.

## Enumerations

Use a trailing colon in the column comment, followed by an enum name and dash-prefixed values:

```csharp
/*
 * Status: DEFAULT CustomerStatus.None; Customer status:
 * "CustomerStatus"
 * - None = 0
 * - Active = 1
 */
```

## Command-line interface

The reusable CLI accepts either raw macro text or a JSON request on standard input and writes JSON to standard output:

```json
{"script":"Customer\ncustomers\nId: PRIMARY KEY; INT64; Identifier"}
```

Successful responses contain `success`, `code`, and `warnings`. Invalid input returns `success: false` and `errors`. The tool does not modify files; editor integrations decide where to apply the generated code.

## Instructions for coding agents

When working with SequelNet DAL code:

- Treat a macro plus its following `<sequelnet-generated>` region as one unit.
- Change the macro, never generated code, for schema or generated-member changes.
- Generate first and inspect warnings before writing the marked region.
- Preserve user-written partial classes and any source between a macro and its generated region; if unrelated code separates them, do not update that region automatically.
- Use the CLI response as structured data. An agent integration should make file mutation a separate explicit action.

For repository-level instructions, see `AGENTS.md`.