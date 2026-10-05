SequelNet
=========

SQL abstraction/generalization/protection layer.

Current connectors available:
* MySql
* MySql2
* MsSql
* Postgre (not tested too much)
* OleDb (requires .NET Framework).

This library allows you fluent SQL building, even for very complex queries.  
It has a built-in capability of reading/writing records from classes - in a very different way than in Entity Framework.  
It it less automated, which means you have to have your record class inherit from `AbstractRecord` and supply the definition of the schema.  
But - that very definition of the schema allows to manage the whole database lifecycle from the code, including creating from scratch, migrations etc.  
It also allows matching value types according to the schema when applicable.

The magic happens in the little `SchemaGenerator` addon for Visual Studio.  
This little guys takes a "pseudo-script" and converts it to a full code of an `AbstractRecord` record.  
You can override/extend almost anything in there, either in the script itself, or by defining a `partial class` and adding stuff.  

A little bit of history
-----------------------

This used to be `dg.Sql` library, renamed to `SequelNet`.  
Migration notes - in the *Releases* section, for `2.0.0`.  

Usage
-----

The use is pretty straight-forward.
To supply a default connector, add a SequelNet.Connector key in the appSettings of web.config. Like this:
```xml
    <add key="SequelNet.Connector" value="MySql2" />
```

To supply a default connection string, add a `SequelNet` connection string in `web.config`,
or `SequelNet::ConnectionString` key in app config.  
Another option is to add a `SequelNet::ConnectionStringKey` key in the app config, which specifies the connectionString's name to be used.

Structure of this library
-------------------------

* `Query`: Build the query, and executes in a number of different ways, and different way to retrieve the results
* `ConnectorBase` is the base class for the connector layer. Each connector has a subclass of this class, which supplies the basic functionalities, similar to the `SqlConnection` class. (*I am considering a change this class's name in the future...*)
* `DataReaderBase` is the same story as with `ConnectorBase`, providing the same functionality as `SqlDataReader`
* `AbstractRecord` is a DAL class that represents your data, and contains a `TableSchema`, data accessors, and helper functions. Using `AbstractRecord` is completely optional!
* `TableSchema` is a class representing the an actual db schema. This assists the `Query` in converting values where necessary, bulding queries or even building `CREATE TABLE`, `ALTER TABLE` and `CREATE INDEX` queries...
* There's a namespace `Phrases` which includes many objects that wrap native sql functions. You can add your own, based on the `IPhrase` class.

Detecting database errors
-------------------------

Use `ConnectorBase.GetDatabaseError(exception)` to detect database errors without
referencing a driver exception type or checking numeric error codes:

```csharp
using SequelNet.Connector;

try
{
    query.Execute(connector);
}
catch (Exception ex) when (connector.GetDatabaseError(ex) == DatabaseError.UniqueViolation)
{
    // Handle a primary-key or unique-key collision.
}
```

`UniqueViolation` means duplicate values in a primary key or unique key.
The `Duplicate*` categories mean duplicate names or definitions; `Undefined*`
categories mean missing objects, including missing drop targets.

| Category | MySQL (both drivers) | SQL Server | PostgreSQL SQLSTATE | Jet/ACE SQLState |
| --- | --- | --- | --- | --- |
| `UniqueViolation` | 1062 | 2601, 2627 | 23505 | 3022 |
| `ForeignKeyViolation` | 1452 | See `ConstraintViolation` | 23503 | 3200, 3201 |
| `ConstraintViolation` | — | 547 | — | — |
| `DuplicateColumn` | 1060 | 2705 | 42701 | 3191, 3380 |
| `DuplicateTable` | 1050 | — | 42P07 | 3010 |
| `DuplicateIndex` | 1061 | — | — | 3284, 3375 |
| `DuplicateObject` | — | 1913, 2714 | 42710 | 3012, 3283 |
| `UndefinedColumn` | 1054 | 207, 4924 | 42703 | 3381 |
| `UndefinedTable` | — | — | 42P01 | 3376 |
| `UndefinedObject` | 1091 | 3728 | 42704 | 3372 |

Some provider conditions have broader meanings:

- PostgreSQL 42P07 maps to `DuplicateTable`, matching its native condition name,
  but also covers duplicate indexes and other relations. 42710 maps separately to
  `DuplicateObject`. See PostgreSQL's [error codes](https://www.postgresql.org/docs/current/errcodes-appendix.html)
  and [index creation source](https://github.com/postgres/postgres/blob/REL_17_STABLE/src/backend/catalog/index.c).
- SQL Server 547 covers foreign-key and check constraints, so it returns
  `ConstraintViolation`. 1913 covers both indexes and statistics, and 2714 covers
  multiple object kinds, so both return `DuplicateObject`. See Microsoft's
  [error reference](https://learn.microsoft.com/en-us/sql/relational-databases/errors-events/database-engine-events-and-errors).
- OleDb examines Jet/ACE engine error records and reads Access error numbers from
  `OleDbError.SQLState`. `NativeError` may contain a different internal number.
  Error records from other OLE DB providers remain `Unknown`. Jet's generic
  missing-parameter errors also remain `Unknown`, since they do not uniquely
  identify a missing column. See the [Access error catalogue](https://docs.oracle.com/cd/E39885_01/doc.40/e18459/errors_access.htm).

SQL Server and OleDb return the first recognized category from their error collection.
The Jet 4.0 mappings were exercised against a real temporary database; ACE was not
installed on the validation machine.

Classification does not open a connection or change the exception. Null, unrecognized
errors, exceptions from other drivers, and connectors without a classification
implementation return `DatabaseError.Unknown`. Wrapped exceptions are not unwrapped;
pass the underlying provider exception explicitly if needed.

Bonus
-----

If you're using MySql, then there's a `MySqlBackup` class, which can create a full backup of a Database, including shcema and data. Yes, I know, it's cool.

SchemaGenerator
--------------------

There's a Visual Studio AddIn, called the "SchemaGenerator", which assists in creating the `TableSchema` and a matching `AbstractRecord` based on a simple language which resides in a comment.

This little tool makes rapid db prototyping easy, and gets you much further in development in significantly less time than when using any other tool I've used.

Full documentation of the syntax in [Macro Structure.MD](https://github.com/danielgindi/SequelNet/blob/master/Macro%20Structure.MD).


## License

All the code here is under MIT license. Which means you could do virtually anything with the code.
I will appreciate it very much if you keep an attribution where appropriate.

    The MIT License (MIT)
    
    Copyright (c) 2013 Daniel Cohen Gindi (danielgindi@gmail.com)
    
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
    
    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.
    
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
