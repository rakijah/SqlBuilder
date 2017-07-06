# SqlBuilder  
This library provides a quick and easy way to build an SQL command string through command-chaining. It's still in early development and currently only supports `SELECT` commands. Sorting also needs to be redone, as there is currently no way to really specify sorting priority for multiple sorting columns.  

# Usage  
`SqlBuild.Build()` to generate a new `BuildSelectCommand`. From there you can chain commands until you're done with your entire SQL command.

`AddColumn`/`AddColumns`: Add one or more columns to the selection (SELECT <columns> FROM...)  
`AddTable`: Add a table to select from (FROM <table>)  
`AddJoin`: Add a JOIN to the SQL command  
`CreateCondition`: Begins creation of the WHERE clause  
`CreateSort`: Begins creation of the ORDER BY clause  
`ToString`: Generates the actual SQL command string  

After calling `CreateCondition` you are dealing with a `BuiltSqlCondition` object, which exposes the following methods:  
`AddCondition`: Adds a comparison to the WHERE clause. The operator to perform the comparison can be passed as a string. It's also possible to compare to a static value and enclose this value in a given character (for example `'value'`).  
`BeginBlock`/`EndBlock`: Begins or ends a block by adding a `(` or `)`, respectively. BuiltSqlCondition also keeps track of wether or not you're currently within a block and throws an exception if you try to create an SQL command from a condition with unmatched block parenthesis.  
`And`/`Or`: Adds an `OR` or `AND` logical expression to the condition. This throws an exception if you try to add logical expressions in invalid places (for example `AND AND` can never be valid)  
`Finish`: Ends the condition creation and returns the parent BuiltSelectCommand to allow for contiuous chaining.  

After calling `CreateSort` you are dealing with a `BuiltSqlSort` object, which exposes the following methods:  
`SortBy`: Adds a column to be sorted and allows you to select which sort mode you want to use (ascending/descending). Currently doesn't order columns correctly.  
`Finish`: Ends the sort creation and returns the parent BuiltSelectCommand to allow for contiuous chaining.  