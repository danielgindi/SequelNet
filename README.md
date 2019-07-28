SequelNet
=========

SQL abstraction/generalization/protection layer.

Current connectors available:
* MySql
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
    <add key="SequelNet.Connector" value="MySql" />
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

Bonus
-----

If you're using MySql, then there's a `MySqlBackup` class, which can create a full backup of a Database, including shcema and data. Yes, I know, it's cool.

SchemaGenerator
--------------------

There's a Visual Studio AddIn, called the "SchemaGenerator", which assists in creating the `TableSchema` and a matching `AbstractRecord` based on a simple language which resides in a comment.

This little tool makes rapid db prototyping easy, and gets you much further in development in significantly less time than when using any other tool I've used.

Full documentation of the syntax in [Macro Structure.MD](https://github.com/danielgindi/SequelNet/blob/master/Macro%20Structure.MD).

