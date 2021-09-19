using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQLite.Tests
{
    [TestFixture]
    class SQLiteTests
    {

        [Test]
        public void TestGetVersion()
        {
            using (var conn = new SQLiteConnection("Data Source=:memory:;Version=3;"))
            {
                conn.Open();

                string versionString = SQLite.GetSqlVersion(conn);
                Console.WriteLine("SQL Version: " + versionString);

                Assert.IsFalse(string.IsNullOrEmpty(versionString));
                Assert.IsTrue(versionString.StartsWith("3."));
            }
        }

 

 

        [Test]
        public void TestMakeTable()
        {
            using (var conn = new SQLiteConnection("Data Source=:memory:"))
            {
                conn.Open();

                string getAdvisorTableStatement = "SELECT name FROM sqlite_master WHERE type='table' AND name='Advisors'";

                var cmd = new SQLiteCommand(getAdvisorTableStatement, conn);
                Assert.IsNull(cmd.ExecuteScalar());

                SQLite.CreateTable<Advisor>(conn);

                string tableName = cmd.ExecuteScalar() as string;
                Assert.AreEqual("Advisors", tableName);
            }
        }


        [Test]
        public void TestInsert()
        {
            List<Advisor> advisors = new List<Advisor>();
            advisors.Add(new Advisor()
            {
                FirstName = "Jeb",
                LastName = "Arange",
                RoomNumber = "5565"
            });
            advisors.Add(new Advisor()
            {
                FirstName = "Suse",
                LastName = "Zenith",
                RoomNumber = "-5"
            });
            advisors.Add(new Advisor()
            {
                FirstName = "Ornath",
                LastName = "Pennridge",
                RoomNumber = "9978"
            });


            using (var conn = new SQLiteConnection("Data Source=:memory:"))
            {
                conn.Open();

                SQLite.CreateTable<Advisor>(conn);

                int rowsInserted = 0;

                foreach (var advisor in advisors)
                {
                    rowsInserted += SQLite.Insert(advisor, conn);
                    Assert.AreEqual(rowsInserted, advisor.Id);
                }

                Assert.AreEqual(3, rowsInserted);
            }
        }

        [Test]
        public void TestAll()
        {
            List<Advisor> advisors = new List<Advisor>();
            advisors.Add(new Advisor()
            {
                FirstName = "Jeb",
                LastName = "Arange",
                RoomNumber = "5565"
            });
            advisors.Add(new Advisor()
            {
                FirstName = "Suse",
                LastName = "Zenith",
                RoomNumber = "-5"
            });
            advisors.Add(new Advisor()
            {
                FirstName = "Ornath",
                LastName = "Pennridge",
                RoomNumber = "9978"
            });

            List<Student> students = new List<Student>();
            students.Add(new Student()
            {
                FirstName = "Jeff",
                LastName = "Student",
                Dob = new DateTime(2012, 09, 10),
                Gpa = 3.9,
                ZipCode = 99999
            });
            students.Add(new Student()
            {
                FirstName = "Abitha",
                LastName = "Taranath",
                Dob = new DateTime(2013, 1, 2),
                Gpa = 4.1,
                ZipCode = 11354
            });
            students.Add(new Student()
            {
                FirstName = "Eddie",
                LastName = "Que",
                Dob = new DateTime(2012, 5, 10),
                Gpa = 2.9,
                ZipCode = 66790
            });


            using (var conn = new SQLiteConnection("Data Source=:memory:"))
            {
                conn.Open();
                SQLite.EnableForeignKeys(conn);

                SQLite.CreateTable<Advisor>(conn);
                SQLite.CreateTableIfNotExists<Student>(conn);

                foreach (var advisor in advisors)
                {
                    SQLite.Insert(advisor, conn);
                }

                var loadedAdvisors = SQLite.GetAll<Advisor>(conn);

                Assert.AreEqual(3, loadedAdvisors.Count);

                Assert.AreEqual("Arange", loadedAdvisors[0].LastName);
                Assert.AreEqual(1, loadedAdvisors[0].Id);
                Assert.AreEqual("9978", loadedAdvisors[2].RoomNumber);
                Assert.AreEqual(3, loadedAdvisors[2].Id);

                students[0].SetAdvisorId(loadedAdvisors[0].Id);
                students[1].SetAdvisorId(loadedAdvisors[2].Id);
                students[2].SetAdvisorId(loadedAdvisors[2].Id);

                var ornath = advisors[2];
                ornath.RoomNumber = "1123";
                var recordsUpdated = SQLite.Update(ornath, conn);
                Assert.AreEqual(1, recordsUpdated);

                var ornathFromDb = SQLite.Get<Advisor>(ornath.Id, conn);
                Assert.IsNotNull(ornathFromDb);
                Assert.AreEqual(ornath.Id, ornathFromDb.Id);
                Assert.AreEqual("1123", ornathFromDb.RoomNumber);

                foreach (var student in students)
                {
                    recordsUpdated = SQLite.Insert(student, conn);
                    Assert.AreEqual(1, recordsUpdated);
                }

                var loadedStudents = SQLite.GetAll<Student>(conn);
                Assert.AreEqual(3, loadedStudents.Count);
                Assert.AreEqual("Jeff", loadedStudents[0].FirstName);
                Assert.AreEqual(99999, loadedStudents[0].ZipCode);
                Assert.AreEqual("Taranath", loadedStudents[1].LastName);
                Assert.AreEqual(4.1, loadedStudents[1].Gpa);
                Assert.AreEqual(new DateTime(2012, 5, 10), loadedStudents[2].Dob);
                Assert.AreEqual(66790, loadedStudents[2].ZipCode);

                loadedStudents = SQLite.GetByForeignKey<Student>(conn, ornath.Id);
                Assert.AreEqual(2, loadedStudents.Count);
                Assert.AreEqual("Abitha", loadedStudents[0].FirstName);
                Assert.AreEqual("Eddie", loadedStudents[1].FirstName);

                recordsUpdated = SQLite.Delete(ornath, conn);
                Assert.AreEqual(1, recordsUpdated);

                var allAdvisors = SQLite.GetAll<Advisor>(conn);
                Assert.AreEqual(2, allAdvisors.Count);
                Assert.IsFalse(allAdvisors.Any(x => x.Id == 3));

                var allStudents = SQLite.GetAll<Student>(conn);
                Assert.AreEqual(1, allStudents.Count);
            }
        }



    }
}
