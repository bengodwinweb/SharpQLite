using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQLite
{

    /// <summary>
    /// Data Access Object generic that handles database transactions for a table.
    /// T must have SqlTableAttribute on the class and SqlColumnAttributes on column properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Dao<T> : IDao<T> where T : class
    {
        private readonly string _connectionString;

        public Dao(string connectionString)
        {
            _connectionString = connectionString;
        }

        public virtual bool Add(T record, out string errorMessage)
        {
            int recordsUpdated = 0;

            if (record != null)
            {
                try
                {
                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        recordsUpdated = SQLite.Insert(record, conn);
                    }
                    errorMessage = string.Format("{0} records updated", recordsUpdated);
                    return recordsUpdated > 0;
                }
                catch (Exception e)
                {
                    errorMessage = e.Message;
                    return false;
                }
            }
            else
            {
                errorMessage = "Cannot add null record";
                return false;
            }
        }

        public bool AddAll(IList<T> records, out string errorMessage)
        {
            int recordsUpdated = 0;

            if (records != null)
            {
                try
                {
                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        recordsUpdated = SQLite.InsertAll(records, conn);
                    }
                    errorMessage = string.Format("{0} records updated", recordsUpdated);
                    return recordsUpdated == records.Count;
                }
                catch (Exception e)
                {
                    errorMessage = e.Message;
                    return false;
                }
            }
            else
            {
                errorMessage = "Cannot add null list";
                return false;
            }
        }

        public virtual bool Delete(T record, out string errorMessage)
        {
            int recordsUpdated = 0;

            if (record != null)
            {
                try
                {
                    using (var conn = GetConnection())
                    {
                        conn.Open();
                        recordsUpdated = SQLite.Delete(record, conn);
                    }
                    errorMessage = string.Format("{0} records updated", recordsUpdated);
                    return recordsUpdated > 0;
                }
                catch (Exception e)
                {
                    errorMessage = e.Message;
                    return false;
                }
            }
            else
            {
                errorMessage = "Cannot delete null record";
            }

            return recordsUpdated > 0;
        }

        public virtual bool Get(long id, out T record, out string errorMessage)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    record = SQLite.Get<T>(id, conn);
                }

                errorMessage = null;
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                record = default(T);
                return false;
            }
        }

        public virtual bool GetAll(out IList<T> records, out string errorMessage)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    records = SQLite.GetAll<T>(conn);
                }

                errorMessage = string.Empty;
                return true;
            }
            catch (Exception e)
            {
                records = new List<T>();
                errorMessage = e.Message;
                return false;
            }
        }

        public virtual bool GetAll(string condition, out IList<T> records, out string errorMessage)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    records = SQLite.GetAll<T>(conn, condition);
                }

                errorMessage = string.Empty;
                return true;
            }
            catch (Exception e)
            {
                records = new List<T>();
                errorMessage = e.Message;
                return false;
            }
        }

        public virtual bool Update(T record, out string errorMessage)
        {
            int recordsUpdated = 0;

            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    recordsUpdated = SQLite.Update(record, conn);
                }

                errorMessage = string.Format("{0} records updated", recordsUpdated);
                return recordsUpdated > 0;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }

        protected SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

    }
}
