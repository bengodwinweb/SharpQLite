using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQLite.Tests
{
    [TestFixture]
    class StatementsTests
    {

        [Test]
        public void TestMakeCreateTableStatement()
        {
            string statement = Statements.MakeCreateTableStatement(typeof(Advisor), false);
            Console.WriteLine("Advisor: ");
            Console.WriteLine(statement);

            /*
             * CREATE TABLE Advisors (
	         *      AdvisorID INTEGER PRIMARY KEY AUTOINCREMENT,
	         *      FirstName TEXT NOT NULL ON CONFLICT FAIL,
	         *      LastName TEXT,
	         *      RoomNumber TEXT UNIQUE ON CONFLICT ABORT DEFAULT (1124)
             *  );
             */

            Assert.AreEqual(
                "CREATE TABLE Advisors (\r\n" +
                "\tAdvisorID INTEGER PRIMARY KEY AUTOINCREMENT,\r\n" +
                "\tFirstName TEXT NOT NULL ON CONFLICT FAIL,\r\n" +
                "\tLastName TEXT,\r\n" +
                "\tRoomNumber TEXT UNIQUE ON CONFLICT ABORT DEFAULT (1124)\r\n" +
                ");", 
                statement);
        }

        [Test]
        public void TestMakeCreateTableStatementIfNotExists()
        {
            string statement = Statements.MakeCreateTableStatement(typeof(Student), true);
            Console.WriteLine("Student: ");
            Console.WriteLine(statement);

            /*
             * CREATE TABLE Students(
	         *      StudentID INTEGER PRIMARY KEY AUTOINCREMENT,
	         *      FirstName TEXT,
	         *      LastName TEXT,
	         *      DateOfBirth TEXT,
	         *      GradePointAverage REAL,
	         *      ZipCode INTEGER,
	         *      AdvisorID INTEGER NOT NULL ON CONFLICT ABORT,
	         *      FOREIGN KEY (AdvisorID) REFERENCES Advisor (AdvisorID) ON DELETE CASCADE ON UPDATE NO ACTION
             *  );
             */

            Assert.AreEqual(
                "CREATE TABLE IF NOT EXISTS Students (\r\n" +
                "\tStudentID INTEGER PRIMARY KEY AUTOINCREMENT,\r\n" +
                "\tFirstName TEXT,\r\n" +
                "\tLastName TEXT,\r\n" +
                "\tDateOfBirth TEXT,\r\n" +
                "\tGradePointAverage REAL,\r\n" +
                "\tZipCode INTEGER,\r\n" +
                "\tAdvisorID INTEGER NOT NULL ON CONFLICT ABORT,\r\n" +
                "\tFOREIGN KEY (AdvisorID) REFERENCES Advisors (AdvisorID) ON DELETE CASCADE ON UPDATE NO ACTION\r\n" +
                ");",
                statement);
        }

        [Test]
        public void TestMakeInsertStatement()
        {
            var a = new Advisor()
            {
                FirstName = "Ornath",
                LastName = "Pennridge",
                RoomNumber = "9978"
            };

            var insertStatement = Statements.MakeInsertStatement(a);
            Console.WriteLine(insertStatement);

            /*
             * INSERT INTO Advisors (
	         *      FirstName,
	         *      LastName,
	         *      RoomNumber
             *  ) VALUES (
	         *      "Ornath",
	         *      "Pennridge",
	         *      "9978"
             *  );
             */

            Assert.AreEqual(
                "INSERT INTO Advisors (\r\n" +
                "\tFirstName,\r\n" +
                "\tLastName,\r\n" +
                "\tRoomNumber\r\n" +
                ") VALUES (\r\n" +
                "\t\"Ornath\",\r\n" +
                "\t\"Pennridge\",\r\n" +
                "\t\"9978\"\r\n" +
                ");",
                insertStatement);
        }

        [Test]
        public void TestMakeUpdateStatment()
        {
            var a = new Advisor()
            {
                Id = 3,
                FirstName = "Ornath",
                LastName = "Pennridge",
                RoomNumber = "1177",
            };

            var updateStatement = Statements.MakeUpdateStatement(a);
            Console.WriteLine(updateStatement);

            Assert.AreEqual(
                "UPDATE Advisors\r\n" +
                "SET\r\n" +
                "\tFirstName = \"Ornath\",\r\n" +
                "\tLastName = \"Pennridge\",\r\n" +
                "\tRoomNumber = \"1177\"\r\n" +
                "WHERE\r\n" +
                 "\tAdvisorID = 3;",
                updateStatement);
        }

        [Test]
        public void TestMakeInsertStatementStudent()
        {
            var s = new Student()
            {
                FirstName = "Eddie",
                LastName = "Que",
                Dob = new DateTime(2012, 5, 10),
                Gpa = 2.9,
                ZipCode = 66790,
            };

            s.SetAdvisorId(4);

            var insertStatement = Statements.MakeInsertStatement(s);
            Console.WriteLine(insertStatement);

            /*
             * INSERT INTO Students (
	         *      FirstName,
	         *      LastName,
	         *      DateOfBirth,
	         *      GradePointAverage,
	         *      ZipCode,
	         *      AdvisorID
             *  ) VALUES (
	         *      "Eddie",
	         *      "Que",
	         *      "2012-05-10 00:00:00:000",
	         *      2.9,
	         *      66790,
	         *      4
             *  ); 
             */

            Assert.AreEqual(
                "INSERT INTO Students (\r\n" +
                "\tFirstName,\r\n" +
                "\tLastName,\r\n" +
                "\tDateOfBirth,\r\n" +
                "\tGradePointAverage,\r\n" +
                "\tZipCode,\r\n" +
                "\tAdvisorID\r\n" +
                ") VALUES (\r\n" +
                "\t\"Eddie\",\r\n" +
                "\t\"Que\",\r\n" +
                "\t\"2012-05-10 00:00:00:000\",\r\n" +
                "\t2.9,\r\n" +
                "\t66790,\r\n" +
                "\t4\r\n" +
                ");",
                insertStatement);
        }
    }
}
