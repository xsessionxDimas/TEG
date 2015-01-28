﻿using System;
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
    public class ProductionRepository<T> : RepositoryBase, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_PRODUCTION");
                RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    objID = int.Parse(reader[0].ToString());
                }
                var composites = (param as Production).Composites;
                var results    = (param as Production).Results;
                try
                {
                    foreach (var composite in composites)
                    {
                        SaveComposite(objID, composite);
                        SaveStockCRUDLog(composite.LogObject);
                    }
                    foreach (var productionResult in results)
                    {
                        SaveResult(objID, productionResult);
                        SaveStockCRUDLog(productionResult.LogObject);
                    }
                }
                catch (Exception)
                {
                    DeleteRow(objID, "System");
                    objID = 0;
                }
            }
            return objID;
        }

        private void SaveComposite(int id, Composite composite)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_PRODUCTION_MATERIAL");
                        cmd.Parameters.AddWithValue("@ProductionId", id);
                        cmd.Parameters.AddWithValue("@ProductId", composite.ProductID);
                        cmd.Parameters.AddWithValue("@Qty", composite.Qty);
                        cmd.Parameters.AddWithValue("@Type", composite.CompositeType);
                        DBClass.ExecuteNonQuery(cmd, txn);
                        txn.Commit();
                    }
                    catch (Exception)
                    {
                        txn.Rollback();
                        throw;
                    }
                }
            }
        }

        private void SaveResult(int id, ProductionResult result)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_PRODUCTION_OUTPUT");
                        cmd.Parameters.AddWithValue("@ProductionId", id);
                        cmd.Parameters.AddWithValue("@ProductId", result.ProductID);
                        cmd.Parameters.AddWithValue("@Qty", result.Qty);
                        DBClass.ExecuteNonQuery(cmd, txn);
                        txn.Commit();
                    }
                    catch (Exception)
                    {
                        txn.Rollback();
                        throw;
                    }
                }
            }
        }

        public override int SaveStockCRUDLog(CRUDLogObject logObject)
        {
            var obj   = logObject as StockLogObject;
            int objID = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("SAVE_NEW_STOCKFLOW");
                cmd.Parameters.AddWithValue("@DepartementId", obj.DepartementID);
                cmd.Parameters.AddWithValue("@ProductId", obj.ProductID);
                cmd.Parameters.AddWithValue("@Description", obj.Description);
                cmd.Parameters.AddWithValue("@ProductionVoucher", obj.ProductionVoucher);
                cmd.Parameters.AddWithValue("@Deposit", obj.Deposit);
                cmd.Parameters.AddWithValue("@Withdraw", obj.Withdraw);
                cmd.Parameters.AddWithValue("@Note", obj.Note);
                cmd.Parameters.AddWithValue("@CreatedBy", obj.CreatedBy);
                cmd.Parameters.AddWithValue("@CreatedDate", obj.CreatedDate);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    objID = int.Parse(reader[0].ToString());
                }
                if(objID == 0) 
                    throw new Exception();
            }
            return objID;
        }

        public override void DeleteStockCRUDLog(CRUDLogObject logObject)
        {
            var obj = logObject as StockLogObject;
            using (DBClass = new MSSQLDatabase())
            {
                using (var txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("DELETE_STOCKFLOW");
                        cmd.Parameters.AddWithValue("@ProductId", obj.ProductID);
                        cmd.Parameters.AddWithValue("@DepartementId", obj.DepartementID);
                        cmd.Parameters.AddWithValue("@ProductionVoucher", obj.ProductionVoucher);
                        var affectedRows = DBClass.ExecuteNonQuery(cmd, txn);
                        if (affectedRows == 0)
                            throw new Exception("Hapus log gagal");
                        txn.Commit();
                    }
                    catch (Exception)
                    {
                        txn.Rollback();
                        throw;
                    }
                }
            }
        }

        public int UpdateRow(T param, string updatedBy)
        {
            var production = param as Production;
            var errorFlag  = default(int);
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_PRODUCTION");
                cmd.Parameters.AddWithValue("@VoucherCode", production.VoucherCode);
                cmd.Parameters.AddWithValue("@DepartementId", production.DepartementID);
                cmd.Parameters.AddWithValue("@Note", production.Note);
                cmd.Parameters.AddWithValue("@LastUpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("@ProductionDate", production.ProductionDate);
                cmd.Parameters.AddWithValue("@ProductionId", production.ProductionID);
                try
                {
                    DBClass.ExecuteNonQuery(cmd);
                    var composites = (param as Production).Composites;
                    var results    = (param as Production).Results;
                    foreach (var composite in composites)
                    {
                        SaveComposite(production.ProductionID, composite);
                        SaveStockCRUDLog(composite.LogObject);
                    }
                    foreach (var productionResult in results)
                    {
                        SaveResult(production.ProductionID, productionResult);
                        SaveStockCRUDLog(productionResult.LogObject);
                    }
                }
                catch (Exception)
                {
                    errorFlag = 1;
                }
            }
            return errorFlag;
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
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_PRODUCTION");
                        cmd.Parameters.AddWithValue("@ProductionId", id);
                        cmd.Parameters.AddWithValue("@LastUpdatedBy", updatedBy);
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
            var result     = new List<Production>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd  = DBClass.GetStoredProcedureCommand("APP_GET_ALL_PRODUCTION");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader      = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var production             = new Production();
                    production.ProductionID    = int.Parse(reader[0].ToString());
                    production.VoucherCode     = reader[1].ToString();
                    production.ProductionDate  = DateTime.Parse(reader[2].ToString());
                    production.DepartementName = reader[3].ToString();
                    production.Note            = reader[4].ToString();
                    result.Add(production);
                }
            }
            return result as List<T>;
        }

        public IList<Composite> GetProductionComposites(int productionID)
        {
            var result     = new List<Composite>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_PRODUCTION_COMPOSITES");
                cmd.Parameters.AddWithValue("@ProductionId", productionID);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var item = new Composite
                                   {
                                       ProductionID  = productionID,
                                       ProductID     = int.Parse(reader[0].ToString()),
                                       ProductCode   = reader[1].ToString(),
                                       ProductName   = reader[2].ToString(),
                                       Qty           = decimal.Parse(reader[3].ToString()),
                                       CompositeType = int.Parse(reader[4].ToString()),
                                       UnitName      = reader[5].ToString(),
                                       LogObject     = new StockLogObject
                                                       {
                                                           DepartementID     = int.Parse(reader[6].ToString()),
                                                           ProductID         = int.Parse(reader[0].ToString()),
                                                           ProductionVoucher = reader[7].ToString()
                                                       }
                                   };
                    result.Add(item);
                }
            }
            return result;
        }

        public IList<ProductionResult> GetProductionResults(int productionID)
        {
            var result     = new List<ProductionResult>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_PRODUCTION_OUTPUTS");
                cmd.Parameters.AddWithValue("@ProductionId", productionID);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var item = new ProductionResult
                                   {
                                       ProductionID = productionID,
                                       ProductID    = int.Parse(reader[0].ToString()),
                                       ProductCode  = reader[1].ToString(),
                                       ProductName  = reader[2].ToString(),
                                       Qty          = decimal.Parse(reader[3].ToString()),
                                       UnitName     = reader[4].ToString(),
                                       LogObject    = new StockLogObject
                                                       {
                                                           DepartementID     = int.Parse(reader[5].ToString()),
                                                           ProductID         = int.Parse(reader[0].ToString()),
                                                           ProductionVoucher = reader[6].ToString()
                                                       }

                                   };
                    result.Add(item);
                }
            }
            return result;
        }

        public T FindbyId(int id)
        {
            var production  = new Production();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_PRODUCTION_BY_ID");
                cmd.Parameters.AddWithValue("@ProductionId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    production.ProductionID       = int.Parse(reader[0].ToString());
                    production.VoucherCode        = reader[1].ToString();
                    production.DepartementID      = int.Parse(reader[2].ToString());
                    production.Note               = reader[3].ToString();
                    production.ProductionDate     = DateTime.Parse(reader[4].ToString());
                }
            }
            return production as T;
        }

        public DataSet[] GetReportData(int productionID)
        {
            DataSet[] dataSetArray = new DataSet[3];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_GET_PRODUCTION_DATA");
                cmd.Parameters.AddWithValue("@ProductionId", productionID);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "Production");
                dataSetArray[0] = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_GET_PRODUCTION_MATERIAL");
                cmd.Parameters.AddWithValue("@ProductionId", productionID);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "ProductionComposite");
                dataSetArray[1] = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_GET_PRODUCTION_OUTPUT");
                cmd.Parameters.AddWithValue("@ProductionId", productionID);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "ProductionResult");
                dataSetArray[2]         = dataSetResult;
            }
            return dataSetArray;
        }

        public string GetNewVoucherCode(int warehouseId)
        {
            string voucherCode = "JBC/" + warehouseId + "/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("GETPRODUCTIONCODENUMBER");
                cmd.Parameters.AddWithValue("@DepartementId", warehouseId);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    voucherCode += reader[0].ToString();
                }
            }
            return voucherCode;
        }

        public int DeleteProductionCompositeItem(int productionID, int productID)
        {
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction)DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_PRODUCTION_COMPOSITE_ITEM");
                        cmd.Parameters.AddWithValue("@ProductionId", productionID);
                        cmd.Parameters.AddWithValue("@ProductId", productID);
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

        public int DeleteProductionOutputItem(int productionID, int productID)
        {
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction)DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_PRODUCTION_OUTPUT_ITEM");
                        cmd.Parameters.AddWithValue("@ProductionId", productionID);
                        cmd.Parameters.AddWithValue("@ProductId", productID);
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
    }
}
