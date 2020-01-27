using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
namespace LFLens
{
    class AzureTableManager
    {
        private CloudTable table;

        /// <summary>
        /// Create Cloud Table object
        /// </summary>
        /// <param name="TableName">Name of the Table Name</param>
        public AzureTableManager(string TableName)
        {
            // Check if Table Name is blank
            if (string.IsNullOrEmpty(TableName))
            {
                throw new ArgumentNullException("Table", "Table Name can't be empty");
            }
            try
            {
                // Get azure table storage connection string.
                string ConnectionString = OAuthConstants.ConnectionString;

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);

                // Create the table if not exist and put the refarence of the table into Cloud Table object
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                table = tableClient.GetTableReference(TableName);
                table.CreateIfNotExistsAsync();
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        /// <summary>
        /// Insert or Update Method
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="entity">T type object</param>
        /// <param name="forInsert">true for Insert, false for Update</param>
        public void InsertEntity<T>(T entity, bool forInsert = true) where T : TableEntity, new()
        {
            try
            {
                if (forInsert)
                {
                    var insertOperation = TableOperation.Insert(entity);
                    table.ExecuteAsync(insertOperation);
                }
                else
                {
                    var insertOrMergeOperation = TableOperation.InsertOrReplace(entity);
                    table.ExecuteAsync(insertOrMergeOperation);
                }
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        /// <summary>
        /// Retrieve List of T type entity
        /// </summary>
        /// <typeparam name="T">Returned Entity Type</typeparam>
        /// <param name="Query"></param>
        /// <returns>List of T type object</returns>
        //public List<T> RetrieveEntity<T>(string Query = null) where T : TableEntity, new()
        //{
        //    try
        //    {
        //        TableContinuationToken continuationToken = null;
        //        TableQuery<T> DataTableQuery = new TableQuery<T>();
        //        if (!string.IsNullOrEmpty(Query))
        //        {
        //            DataTableQuery = new TableQuery<T>().Where(Query);
        //        }
        //        IEnumerable<T> IDataList = table.ExecuteAsync(DataTableQuery);
        //        List<T> DataList = new List<T>();
        //        foreach (var singleData in IDataList)
        //            DataList.Add(singleData);
        //        return DataList;
        //    }
        //    catch (Exception ExceptionObj)
        //    {
        //        throw ExceptionObj;
        //    }
        //}

        /// <summary>
        /// Delete the entity
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="entity">T type entity</param>
        /// <returns>true if able to delete</returns>
        public bool DeleteEntity<T>(T entity) where T : TableEntity, new()
        {
            try
            {
                var DeleteOperation = TableOperation.Delete(entity);
                table.ExecuteAsync(DeleteOperation);
                return true;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }
    }
}
