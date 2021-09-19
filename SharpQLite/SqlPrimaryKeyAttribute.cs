using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQLite
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class SqlPrimaryKeyAttribute : Attribute
    {
        public bool AutoIncrement { get; set; }

        public string ColumnName { get; set; }

        public SqlPrimaryKeyAttribute(string columnName)
        {
            AutoIncrement = true;
            ColumnName = columnName;
        }
    }
}
