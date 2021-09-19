using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpQLite
{
    internal static class SqlUtility
    {

        #region Reflection
        private const BindingFlags PROPERTY_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        internal static IEnumerable<PropertyInfo> GetPropertiesWithAttribute(Type type, Type attributeType)
        {
            return type.GetProperties(PROPERTY_FLAGS).Where(prop => prop.IsDefined(attributeType));
        }

        internal static SqliteDatatype GetDatatypeFromProperty(PropertyInfo prop)
        {
            if (prop == null)
            {
                throw new ArgumentException("Cannot get datatype from null property");
            }

            switch (Type.GetTypeCode(prop.PropertyType))
            {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return SqliteDatatype.INTEGER;
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return SqliteDatatype.REAL;
                case TypeCode.DateTime:
                case TypeCode.String:
                    return SqliteDatatype.TEXT;
                default:
                    throw new ArgumentException(string.Format("No SQL Datatype mapping found for property type {0}", prop.PropertyType));
            }
        }
        #endregion

        #region From SQL
        public static object FromSql(Type propType, object value)
        {
            if (value is null)
            {
                return null;
            }

            var typeCode = Type.GetTypeCode(propType);

            switch (typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return Convert.ChangeType(Convert.ToUInt64(value), typeCode);
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return Convert.ChangeType(Convert.ToInt64(value), typeCode);
                case TypeCode.Boolean:
                    return Convert.ToBoolean(Convert.ToByte(value));
                case TypeCode.Char:
                    return Convert.ToChar(value);
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return Convert.ChangeType(Convert.ToDecimal(value), typeCode);
                case TypeCode.String:
                    return Convert.ToString(value);
                case TypeCode.DateTime:
                    string dateString = value as string;
                    if (string.IsNullOrEmpty(dateString))
                    {
                        return DateTime.MinValue;
                    }
                    return DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm:ss:fff", null, System.Globalization.DateTimeStyles.None);
                default:
                    throw new ArgumentException(string.Format("No SQL Datatype mapping found for property type {0}", propType));
            }
        }
        #endregion

        #region To SQL
        public static string ToSqlString(object value, Type propType)
        {
            if (value is null)
            {
                return "null";
            }

            var typeCode = Type.GetTypeCode(propType);

            switch (typeCode)
            {
                case TypeCode.SByte:
                    return ToSqlString((sbyte) Convert.ChangeType(value, typeof(sbyte)));
                case TypeCode.Byte:
                    return ToSqlString((byte) Convert.ChangeType(value, typeof(byte)));
                case TypeCode.Int16:
                    return ToSqlString((short) Convert.ChangeType(value, typeof(short)));
                case TypeCode.UInt16:
                    return ToSqlString((ushort) Convert.ChangeType(value, typeof(ushort)));
                case TypeCode.Int32:
                    return ToSqlString((int) Convert.ChangeType(value, typeof(int)));
                case TypeCode.UInt32:
                    return ToSqlString((uint) Convert.ChangeType(value, typeof(uint)));
                case TypeCode.Int64:
                    return ToSqlString((long) Convert.ChangeType(value, typeof(long)));
                case TypeCode.UInt64:
                    return ToSqlString((ulong) Convert.ChangeType(value, typeof(ulong)));
                case TypeCode.Boolean:
                    return ToSqlString((bool) Convert.ChangeType(value, typeof(bool)));
                case TypeCode.Char:
                    return ToSqlString((char) Convert.ChangeType(value, typeof(char)));
                case TypeCode.Single:
                    return ToSqlString((float) Convert.ChangeType(value, typeof(float)));
                case TypeCode.Double:
                    return ToSqlString((double) Convert.ChangeType(value, typeof(double)));
                case TypeCode.Decimal:
                    return ToSqlString((decimal) Convert.ChangeType(value, typeof(decimal)));
                case TypeCode.DateTime:
                    return ToSqlString((DateTime) Convert.ChangeType(value, typeof(DateTime)));
                case TypeCode.String:
                    return ToSqlString(value as string);
                default:
                    return string.Format("\"{0}\"", value.ToString());
            }
        }

        internal static string ToSqlString(DateTime dateTime)
        {
            return string.Format("\"{0}\"", dateTime.ToString("yyyy-MM-dd HH:mm:ss:fff"));
        }

        internal static string ToSqlString(string str)
        {
            return string.Format("\"{0}\"", str ?? "null");
        }

        internal static string ToSqlString(bool b)
        {
            return b ? "1" : "0";
        }

        internal static string ToSqlString(char c)
        {
            return ((int) c).ToString();
        }

        internal static string ToSqlString(double d)
        {
            return d.ToString();
        }

        internal static string ToSqlString(decimal d)
        {
            return d.ToString();
        }

        internal static string ToSqlString(float f)
        {
            return f.ToString();
        }

        internal static string ToSqlString(sbyte n)
        {
            return n.ToString();
        }

        internal static string ToSqlString(byte n)
        {
            return n.ToString();
        }

        internal static string ToSqlString(short n)
        {
            return n.ToString();
        }

        internal static string ToSqlString(ushort n)
        {
            return n.ToString();
        }

        internal static string ToSqlString(int n)
        {
            return n.ToString();
        }

        internal static string ToSqlString(uint n)
        {
            return n.ToString();
        }

        internal static string ToSqlString(long n)
        {
            return n.ToString();
        }

        internal static string ToSqlString(ulong n)
        {
            return n.ToString();
        }
        #endregion
    }
}
