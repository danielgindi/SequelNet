dg.Sql
======

My SQL abstraction/generalization/protection layer. (Well, anyone is welcome to use and improve :-)

This was separated from a framework that I wrote sometime deep in the past. Since I now use it in all kinds of projects, and give it to my friends and employees to work with, I decided to make this public. Enjoy :-)

The use is pretty straight-forward.
To supply a default connector, add a dg.Sql.Connector key in the appSettings of web.config. Like this:
    <add key="dg.Sql.Connector" value="MySql" />
The default is MySql.
Available connectors right now are MySql, MsSql, OleDb.

To supply a default connection string, add a dg.Sql connection string in web.config, or dg.Sql::ConnectionString key in appSettings. Another option is to add a dg.Sql::ConnectionStringKey key in the appSettings, which specifies the connectionString's name to be used.

Use the Query object for anything that's query related.
There are some helper functions in the AbstractRecord and AbstractRecordList classes.
Some more helper classes are in the Phrases classes.
Anything geospatial is in the Spatial classes.
