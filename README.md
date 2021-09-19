# SharpQLite
SharpQLite is a C# library designed to handle interactions with a SQLite database.
The library defines attributes which can be placed on a class that corresponds to a database table. 
The library then uses reflection to implement the SQL logic for each user-defined class.

Usage:
Create a model class and add the SqlTable attribute to define a corresponding SQL Table.
Define a primary key property and add the PrimaryKey attribute, then add SqlColumn and SqlForeignKey attributes to the other properties on the model class as appropriate. 

Instantiate a Dao generic object to handle the SQL logic - including creating the table as well as inserting, adding, updating, and deleting records.
For customization, the Dao class can be inhertted and methods can be overridden. 
