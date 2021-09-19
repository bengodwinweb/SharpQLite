using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQLite
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SqlTableAttribute : Attribute
    {

        public string TableName { get; set; }

        public SqlTableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
