using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpQLite
{
    public static class SQLite
    {
        private const BindingFlags PROPERTY_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Returns the SQLite version as a string.
        /// Ex.: "3.6.22"
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static string GetSqlVersion(SQLiteConnection conn)
        {
            string version;

            string statement = "SELECT sqlite_version()";

            using (var cmd = new SQLiteCommand(statement, conn))
            {
                version = cmd.ExecuteScalar() as string;
            }

            return version;
        }

        /// <summary>
        /// Should be called once at the beginning of each session.
        /// Configures the database to enforce foreign key constraints.
        /// </summary>
        /// <param name="conn"></param>
        public static void EnableForeignKeys(SQLiteConnection conn)
        {
            string statement = "PRAGMA foreign_keys = ON;";

            using (var cmd = new SQLiteCommand(statement, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Creates a table for the given type.
        /// If unsure if the table already exists, use CreateTableIfNotExists() instead.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        public static void CreateTable<T>(SQLiteConnection conn) where T : class
        {
            Type type = typeof(T);
            string statement = Statements.MakeCreateTableStatement(type, false);

            using (var cmd = new SQLiteCommand(statement, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Creates a table for the given type, if one does not exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        public static void CreateTableIfNotExists<T>(SQLiteConnection conn) where T : class
        {
            Type type = typeof(T);
            string statement = Statements.MakeCreateTableStatement(type, true);

            using (var cmd = new SQLiteCommand(statement, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Retrieves all records from a table and returns them as a list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static IList<T> GetAll<T>(SQLiteConnection conn) where T : class
        {
            return GetAll<T>(conn, null);
        }

        /// <summary>
        /// Gets the record from the table matching the primary key value, if one exists.
        /// Returns null if none exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKeyValue"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static T Get<T>(object primaryKeyValue, SQLiteConnection conn) where T : class
        {
            var primaryKeyProp = SqlUtility.GetPropertiesWithAttribute(typeof(T), typeof(SqlPrimaryKeyAttribute)).FirstOrDefault();
            if (primaryKeyProp == null)
            {
                throw new ArgumentException("No property found with SqlPrimaryKeyAttribute for type " + typeof(T));
            }
            var primaryKeyAttribute = primaryKeyProp.GetCustomAttribute<SqlPrimaryKeyAttribute>();

            string condition = primaryKeyAttribute.ColumnName + " = " + SqlUtility.ToSqlString(primaryKeyValue, primaryKeyProp.PropertyType);
            return GetAll<T>(conn, condition).FirstOrDefault();
        }

        /// <summary>
        /// Returns a list of all records in the table matching the given condition.
        /// Condition should include column name(s) and value(s).
        /// Ex: "Make = 'Mazda' AND ModelYear = 2003"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IList<T> GetAll<T>(SQLiteConnection conn, string condition) where T : class
        {
            Type type = typeof(T);
            if (!type.IsDefined(typeof(SqlTableAttribute)))
            {
                throw new ArgumentException(string.Format("No SqlTableAttribute found on type {0}", type));
            }

            List<T> resultsList = new List<T>();

            var tableAttribute = type.GetCustomAttribute<SqlTableAttribute>();

            string query = "SELECT * FROM " + tableAttribute.TableName;

            if (!string.IsNullOrEmpty(condition))
            {
                query += " WHERE " + condition;
            }

            query += ";";

            using (var adapter = new SQLiteCommand(query, conn))
            {
                using (var reader = adapter.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    foreach (DataRow row in dt.Rows)
                    {
                        var item = Activator.CreateInstance<T>();

                        foreach (var prop in SqlUtility.GetPropertiesWithAttribute(type, typeof(SqlColumnAttribute)))
                        {
                            var columnAttribute = prop.GetCustomAttribute<SqlColumnAttribute>();

                            prop.SetValue(item, SqlUtility.FromSql(prop.PropertyType, row[columnAttribute.ColumnName]));
                        }

                        var primaryKeyProperty = SqlUtility.GetPropertiesWithAttribute(type, typeof(SqlPrimaryKeyAttribute)).FirstOrDefault();
                        if (primaryKeyProperty != null)
                        {
                            var primaryKeyAttribute = primaryKeyProperty.GetCustomAttribute<SqlPrimaryKeyAttribute>();
                            primaryKeyProperty.SetValue(item, SqlUtility.FromSql(primaryKeyProperty.PropertyType, row[primaryKeyAttribute.ColumnName]));
                        }

                        resultsList.Add(item);
                    }
                }
            }

            return resultsList;
        }

        /// <summary>
        /// Returns a list of records that match the given foreign key value.
        /// If the table contains multiple foreign keys use the overload where a property name can be specified instead. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="foreignKey"></param>
        /// <returns></returns>
        public static IList<T> GetByForeignKey<T>(SQLiteConnection conn, object foreignKey) where T : class
        {
            var foreignKeyProp = SqlUtility.GetPropertiesWithAttribute(typeof(T), typeof(SqlForeignKeyAttribute)).FirstOrDefault();
            if (foreignKeyProp == null)
            {
                throw new ArgumentException("No properties with SqlForeignKeyAttribute found on type " + typeof(T));
            }

            return GetByForeignKey<T>(conn, foreignKeyProp.Name, foreignKey);
        }

        /// <summary>
        /// Returns a list of records that match the given foreign key value.
        /// propertyName is the name of the property with the foreign key attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="propertyName"></param>
        /// <param name="foreignKey"></param>
        /// <returns></returns>
        public static IList<T> GetByForeignKey<T>(SQLiteConnection conn, string propertyName, object foreignKey) where T : class
        {
            Type type = typeof(T);
            if (!type.IsDefined(typeof(SqlTableAttribute)))
            {
                throw new ArgumentException(string.Format("No SqlTableAttribute found on type {0}", type));
            }

            var tableAttribute = type.GetCustomAttribute<SqlTableAttribute>();

            var foreignKeyProperty = type.GetProperty(propertyName, PROPERTY_FLAGS);
            if (foreignKeyProperty == null)
            {
                throw new ArgumentException("Unable to find property " + propertyName + " for type " + type);
            }

            var foreignKeyAttribute = foreignKeyProperty.GetCustomAttribute<SqlForeignKeyAttribute>();
            if (foreignKeyAttribute == null)
            {
                throw new ArgumentException("No foreign key attribute found on property " + propertyName);
            }

            string filter = foreignKeyAttribute.ColumnName + " = " + SqlUtility.ToSqlString(foreignKey, foreignKeyProperty.PropertyType);
            return GetAll<T>(conn, filter);
        }

        /// <summary>
        /// Inserts a record into the table.
        /// Returns the number of rows affected.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert<T>(T item, SQLiteConnection conn) where T : class
        {
            string statement = Statements.MakeInsertStatement(item);

            int result = 0;

            using (var cmd = new SQLiteCommand(statement, conn))
            {
                result = cmd.ExecuteNonQuery();

                if (result == 1)
                {
                    var primaryKeyProp = SqlUtility.GetPropertiesWithAttribute(typeof(T), typeof(SqlPrimaryKeyAttribute)).FirstOrDefault();
                    if (primaryKeyProp != null)
                    {
                        primaryKeyProp.SetValue(item, conn.LastInsertRowId);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Inserts a list of records into the table in a single transaction.
        /// Returns the number of rows affected.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int InsertAll<T>(IList<T> items, SQLiteConnection conn) where T : class
        {
            int recordsUpdated = 0;

            if (items != null && items.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("BEGIN TRANSACTION;");
                sb.Append(Environment.NewLine);
                foreach (var item in items)
                {
                    sb.Append(Statements.MakeInsertStatement(item));
                    sb.Append(Environment.NewLine);
                }
                sb.Append("COMMIT;");

                string statement = sb.ToString();
                using (var cmd = new SQLiteCommand(statement, conn))
                {
                    recordsUpdated = cmd.ExecuteNonQuery();
                }
            }

            return recordsUpdated;
        }

        /// <summary>
        /// Updates an existing record in the table.
        /// Returns the number of rows affected.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update<T>(T item, SQLiteConnection conn) where T : class
        {
            string statement = Statements.MakeUpdateStatement(item);

            int result = 0;

            using (var cmd = new SQLiteCommand(statement, conn))
            {
                result = cmd.ExecuteNonQuery();
            }

            return result;
        }

        /// <summary>
        /// Drops the record from the table.
        /// Returns the number of rows affected.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="record"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete<T>(T record, SQLiteConnection conn) where T : class
        {
            string statement = Statements.MakeDeleteStatement(record);

            int result = 0;

            using (var cmd = new SQLiteCommand(statement, conn))
            {
                result = cmd.ExecuteNonQuery();
            }

            return result;
        }

        /// <summary>
        /// Deletes all records from the table with the given foreign key.
        /// propertyName is the name of the property with the foreign key attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="propertyName"></param>
        /// <param name="foreignKey"></param>
        /// <returns></returns>
        public static int DeleteByForeignKey<T>(SQLiteConnection conn, string propertyName, object foreignKey) where T : class
        {
            string statement = Statements.MakeDeleteByForeignKeyStatement<T>(propertyName, foreignKey);

            int rowsAffected = 0;

            using (var cmd = new SQLiteCommand(statement, conn))
            {
                rowsAffected = cmd.ExecuteNonQuery();
            }

            return rowsAffected;
        }

    }
}
