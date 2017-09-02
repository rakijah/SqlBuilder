# SqlBuilder  
This library provides a quick and easy way to build an SQL command string through command-chaining. It's still in early development and currently supports `SELECT`, `INSERT`,`DELETE` and basic `ALTER TABLE` and `CREATE TABLE` commands. Sorting also needs to be redone, as there is currently no way to really specify sorting priority for multiple sorting columns. Formatting dates for specific providers is still a work in progress. 

!! *Don't use this in production environments. It uses parameters to avoid SQL injection attacks, but the project is poorly tested so safety cannot be guaranteed* !!

It's basically an insecure micro-clone of Dapper / EntityFramework.

# Usage  
First, create classes that will represent the tables using the custom attributes `SqlTable` and `SqlColumn`, like so:
```csharp
[SqlTable("users")]
public class Users
{
    [SqlColumn("id", DbType.Int32)]
    public string Id { get; set; }

    [SqlColumn("username", DbType.String)]
    public string Username { get; set; }

    [SqlColumn("password", DbType.String)]
    public string Password { get; set; }

    [SqlColumn("email", DbType.String)]
    public string Email { get; set; }
        
    [SqlColumn("lastname", DbType.String)]
    public string LastName { get; set; }

    [SqlColumn("firstname", DbType.String)]
    public string FirstName { get; set; }
}
```
These classes basically represent your database tables and can later be used to easily query entities via datasets without writing any SQL statements.

Before using any classes from this library, call `SqlBuild.Configure()` once to initialize the builder. Here you can specify your database provider, as well as whether or not table/column names should be wrapped in square brackets (i.e. *[table].[column]*).
The `SqlBuild` class then offers static methods to start building commands:
* `.Select()`
* `.InsertInto()`
* `.Delete()`
* `.AlterTable()`
* `.CreateTable()`

From there you can chain methods until you're done with your entire SQL command.

See [the test project](SqlBuilderTest/Program.cs) for example usage.

# Other classes
### ProviderSpecific
Provides provider-specific SQL strings.

### EntityFetcher
Provides methods to fetch table class instances ("entities") directly from a database. Simply provide an opened database-connection (`DbConnection`) to the constructor and use these methods to query your table classes:
* `All`: Get all entities available on the database.
* `Single`: Get one entity from the database.
* `Fetch`: Get the specified amount of entities (or less, if not available) from the database.

This currently doesn't support foreign keys / sub entities, but this is planned to be implemented in the future.
# Command classes

### Select command
The `BuiltSelectCommand` exposes the following methods:

* `AddColumns`: Add one or more columns to the selection (SELECT columns FROM...)
* `AddColumnDirect`: !!UNSAFE!! Add a column by directly specifying a string. This can be used to transform columns before selecting them (like "TO_CHAR(BIRTHDATE, 'yyyy-MM-dd hh:mm:ss'" etc.)
* `AddTable`: Add a table to select from (FROM table)
* `Join`: Add a JOIN to the SQL command
* `Limit`: Add a LIMIT clause
* `Where`: Allows creation of a WHERE clause
* `OrderBy`: Begins creation of the ORDER BY clause
* `ToString`/`Generate`: Generates the actual SQL command string

When calling `Where` you need to pass in a `BuiltSqlCondition` object, which exposes the following methods:  
* `AddCondition`: Adds a comparison to the WHERE clause. The operator to perform the comparison can be passed as a string.
* `AddConditionDirect`: !!UNSAFE!! Add a condition by directly specifying a string.
* `BeginBlock`/`EndBlock`: Begins or ends a block by adding a `(` or `)`, respectively. BuiltSqlCondition also keeps track of wether or not you're currently within a block and throws an exception if you try to create an SQL command from a condition with unmatched block parenthesis.  
* `And`/`Or`: Adds an `OR` or `AND` logical expression to the condition. This throws an exception if you try to add logical expressions in invalid places (for example `AND AND` can never be valid)  
* `Finish`: Ends the condition creation and returns the parent BuiltSelectCommand to allow for contiuous chaining.  

After calling `OrderBy` you are dealing with a `BuiltSqlSort` object, which exposes the following methods:  
* `SortBy`: Adds a column to be sorted and allows you to select which sort mode you want to use (ascending/descending). Currently doesn't order columns correctly.  
* `Finish`: Ends the sort creation and returns the parent BuiltSelectCommand to allow for contiuous chaining.  

### Insert command
The `BuiltInsertCommand` exposes the following methods:
* `AddValues`: Add a new group of values to be inserted.
* `AddRow`: Allows you to add an entire row by specifying the values in the same order as you added the columns.
* `ToString`/`Generate`: Generate the actual SQL command string.

After calling `AddValues()` you are dealing with a `BuiltInsertValue` object. Use the following methods to specify the values to be inserted:
* `AddValueFor`: Allows you to specify the value for a specific column, as well as giving you the option of enclosing the value in a char (for example `"value"` for strings).
* `Finish`: Ends the creation and returns the parent `BuiltInsertCommand` to allow for continous chaining.

`AddValues()` and `AddRow()` can be called multiple times to insert multiple rows in a single SQL command.

### Delete command
The `BuiltDeleteCommand` exposes the following methods:
* `From`: Specify the table from which to delete.
* `Where`: Same as the `BuiltSelectCommand`s `Where()` method.
* `ToString`/`Generate`: Generate the actual SQL command string.

### Alter table command
The `BuiltAlterTableCommand` exposes the following methods:
* `RenameTable`: Rename the table.
* `Add`: Add a column to the table.
* `Drop`: Drop (delete) a column from the table.
* `AddPrimaryKey`: Add a primary key to the table.
* `ChangeColumnType`: Changes the type of a column.
* `ToString`/`Generate`: Generate the actual SQL command string.

This class has not been updated to use table types yet.

### Create table command
The `BuiltCreateCommand` exposes the following methods:
* `SetPrimaryKey`: Specify a column to be made primary key.
* `ToString`/`Generate`: Generate the actual SQL command string.

# Attribute classes
### SqlTable
This attribute allows you to specify a table name on a table class, like so:
```csharp
[SqlTable("dbo.customers")]
public class Customers
{
...
```
### SqlColumn
This attribute allows you to specify the name and type of a column on a property of a table class, like so:
```csharp
[SqlTable("dbo.customers")]
public class Customers
{
    [SqlColumn("customerid", DbType.Int32)]
    public int ID { get; set; }
    
    [SqlColumn("name", DbType.String)]
    public string Lastname { get; set; }
}
```
Currently, only `Integer` and `String`  are supported. Date has conversion problems due to different providers sometimes using completely different approaches to storing dates (for example SQLite doesn't store dates at all and simply uses TEXT/VARCHAR columns instead).

### SqlForeignKey
Allows you to query entities from foreign key constraints, like so:
```csharp
[SqlTable("bankaccount")]
public class BankAccount
{
    [SqlColumn("accountid", SqlColumnType.String)]
    public string Id { get; set; }

    [SqlColumn("customerid", DbType.String)]
    [SqlForeignKey("id", typeof(Customer))]
    public Customer Customer { get; set; }
}

[SqlTable("customer")]
public class Customer
{
    [SqlColumn("id", DbType.Int32)]
    public string Id { get; set; }

    [SqlColumn("lastname", DbType.String)]
    public string Name { get; set; }
}

[...]
var account = _fetcher.Single<BankAccount>(...);
Console.WriteLine($"Account {account.Id} belongs to {account.Customer.Name}.");
```
