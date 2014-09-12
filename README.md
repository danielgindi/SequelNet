dg.Sql
======

SQL abstraction/generalization/protection layer. Anyone is welcome to use and improve :-)

A little bit of history
-----------------------
In the far past, I developed a framework for use in many of my projects, and then not so long ago I have decided to separate the Sql stuff to a library.
Now this is an awesome library which allows you to *very easily* prototype db schemas.
No so long ago I have decided to also make this public on GitHub.

The fact that this library allows great flexibility *and* is open source, means we have full control over our queries - while having the ease of OOP and the protection of a query builder.
If we required, we can also build our schemas entirely in runtime, not depending on any prebuilt schema files.
And if a feature is missing, well, it's open source...

The use is pretty straight-forward.
To supply a default connector, add a dg.Sql.Connector key in the appSettings of web.config. Like this:
    <add key="dg.Sql.Connector" value="MySql" />
The default is MySql.
Available connectors right now are MySql, MsSql, OleDb. Postgre is not tested yet - I welcome anyone who wants to do this.

To supply a default connection string, add a dg.Sql connection string in web.config, or dg.Sql::ConnectionString key in appSettings. Another option is to add a dg.Sql::ConnectionStringKey key in the appSettings, which specifies the connectionString's name to be used.

Structure of this library
-------------------------

* `Query`: Build the query, and executes in a number of different ways, and different way to retrieve the results
* `ConnectorBase` is the base class for the connector layer. Each connector has a subclass of this class, which supplies the basic functionalities, similar to the `SqlConnection` class. (*I am considering a change this class's name in the future...*)
* `DataReaderBase` is the same story as with `ConnectorBase`, providing the same functionality as `SqlDataReader`
* `FactoryBase` is a special base class which is responsible for creating special objects like `DbCommand`s and `DbDataAdapter`s. Each connector has a subclass of this class.
* `AbstractRecord` is a DAL class that represents your data, and contains a `TableSchema`, data accessors, and helper functions. Using `AbstractRecord` is completely optional!
* `TableSchema` is a class representing the an actual db schema. This assists the `Query` in converting values where necessary, bulding queries or even building `CREATE TABLE`, `ALTER TABLE` and `CREATE INDEX` queries...
* There's a namespace `Phrases` which includes many objects that wrap native sql functions. You can add your own, based on the `BasePhrase` class.

Bonus
-----

If you're using MySql, then there's a `MySqlBackup` class, which can create a full backup of a Database, including shcema and data. Yes, I know, it's cool.

SchemaGeneratorAddIn
--------------------

There's a Visual Studio AddIn, called the "SchemaGeneratorAddIn", which assists in creating the `TableSchema` and a matching `AbstractRecord` based on a simple language which resides in a comment.

This little tool makes rapid db prototyping easy, and gets you much further in development in significantly less time than when using any other tool known to humanity.

I will soon post here some examples on how to use it. 
(Although there is a full documentation of the syntax in the Macro Structure.txt file)

