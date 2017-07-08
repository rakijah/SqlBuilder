# SqlBuilder  
This library provides a quick and easy way to build an SQL command string through command-chaining. It's still in early development and currently only supports the `SELECT`, `INSERT` and `DELETE` commands. Sorting also needs to be redone, as there is currently no way to really specify sorting priority for multiple sorting columns.  

# Usage  
The `SqlBuild` class offers static methods to start building commands: `.Select()`, `.Delete()`, `.Insert()`. From there you can chain commands until you're done with your entire SQL command.

## Select command
The `BuiltSelectCommand` exposes the following methods:

* `AddColumns`: Add one or more columns to the selection (SELECT columns FROM...)
* `AddTable`: Add a table to select from (FROM table)
* `Join`: Add a JOIN to the SQL command
* `Where`: Begins creation of the WHERE clause
* `OrderBy`: Begins creation of the ORDER BY clause
* `ToString`/`Generate`: Generates the actual SQL command string

After calling `Where` you are dealing with a `BuiltSqlCondition` object, which exposes the following methods:  
`AddCondition`: Adds a comparison to the WHERE clause. The operator to perform the comparison can be passed as a string. It's also possible to compare to a static value and enclose this value in a given character (for example `'value'`).  
`BeginBlock`/`EndBlock`: Begins or ends a block by adding a `(` or `)`, respectively. BuiltSqlCondition also keeps track of wether or not you're currently within a block and throws an exception if you try to create an SQL command from a condition with unmatched block parenthesis.  
`And`/`Or`: Adds an `OR` or `AND` logical expression to the condition. This throws an exception if you try to add logical expressions in invalid places (for example `AND AND` can never be valid)  
`Finish`: Ends the condition creation and returns the parent BuiltSelectCommand to allow for contiuous chaining.  

After calling `OrderBy` you are dealing with a `BuiltSqlSort` object, which exposes the following methods:  
`SortBy`: Adds a column to be sorted and allows you to select which sort mode you want to use (ascending/descending). Currently doesn't order columns correctly.  
`Finish`: Ends the sort creation and returns the parent BuiltSelectCommand to allow for contiuous chaining.  

## Insert command
The `BuiltInsertCommand` exposes the following methods:
* `Into`: Specify the table and column names to be used in this insert command.
* `AddValues`: Add a new group of values to be inserted. Has to be called after `Into()`.
* `ToString`/`Generate`: Generate the actual SQL command string.

After calling `AddValues()` you are dealing with a `BuiltInsertValue` object. Use the following methods to specify the values to be inserted:
* `AddValueFor`: Allows you to specify the value for a specific column, as well as giving you the option of enclosing the value in a char (for example `"value"` for strings).
* `Finish`: Ends the creation and returns the parent `BuiltInsertCommand` to allow for continous chaining.

`AddValues()` can be called multiple times to insert multiple rows in a single SQL command.
## Delete command
The `BuiltDeleteCommand` exposes the following methods:
* `From`: Specify the table from which to delete.
* `Where`: Same as the `BuiltSelectCommand`s `Where()` method.
* `ToString`/`Generate`: Generate the actual SQL command string.