using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQLite
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class SqlForeignKeyAttribute : SqlColumnAttribute
    {
        public string ParentTable { get; set; }

        public SqlParentChangedAction OnParentUpdate { get; set; }

        public SqlParentChangedAction OnParentDelete { get; set; }

        public SqlForeignKeyAttribute(string parentTable, string parentColumn) : base(parentColumn)
        {
            ParentTable = parentTable;
        }
    }
}
