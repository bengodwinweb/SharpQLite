using System.ComponentModel;

namespace SharpQLite
{
    public enum SqlParentChangedAction
    {
        NO_ACTION,
        RESTRICT,
        SET_NULL,
        SET_DEFAULT,
        CASCADE
    }
}
