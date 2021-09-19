using System;

namespace SharpQLite.Tests
{
    [SqlTable("Students")]
    class Student
    {
        [SqlPrimaryKey("StudentID", AutoIncrement = true)]
        public long Id { get; set; }

        [SqlColumn(nameof(FirstName))]
        public string FirstName { get; set; }

        [SqlColumn(nameof(LastName))]
        public string LastName { get; set; }

        [SqlColumn("DateOfBirth")]
        public DateTime Dob { get; set; }

        [SqlColumn("GradePointAverage")]
        public double Gpa { get; set; }

        [SqlColumn(nameof(ZipCode))]
        public int ZipCode { get; set; }

        [SqlForeignKey("Advisors", "AdvisorID", Constraints = SqlColumnConstraintFlags.NotNull, OnParentDelete = SqlParentChangedAction.CASCADE)]
        private long _advisorId { get; set; }

        public void SetAdvisorId(long id)
        {
            _advisorId = id;
        }

    }
}
