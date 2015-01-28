using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CustomTypes.Base;
using CustomTypes.Objects;
using Repository.DBClass;
using Repository.Enums;
using Repository.abstraction;
using Repository.tools;

namespace Repository
{
    public class ProductStockRepository<T> : RepositoryBase, IUniqueValidation, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_PRODUCT_STOCK");
                RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                cmd.Parameters.AddWithValue("@LastUpdatedBy", createdBy);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    objID = int.Parse(reader[0].ToString());
                }
            }
            return objID;
        }

        public int UpdateRow(T param, string updatedBy)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_STOCK");
                    RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Update);
                    cmd.Parameters.AddWithValue("@LastUpdatedBy", updatedBy);
                    DBClass.ExecuteNonQuery(cmd, txn);
                    txn.Commit();
                }
            }
            /* bypass compiler error need to be updated soon */
            return 0;
        }

        public int DeleteRow(int id, string updatedBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindAll(List<Dictionary<string, object>> keyValueParam)
        {
            var result = new List<ProductStock>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_STOCK");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var stock               = new ProductStock();
                    stock.StockID           = int.Parse(reader[0].ToString());
                    stock.ProductCode       = reader[1].ToString();
                    stock.ProductName       = reader[2].ToString();
                    stock.DepartementName   = reader[3].ToString();
                    stock.StartQuantity     = decimal.Parse(reader[4].ToString());
                    stock.CurrentQuantity   = decimal.Parse(reader[5].ToString());
                    stock.MinimumQuantity   = decimal.Parse(reader[6].ToString());
                    result.Add(stock);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<T> FindNormalStock(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<ProductStock>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_STOCK_WITH_NORMAL_STOCK");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var stock             = new ProductStock();
                    stock.StockID         = int.Parse(reader[0].ToString());
                    stock.ProductID       = int.Parse(reader[1].ToString());
                    stock.ProductCode     = reader[2].ToString();
                    stock.ProductName     = reader[3].ToString();
                    stock.DepartementName = reader[4].ToString();
                    stock.StartQuantity   = decimal.Parse(reader[5].ToString());
                    stock.CurrentQuantity = decimal.Parse(reader[6].ToString());
                    stock.MinimumQuantity = decimal.Parse(reader[7].ToString());
                    result.Add(stock);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<T> GetAllDepartementStock(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<ProductStock>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_DEPARTEMENT_STOCK");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var stock             = new ProductStock();
                    stock.StockID         = int.Parse(reader[0].ToString());
                    stock.ProductID       = int.Parse(reader[1].ToString());
                    stock.ProductCode     = reader[2].ToString();
                    stock.ProductName     = reader[3].ToString();
                    result.Add(stock);
                }
            }
            return result as List<T>;
        }

        public T FindbyId(int id)
        {
            var stock      = new ProductStock();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_STOCK_BY_ID");
                cmd.Parameters.AddWithValue("@StockId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    stock.StockID           = int.Parse(reader[0].ToString());
                    stock.ProductID         = int.Parse(reader[1].ToString());
                    stock.DepartementID     = int.Parse(reader[2].ToString());
                    stock.StartQuantity     = decimal.Parse(reader[3].ToString());
                    stock.CurrentQuantity   = decimal.Parse(reader[4].ToString());
                    stock.MinimumQuantity   = decimal.Parse(reader[5].ToString());
                }
            }
            return stock as T;
        }

        public bool UniqueNameAvailable(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = false;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_PRODUCT_STOCK_AVAILABLE");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    result = int.Parse(reader[0].ToString()) == 0;
                }
            }
            return result;
        }

        public bool UniqueNameAvailableExcept(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = false;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_PRODUCT_STOCK_AVAILABLE2");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    result = int.Parse(reader[0].ToString()) == 0;
                }
            }
            return result;
        }

        public IEnumerable<T> DashboardMinimumStockAlert(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<ProductStock>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_STOCK_MINIMUM_ALERT");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var stock             = new ProductStock();
                    stock.DepartementName = reader[0].ToString();
                    stock.ProductCode     = reader[1].ToString();
                    stock.ProductName     = reader[2].ToString();
                    stock.MinimumQuantity = decimal.Parse(reader[3].ToString());
                    stock.CurrentQuantity = decimal.Parse(reader[4].ToString());
                    result.Add(stock);
                }
            }
            return result as List<T>;
        }

        public DataSet[] GetProductStockReportData(int departementId, int productId, DateTime date, DateTime printDate)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd         = DBClass.GetStoredProcedureCommand("REPORT_PRODUCT_STOCK_HEADER");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@PrintDate", printDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dataSetResult          = new DataSet();
                adapter.Fill(dataSetResult, "FlowHeader");
                dataSetArray[0]        = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_PRODUCT_STOCK_DATA");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@ByPassProduct", productId == 0 ? 1 : 0);
                cmd.Parameters.AddWithValue("@Date", date);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "ProductStock");
                dataSetArray[1]         = dataSetResult;
            }
            return dataSetArray;
        }

        public DataSet GetProductStockForExcell(int departementId, int productId, DateTime date, DateTime printDate)
        {
            DataSet dataSetResult = new DataSet();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd         = DBClass.GetStoredProcedureCommand("REPORT_PRODUCT_STOCK_HEADER_EXCELL");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@PrintDate", printDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                var table              = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }

            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("REPORT_PRODUCT_STOCK_DATA");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@ByPassProduct", productId == 0 ? 1 : 0);
                cmd.Parameters.AddWithValue("@Date", date);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                var table              = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            return dataSetResult;
        }
    }
}
