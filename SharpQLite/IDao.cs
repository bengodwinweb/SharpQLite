using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQLite
{
    /// <summary>
    /// Interface for Data Access Object generic that handles database transactions for a table.
    /// T must have SqlTableAttribute on the class and SqlColumnAttributes on column properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDao<T>
    {

        /// <summary>
        /// Returns all records
        /// </summary>
        /// <returns></returns>
        bool GetAll(out IList<T> records, out string errorMessage);

        /// <summary>
        /// Returns all records matching the condition.
        /// Condition should be formatted as the WHERE portion of a SQL SELECT statement.
        /// Ex.: "last_name = 'Smith' AND age = 49"
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        bool GetAll(string condition, out IList<T> records, out string errorMessage);

        /// <summary>
        /// Returns the record with matching ID, or none if not found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Get(long id, out T record, out string errorMessage);

        /// <summary>
        /// Inserts a newly created record in the DB.
        /// Sets the primary key property after inserting. 
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        bool Add(T record, out string errorMessage);

        /// <summary>
        /// Inserts a list of newly created records in the DB in a single transaction.
        /// Does not set the primary key property on each item.
        /// </summary>
        /// <param name="records"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool AddAll(IList<T> records, out string errorMessage);

        /// <summary>
        /// Update a previously existing record. 
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        bool Update(T record, out string errorMessage);

        /// <summary>
        /// Delete an existing record
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        bool Delete(T record, out string errorMessage);
    }
}
