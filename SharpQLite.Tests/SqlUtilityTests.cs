using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQLite.Tests
{
    [TestFixture]
    class SqlUtilityTests
    {

        [Test]
        public void TestFromSqlBool()
        {
            Assert.AreEqual(true, SqlUtility.FromSql(typeof(bool), 1));
            Assert.AreEqual(true, SqlUtility.FromSql(typeof(bool), "1"));
            Assert.AreEqual(false, SqlUtility.FromSql(typeof(bool), 0));
            Assert.AreEqual(false, SqlUtility.FromSql(typeof(bool), "0"));
        }

        [Test]
        public void TestFromSqlChar()
        {
            Assert.AreEqual('C', SqlUtility.FromSql(typeof(char), "C"));
            Assert.AreEqual('C', SqlUtility.FromSql(typeof(char), 'C'));
            Assert.AreEqual('C', SqlUtility.FromSql(typeof(char), 0x43));

            Assert.AreEqual('y', SqlUtility.FromSql(typeof(char), "y"));
            Assert.AreEqual('y', SqlUtility.FromSql(typeof(char), 'y'));
            Assert.AreEqual('y', SqlUtility.FromSql(typeof(char), 0x79));
        }

        [Test]
        public void TestFromSqlString()
        {
            Assert.AreEqual("Comments, comment string", SqlUtility.FromSql(typeof(string), "Comments, comment string"));
            Assert.AreEqual("123.456", SqlUtility.FromSql(typeof(string), 123.456));
        }

        [Test]
        public void TestFromSqlDateTime()
        {
            Assert.AreEqual(new DateTime(2021, 08, 17, 13, 27, 11, 668, DateTimeKind.Utc), SqlUtility.FromSql(typeof(DateTime), "2021-08-17 13:27:11:668"));
        }

        [Test]
        public void TestFromSqlDecimal()
        {
            Assert.AreEqual(2.267892892304813m, SqlUtility.FromSql(typeof(decimal), "2.267892892304813"));
            Assert.AreEqual(2.26789289230481m, SqlUtility.FromSql(typeof(decimal), 2.267892892304813));
        }

        [Test]
        public void TestFromSqlDouble()
        {
            Assert.AreEqual(2.267892892304813, (double) SqlUtility.FromSql(typeof(double), "2.267892892304813"), 0.000001);
            Assert.AreEqual(2.267892892304813, (double) SqlUtility.FromSql(typeof(double), 2.267892892304813), 0.000001);
        }

        [Test]
        public void TestFromSqlFloat()
        {
            Assert.AreEqual(2.267892892304813f, (float) SqlUtility.FromSql(typeof(float), "2.267892892304813"), 0.0001);
            Assert.AreEqual(2.267892892304813f, (float) SqlUtility.FromSql(typeof(float), 2.267892892304813), 0.0001);
        }

        [Test]
        public void TestFromSqlByte()
        {
            Assert.AreEqual(0x89, SqlUtility.FromSql(typeof(byte), 0x89L));
            Assert.AreEqual(0x89, SqlUtility.FromSql(typeof(byte), "137"));
        }

        [Test]
        public void TestFromSqlSbyte()
        {
            Assert.AreEqual(-9, SqlUtility.FromSql(typeof(sbyte), -9L));
            Assert.AreEqual(-9, SqlUtility.FromSql(typeof(sbyte), "-9"));
        }

    }
}
