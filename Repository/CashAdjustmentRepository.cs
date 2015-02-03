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
    public class CashAdjustmentRepository<T> : RepositoryBase, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_CASH_ADJUSTMENT") as SqlCommand;
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
                    var cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_CASH_ADJUSTMENT") as SqlCommand;
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
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_CASH_ADJUSTMENT") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@CashAdjustmentId", id);
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

        public override int SaveCashCRUDLog(CashLogObject logObject)
        {
            var objID = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("SAVE_NEW_CASHFLOW") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", logObject.DepartementID);
                DBClass.AddSimpleParameter(cmd, "@Description", logObject.Description);
                DBClass.AddSimpleParameter(cmd, "@AdjustmentVoucher", logObject.AdjustmentVoucher);
                DBClass.AddSimpleParameter(cmd, "@Deposit", logObject.Deposit);
                DBClass.AddSimpleParameter(cmd, "@Withdraw", logObject.Withdraw);
                DBClass.AddSimpleParameter(cmd, "@Note", logObject.Note);
                DBClass.AddSimpleParameter(cmd, "@CreatedBy", logObject.CreatedBy);
                DBClass.AddSimpleParameter(cmd, "@CreatedDate", logObject.CreatedDate);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    objID = int.Parse(reader[0].ToString());
                }
                if (objID == 0)
                    throw new Exception();
            }
            return objID;
        }

        public override void DeleteCashCRUDLog(CashLogObject logObject)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("DELETE_CASHFLOW") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@CashId", logObject.CashID);
                        DBClass.AddSimpleParameter(cmd, "@AdjustmentVoucher", logObject.AdjustmentVoucher);
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
            var result     = new List<CashAdjustment>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_CASH_ADJUSTMENT") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader      = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var cashAdjustment             = new CashAdjustment();
                    cashAdjustment.AdjustmentID    = int.Parse(reader[0].ToString());
                    cashAdjustment.VoucherCode     = reader[1].ToString();
                    cashAdjustment.AdjustmentDate  = DateTime.Parse(reader[2].ToString());
                    cashAdjustment.DepartementName = reader[3].ToString();
                    cashAdjustment.AdjustmentType  = reader[4].ToString();
                    cashAdjustment.Nominal         = decimal.Parse(reader[5].ToString());
                    cashAdjustment.Note            = reader[6].ToString();
                    result.Add(cashAdjustment);
                }
            }
            return result as List<T>;
        }

        public T FindbyId(int id)
        {
            var cashAdjustment = new CashAdjustment();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_CASH_ADJUSTEMENT_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@CashAdjustmentId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    cashAdjustment.AdjustmentID   = int.Parse(reader[0].ToString());
                    cashAdjustment.VoucherCode    = reader[1].ToString();
                    cashAdjustment.DepartementId  = int.Parse(reader[2].ToString());
                    cashAdjustment.CashId         = int.Parse(reader[3].ToString());
                    cashAdjustment.AdjustmentType = reader[4].ToString();
                    cashAdjustment.Nominal        = decimal.Parse(reader[5].ToString());
                    cashAdjustment.Note           = reader[6].ToString();
                    cashAdjustment.AdjustmentDate = DateTime.Parse(reader[7].ToString());
                    cashAdjustment.LogObject      = new CashLogObject
                                                    {
                                                        DepartementID     = cashAdjustment.DepartementId,
                                                        CashID            = cashAdjustment.CashId,
                                                        AdjustmentVoucher = cashAdjustment.VoucherCode
                                                    };
                
                }
            }
            return cashAdjustment as T;
        }

        public DataSet[] GetReportData(int adjustmentId, string inWords)
        {
            DataSet[] dataSetArray = new DataSet[1];
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_GET_CASH_ADJUSTMENT_DATA") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@AdjustmentId", adjustmentId);
                DBClass.AddSimpleParameter(cmd, "@InWords", inWords);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet dataSetResult  = new DataSet();
                adapter.Fill(dataSetResult, "Adjustment");
                dataSetArray[0]        = dataSetResult;
            }
            return dataSetArray;
        }

        public string GetVoucherCode()
        {
            string VoucherCode = "ADJC/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("GETCASHADJUSTMENTCODENUMBER") as SqlCommand;
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
