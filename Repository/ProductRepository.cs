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
    public class ProductRepository<T> : RepositoryBase, IUniqueValidation, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_PRODUCT") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                DBClass.AddSimpleParameter(cmd, "@CreatedBy", createdBy);
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
                    var cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_PRODUCT") as SqlCommand;
                    RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Update);
                    DBClass.AddSimpleParameter(cmd, "@LastUpdatedBy", updatedBy);
                    DBClass.ExecuteNonQuery(cmd, txn);
                    txn.Commit();
                }
            }
            /* bypass compiler error need to be updated soon */
            return 0;
        }

        public int DeleteRow(int id, string updatedBy)
        {
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction)DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_PRODUCT") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@ProductId", id);
                        DBClass.AddSimpleParameter(cmd, "@LastUpdatedBy", updatedBy);
                        DBClass.ExecuteNonQuery(cmd, txn);
                        txn.Commit();
                    }
                }
            }
            catch (Exception)
            {
                result = 1;
            }
            return result;
        }

        public IEnumerable<T> FindAll(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<Product>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_PRODUCT") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var product = new Product();
                    product.ProductID    = int.Parse(reader[0].ToString());
                    product.ProductCode  = reader[1].ToString();
                    product.ProductName  = reader[2].ToString();
                    product.CategoryName = reader[3].ToString();
                    product.UnitName     = reader[4].ToString();
                    product.BronzePrice  = decimal.Parse(reader[5].ToString());
                    product.SilverPrice  = decimal.Parse(reader[6].ToString());
                    product.GoldPrice    = decimal.Parse(reader[7].ToString());
                    product.Pricelist    = bool.Parse(reader[8].ToString());
                    product.Active       = bool.Parse(reader[9].ToString());
                    result.Add(product);
                }
            }
            return result as List<T>;
        }

        public Dictionary<string, Dictionary<string, string>> GetProductWithSpesificPrice(int custStatusType)
        {
            var result     = new Dictionary<string, Dictionary<string, string>>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_PRODUCT_WITH_SPESIFIC_PRICE") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@CustomerStatusType", custStatusType);
                var reader      = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var productID  = reader[0].ToString();
                    var dictionary = new Dictionary<string, string>
                                         {
                                             {reader[1].ToString(), reader[2].ToString()}
                                         };
                    result.Add(productID, dictionary);
                }
            }
            return result;
        }

        public T FindbyId(int id)
        {
            var product    = new Product();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_PRODUCT_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@ProductId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    product.ProductID    = int.Parse(reader[0].ToString());
                    product.ProductCode  = reader[1].ToString();
                    product.ProductName  = reader[2].ToString();
                    product.CategoryID   = int.Parse(reader[3].ToString());
                    product.CategoryName = reader[4].ToString();
                    product.UnitID       = int.Parse(reader[5].ToString());
                    product.UnitName     = reader[6].ToString();
                    product.BronzePrice  = decimal.Parse(reader[7].ToString());
                    product.SilverPrice  = decimal.Parse(reader[8].ToString());
                    product.GoldPrice    = decimal.Parse(reader[9].ToString());
                    product.Pricelist    = bool.Parse(reader[10].ToString());
                    product.Active       = bool.Parse(reader[11].ToString());
                }
            }
            return product as T;
        }

        public bool UniqueNameAvailable(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = false;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_PRODUCT_NAME_AVAILABLE") as SqlCommand;
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
                var cmd = DBClass.GetStoredProcedureCommand("APP_PRODUCT_NAME_AVAILABLE2") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    result = int.Parse(reader[0].ToString()) == 0;
                }
            }
            return result;
        }

        public string GetNewProductCode(int categoryID)
        {
            string productCode = "";
            switch (categoryID)
            {
                case 1 :
                    productCode = "KM-";
                    break;
                case 2 :
                    productCode = "AL-";
                    break;
                case 3 :
                    productCode = "LN-";
                    break;
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("GETPRODUCTNEWCODENUMBER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@CategoryId", categoryID);
                var reader      = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    productCode += reader[0].ToString();
                }
            }
            return productCode;
        }

        public DataSet[] GetProductPriceListReportData(DateTime printDate)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_PRICELIST_HEADER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@PrintDate", printDate);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "FlowHeader");
                dataSetArray[0]         = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_GET_PRODUCT_PRICELIST") as SqlCommand;
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "PriceList");
                dataSetArray[1]         = dataSetResult;
            }
            return dataSetArray;
        }

        public DataSet GetProductPriceListForExcell()
        {
            DataSet dataSetResult = new DataSet();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_GET_PRODUCT_PRICELIST") as SqlCommand;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                var table = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            return dataSetResult;
        }
    }
}
