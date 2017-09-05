##### Define a DAL class in comments like this:

```C#
/*  Class name
 *  Table name (this is the actual table name in the db)
 *  Column name:            List of keywords/datatypes ended with ; A comment
 *  @ForeignKey:            NAME(fk_name); FOREIGNTABLE(other DAL's class name); COLUMNS[Column name, Column name]; FOREIGNCOLUMNS[Column name, Column name]; ONUPDATE(CASCADE/RESTRICT/SETNULL/NOACTION); ONDELETE(CASCADE/RESTRICT/SETNULL/NOACTION);
 *  @Index:                 NAME(ix_name); [Column name, column name]; UNIQUE/PRIMARYKEY/SPATIAL/FULLTEXT/BTREE/RTREE/HASH/NONCLUSTERED/CLUSTERED
 *  @BeforeInsert:          Code to execute in beginning of Insert(...) code
 *  @BeforeUpdate:          Code to execute in beginning of Update(...) code
 *  @AfterRead:             Code to execute at the end of Read(...) code
 *  @MySqlEngine:           InnoDB/MyISAM/ARCHIVE (Use MyISAM for geographical, FULLTEXT, and general RTREE indexes)
 *  @StaticColumns          Export column names as static fields (writable) instead of const
 *  @OmitCollection         Omit collection class
 *  @AtomicUpdates          Track atomic column updates
 *  @InsertAutoIncrement    Allow inserting autoincrement values
 *  @NoCreatedBy			Disable the automatic "CreatedBy" setting on Insert
 *  @NoCreatedOn			Disable the automatic "CreatedOn" setting on Insert
 *  @NoModifiedBy			Disable the automatic "ModifiedBy" setting on Update
 *  @NoModifiedOn			Disable the automatic "ModifiedOn" setting on Update
 */
 ```

##### Keywords that affect the DB schema: (Case Insensitive)

 `ColumnName` _name_ - define the actual column name in the db, if it is different than the default specified name  
 `PropertyName` _name_ - define the name of the property, if it is different than the default specified name  
 `Primary Key` - Defines the column as primary key or part of primary key  
 `AutoIncrement` / `Auto Increment` / `AUTO_INCREMENT` - Defines the column as a serial column  
 `Default` _value_ - Sets the DEFAULT value for the column  
 `Nullable` - Defines the column as NULLABLE. Default is NOT NULL.  
 `IsMutatedProperty` - the name of a property specifying whether the column is mutated. Goes hand-in-hand with `@AtomicUpdates`
 `Charset` - the name of a character set for this column  
 `Collate` - the name of a collation for this column  
 `Unique` / `Unique Index` - Defines a UNIQUE index on this column  
 `Foreign` _TableClassName_._ColumnName_ - Defines a foreign key for this column  

##### Keywords that affect the AbstractRecord<> structure: (Case Insensitive)

 `NoProperty` - will remove the property code for this specific column. Can be used to write a custom property  
 `NoSave` - will remove the property from the save routines  
 `ActualDefault` _value_ - define a different code-based default value for the class's instance member variable  
 `ActualType` _type_ - The actual .Net type name  
 `ToDB` - Define a format to be used in Insert(...) and Update(...). Can use the {0} specifier for column value in format  
 `FromDB` - Define a format to be used in Read(...). Can use the {0} specifier for column value in format  
 `VirtualProp` - Defines the property as virtual  

##### Computed Columns:

 Specify "`Computed` _expression_here_ _[STORED]_"

##### Data types:

 `INT64`; `INT32`; `INT16`; `INT8`; `UINT64`; `UINT32`; `UINT16`; `UINT8`; `DOUBLE`; `FLOAT`; 
 `DECIMAL`; `MONEY`; `PRECISION(length)`; `SCALE(length)`;
 `DECIMAL(precision,scale)`; `MONEY(precision,scale)`;
 `DECIMAL(precision)`; `MONEY(precision)`;
 `FIXEDSTRING(length)`; `STRING(length)`; `TEXT`; `LONGTEXT(length)`; `MEDIUMTEXT(length)`; 
 `BOOL`; `GUID`;
 `DATETIME`; `DATETIME_UTC`; `DATETIME_LOCAL`; 
 `LITERALTYPE _some_literal_type_here_`

**NOTE:** `LITERALTYPE` must come with an `ActualType` 

##### Geometry spatial types:

 `GEOMETRY`; GEOMETRYCOLLECTION`; 
 `POINT`; `LINESTRING`; `POLYGON`; `LINE`; `CURVE`; `SURFACE`; `LINEARRING`; 
 `MULTIPOINT`; `MULTILINESTRING`; `MULTIPOLYGON`; `MULTICURVE`; `MULTISURFACE`;

##### Geographic spatial types:

 `GEOGRAPHIC`; `GEOGRAPHICCOLLECTION`; 
 `GEOGRAPHIC_POINT`; `GEOGRAPHIC_LINESTRING`; `GEOGRAPHIC_POLYGON`; 
 `GEOGRAPHIC_LINE`; `GEOGRAPHIC_CURVE`; `GEOGRAPHIC_SURFACE`; `GEOGRAPHIC_LINEARRING`; 
 `GEOGRAPHIC_MULTIPOINT`; `GEOGRAPHIC_MULTILINESTRING`; `GEOGRAPHIC_MULTIPOLYGON`; 
 `GEOGRAPHIC_MULTICURVE`; `GEOGRAPHIC_MULTISURFACE`;

##### Enumeration type:

 You can define an enum for the column, if specified like this:
```C#
/* Column name:            DEFAULT EnumName.None; Comment for column:
 *                         "EnumName"
 *                         - None = 0
 *                         - Option1 = 1
 *                         - Option2 = 2
 */
```
