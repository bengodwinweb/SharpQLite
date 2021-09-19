using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQLite
{
    [Flags]
    public enum SqlColumnConstraintFlags
    {
        None = 0,
        NotNull = 1,
        Unique = 2,
        Default = 4
    }
}
