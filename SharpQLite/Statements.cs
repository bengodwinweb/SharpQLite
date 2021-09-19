using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpQLite
{
    internal static class Statements
    {
        private const BindingFlags PROPERTY_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        internal static string MakeInsertStatement<T>(T item) where T : class
        {
            Type type = typeof(T);
            if (!type.IsDefined(typeof(SqlTableAttribute)))
            {
                throw new ArgumentException(string.Format("No SqlTableAttribute found on type {0}", type));
            }

            var tableAttribute = type.GetCustomAttribute<SqlTableAttribute>();

            StringBuilder sb = new StringBuilder();

            sb.Append("INSERT INTO ");
            sb.Append(tableAttribute.TableName);
            sb.Append(" (" + Environment.NewLine + '\t');

            var columnProps =SqlUtility.GetPropertiesWithAttribute(type, typeof(SqlColumnAttribute));

            List<string> columnNames = new List<string>();
            foreach (var prop in columnProps)
            {
                columnNames.Add(prop.GetCustomAttribute<SqlColumnAttribute>().ColumnName);
            }
            sb.Append(string.Join("," + Environment.NewLine + '\t', columnNames));

            sb.Append(Environment.NewLine + ") VALUES (" + Environment.NewLine + '\t');

            List<string> values = new List<string>();
            foreach (var prop in columnProps)
            {
                var columnAttribute = prop.GetCustomAttribute<SqlColumnAttribute>();
                var val = prop.GetValue(item);
                values.Add(SqlUtility.ToSqlString(val, prop.PropertyType));
            }
            sb.Append(string.Join("," + Environment.NewLine + '\t', values));

            sb.Append(Environment.NewLine + ");");
            return sb.ToString();
        }

        internal static string MakeUpdateStatement<T>(T item) where T : class
        {
            Type type = typeof(T);
            if (!type.IsDefined(typeof(SqlTableAttribute)))
            {
                throw new ArgumentException(string.Format("No SqlTableAttribute found on type {0}", type));
            }

            var tableAttribute = type.GetCustomAttribute<SqlTableAttribute>();

            StringBuilder sb = new StringBuilder();

            sb.Append("UPDATE ");
            sb.Append(tableAttribute.TableName);
            sb.Append(Environment.NewLine + "SET" + Environment.NewLine + '\t');

            var columnProps = SqlUtility.GetPropertiesWithAttribute(type, typeof(SqlColumnAttribute));

            List<string> setPairs = new List<string>();
            foreach (var prop in columnProps)
            {
                var columnAttribute = prop.GetCustomAttribute<SqlColumnAttribute>();
                string pairString = columnAttribute.ColumnName + " = " + SqlUtility.ToSqlString(prop.GetValue(item), prop.PropertyType);
                setPairs.Add(pairString);
            }
            sb.Append(string.Join("," + Environment.NewLine + '\t', setPairs));

            sb.Append(Environment.NewLine + "WHERE" + Environment.NewLine + '\t');

            var primaryKeyProperty = SqlUtility.GetPropertiesWithAttribute(type, typeof(SqlPrimaryKeyAttribute)).FirstOrDefault();
            var primaryKeyAttribute = primaryKeyProperty.GetCustomAttribute<SqlPrimaryKeyAttribute>();
            sb.Append(primaryKeyAttribute.ColumnName);
            sb.Append(" = ");
            sb.Append(SqlUtility.ToSqlString(primaryKeyProperty.GetValue(item), primaryKeyProperty.PropertyType));
            sb.Append(";");

            return sb.ToString();
        }

        internal static string MakeDeleteStatement<T>(T record) where T : class
        {
            Type type = typeof(T);
            if (!type.IsDefined(typeof(SqlTableAttribute)))
            {
                throw new ArgumentException(string.Format("No SqlTableAttribute found on type {0}", type));
            }

            var tableAttribute = type.GetCustomAttribute<SqlTableAttribute>();

            var primaryKeyProperty = SqlUtility.GetPropertiesWithAttribute(type, typeof(SqlPrimaryKeyAttribute)).FirstOrDefault();
            if (primaryKeyProperty == null)
            {
                throw new ArgumentException("No property with SqlPrimaryKeyAttribute found on type " + type);
            }
            var primaryKeyAttribute = primaryKeyProperty.GetCustomAttribute<SqlPrimaryKeyAttribute>();


            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM ");
            sb.Append(tableAttribute.TableName);
            sb.Append(" WHERE ");
            sb.Append(primaryKeyAttribute.ColumnName);
            sb.Append(" = ");
            sb.Append(SqlUtility.ToSqlString(primaryKeyProperty.GetValue(record), primaryKeyProperty.PropertyType));
            sb.Append(";");

            return sb.ToString();
        }

        internal static string MakeDeleteByForeignKeyStatement<T>(string propertyName, object foreignKey) where T : class
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

            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM ");
            sb.Append(tableAttribute.TableName);
            sb.Append(" WHERE ");
            sb.Append(foreignKeyAttribute.ColumnName);
            sb.Append(" = ");
            sb.Append(SqlUtility.ToSqlString(foreignKey, foreignKeyProperty.PropertyType));
            sb.Append(";");

            return sb.ToString();
        }


        /// <summary>
        /// Returns a string 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static string MakeCreateTableStatement(Type type, bool ifNotExists)
        {
            StringBuilder sb = new StringBuilder();

            var tableAttribute = type.GetCustomAttribute<SqlTableAttribute>();
            if (tableAttribute == null)
            {
                throw new ArgumentException(string.Format("No SqlTableAttribute found on type {0}", type));
            }

            sb.Append("CREATE TABLE ");
            if (ifNotExists)
            {
                sb.Append("IF NOT EXISTS ");
            }
            sb.Append(tableAttribute.TableName);
            sb.Append(" (" + Environment.NewLine + '\t');

            List<string> columnDefinitions = new List<string>();

            var primaryKeyProperties = SqlUtility.GetPropertiesWithAttribute(type, typeof(SqlPrimaryKeyAttribute));
            if (primaryKeyProperties.Count() > 1)
            {
                throw new ArgumentException(string.Format("Unable to create table for type {0}, multiple primary key attributes found", type));
            }

            foreach (var prop in primaryKeyProperties)
            {
                var primaryKeyAttribute = prop.GetCustomAttribute<SqlPrimaryKeyAttribute>();

                StringBuilder pkBuilder = new StringBuilder();
                pkBuilder.Append(primaryKeyAttribute.ColumnName);
                pkBuilder.Append(" ");
                pkBuilder.Append(SqlUtility.GetDatatypeFromProperty(prop).ToString());
                pkBuilder.Append(" PRIMARY KEY");

                if (primaryKeyAttribute.AutoIncrement)
                {
                    pkBuilder.Append(" AUTOINCREMENT");
                }

                columnDefinitions.Add(pkBuilder.ToString());
            }

            foreach (var prop in SqlUtility.GetPropertiesWithAttribute(type, typeof(SqlColumnAttribute)))
            {
                StringBuilder columnBuilder = new StringBuilder();

                var columnAttribute = prop.GetCustomAttribute<SqlColumnAttribute>();
                columnBuilder.Append(columnAttribute.ColumnName);
                columnBuilder.Append(" ");
                columnBuilder.Append(SqlUtility.GetDatatypeFromProperty(prop).ToString());
                columnBuilder.Append(columnAttribute.GetConstraintString());

                columnDefinitions.Add(columnBuilder.ToString());
            }

            foreach (var prop in SqlUtility.GetPropertiesWithAttribute(type, typeof(SqlForeignKeyAttribute)))
            {
                StringBuilder fkBuilder = new StringBuilder();

                var foreignKeyAttribute = prop.GetCustomAttribute<SqlForeignKeyAttribute>();
                fkBuilder.Append("FOREIGN KEY (");
                fkBuilder.Append(foreignKeyAttribute.ColumnName);
                fkBuilder.Append(") REFERENCES ");
                fkBuilder.Append(foreignKeyAttribute.ParentTable);
                fkBuilder.Append(" (");
                fkBuilder.Append(foreignKeyAttribute.ColumnName);
                fkBuilder.Append(") ON DELETE ");
                fkBuilder.Append(foreignKeyAttribute.OnParentDelete.ToString().Replace('_', ' '));
                fkBuilder.Append(" ON UPDATE ");
                fkBuilder.Append(foreignKeyAttribute.OnParentUpdate.ToString().Replace('_', ' '));

                columnDefinitions.Add(fkBuilder.ToString());
            }

            sb.Append(string.Join("," + Environment.NewLine + '\t', columnDefinitions));
            sb.Append(Environment.NewLine + ");");

            return sb.ToString();
        }
    }
}
