using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQLite.Tests
{

    [SqlTable("Advisors")]
    class Advisor
    {
        [SqlPrimaryKey("AdvisorID")]
        public long Id { get; set; }

        [SqlColumn(nameof(FirstName), Constraints = SqlColumnConstraintFlags.NotNull, NotNullOnConflict = SqlConflictAction.FAIL)]
        public string FirstName { get; set; }

        [SqlColumn(nameof(LastName))]
        public string LastName { get; set; }

        [SqlColumn(nameof(RoomNumber), Constraints = SqlColumnConstraintFlags.Default | SqlColumnConstraintFlags.Unique, DefaultValue = "1124")]
        public string RoomNumber { get; set; }

    }
}
