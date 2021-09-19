using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQLite
{
    public enum SqlConflictAction
    {
        ABORT,
        ROLLBACK,
        FAIL,
        IGNORE,
        REPLACE
    }
}
