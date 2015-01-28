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
    public class StockAdjustmentRepository<T> : RepositoryBase, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID = 0;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_STOCK_ADJUSTMENT");
                RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
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
                    SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_STOCK_ADJUSTMENT");
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
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction)DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_STOCK_ADJUSTMENT");
                        cmd.Parameters.AddWithValue("@StockAdjustmentId", id);
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
                cmd.Parameters.AddWithValue("@AdjustmentVoucher", obj.AdjustmentVoucher);
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
                        cmd.Parameters.AddWithValue("@AdjustmentVoucher", obj.AdjustmentVoucher);
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

        public IEnumerable<T> FindAll(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<StockAdjustment>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd  = DBClass.GetStoredProcedureCommand("APP_GET_ALL_STOCK_ADJUSTMENT");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader      = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var stockAdjustment = new StockAdjustment
                                              {
                                                  AdjustmentID   = int.Parse(reader[0].ToString()),
                                                  VoucherCode    = reader[1].ToString(),
                                                  AdjustmentDate = DateTime.Parse(reader[2].ToString()),
                                                  ProductCode    = reader[3].ToString(),
                                                  ProductName    = reader[4].ToString(),
                                                  AdjustmentType = reader[5].ToString(),
                                                  Qty            = decimal.Parse(reader[6].ToString()),
                                                  Note           = reader[7].ToString()
                                              };
                    result.Add(stockAdjustment);
                }
            }
            return result as List<T>;
        }

        public T FindbyId(int id)
        {
            var stockAdjustment = new StockAdjustment();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_STOCK_ADJUSTEMENT_BY_ID");
                cmd.Parameters.AddWithValue("@StockAdjustmentId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    stockAdjustment.AdjustmentID   = int.Parse(reader[0].ToString());
                    stockAdjustment.VoucherCode    = reader[1].ToString();
                    stockAdjustment.DepartementId  = int.Parse(reader[2].ToString());
                    stockAdjustment.ProductId      = int.Parse(reader[3].ToString());
                    stockAdjustment.AdjustmentType = reader[4].ToString();
                    stockAdjustment.Qty            = decimal.Parse(reader[5].ToString());
                    stockAdjustment.Note           = reader[6].ToString();
                    stockAdjustment.AdjustmentDate = DateTime.Parse(reader[7].ToString());
                    stockAdjustment.LogObject = new StockLogObject
                                                {
                                                    DepartementID     = stockAdjustment.DepartementId,
                                                    ProductID         = stockAdjustment.ProductId,
                                                    AdjustmentVoucher = stockAdjustment.VoucherCode
                                                };
                }
            }
            return stockAdjustment as T;
        }

        public DataSet[] GetReportData(int adjustmentId)
        {
            DataSet[] dataSetArray = new DataSet[1];
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd         = DBClass.GetStoredProcedureCommand("REPORT_GET_STOCK_ADJUSTMENT_DATA");
                cmd.Parameters.AddWithValue("@AdjustmentId", adjustmentId);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet dataSetResult  = new DataSet();
                adapter.Fill(dataSetResult, "Adjustment");
                dataSetArray[0]        = dataSetResult;
            }
            return dataSetArray;
        }

        public string GetVoucherCode()
        {
            string VoucherCode = "ADJS/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("GETSTOCKADJUSTMENTCODENUMBER");
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    VoucherCode += reader[0].ToString();
                }
            }
            return VoucherCode;
        }
    }
}
