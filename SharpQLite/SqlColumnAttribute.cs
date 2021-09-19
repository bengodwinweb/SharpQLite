using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQLite
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class SqlColumnAttribute : Attribute
    {
        public string ColumnName { get; set; }

        public bool NotNull { get; set; }

        public SqlColumnConstraintFlags Constraints { get; set; }

        public SqlConflictAction NotNullOnConflict { get; set; }

        public SqlConflictAction UniqueOnConflict { get; set; }

        public object DefaultValue { get; set; }

        public SqlColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }


    public static class SqlColumnAttributeExtender
    {
        public static string GetConstraintString(this SqlColumnAttribute attrib)
        {
            if (attrib == null || attrib.Constraints == SqlColumnConstraintFlags.None)
            {
                return string.Empty;
            }

            StringBuilder constraints = new StringBuilder();

            if (attrib.Constraints.HasFlag(SqlColumnConstraintFlags.NotNull))
            {
                constraints.Append(string.Format(" NOT NULL ON CONFLICT {0}", attrib.NotNullOnConflict.ToString()));
            }

            if (attrib.Constraints.HasFlag(SqlColumnConstraintFlags.Unique))
            {
                constraints.Append(string.Format(" UNIQUE ON CONFLICT {0}", attrib.UniqueOnConflict.ToString()));
            }

            if (attrib.Constraints.HasFlag(SqlColumnConstraintFlags.Default) && attrib.DefaultValue != null)
            {
                constraints.Append(string.Format(" DEFAULT ({0})", attrib.DefaultValue.ToString()));
            }

            return constraints.ToString();
        }
    }
}
