using RandomNumberGenerator.App_System;
using RandomNumberGenerator.ViewModel.Core;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace RandomNumberGenerator.Model.Extensions
{
    public static class EntityExtensions
    {
        private static DataTable ToDataTable<T>(this IEnumerable<T> items) where T : class
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prioterty in properties)
            {
                dataTable.Columns.Add(prioterty.Name);
            }
            foreach (T item in items)
            {
                var values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static void BulkInsert<T>(this IQueryable<T> entity, IEnumerable<T> items, IProgressObserver progressObserver) where T : class
        {
            using (var connection = new SqlConnection(AppConfiguration.GetConnectionString()))
            {
                connection.Open();

                var transaction = connection.BeginTransaction();

                int batchSize = AppConfiguration.GetResultSaveBatchSize();
                int batchCount = items.Count() / batchSize;

                batchCount += items.Count() % batchSize > 0 ? 1 : 0;

                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.BulkCopyTimeout = AppConfiguration.GetConnectionTimeout();

                    for (int i = 0; i < batchCount; i++)
                    {
                        IEnumerable<T> batch = items.Skip(i * batchSize).Take(batchSize);

                        DataTable dataTable = batch.ToDataTable();
                        bulkCopy.DestinationTableName = dataTable.TableName;
                        bulkCopy.WriteToServer(dataTable);

                        progressObserver.InvokeAction(i + 1, batchCount);
                    }
                }

                transaction.Commit();
                connection.Close();
            }
        }
    }
}
