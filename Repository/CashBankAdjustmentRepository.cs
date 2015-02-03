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
    public class CashBankAdjustmentRepository<T> : RepositoryBase, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_CASH_BANK_ADJUSTMENT") as SqlCommand;
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
                    var cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_CASH_BANK_ADJUSTMENT") as SqlCommand;
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
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_CASH_BANK_ADJUSTMENT") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@CashBankAdjustmentId", id);
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

        public override int SaveBankCRUDLog(BankLogObject logObject)
        {
            int objID = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("SAVE_NEW_BANKFLOW") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@CashBankId", logObject.CashBankID);
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

        public override void DeleteBankCRUDLog(BankLogObject logObject)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("DELETE_BANKFLOW") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@CashBankId", logObject.CashBankID);
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
            var result     = new List<CashBankAdjustment>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_CASH_BANK_ADJUSTMENT") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader      = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var cashBankAdjustment             = new CashBankAdjustment();
                    cashBankAdjustment.AdjustmentID    = int.Parse(reader[0].ToString());
                    cashBankAdjustment.VoucherCode     = reader[1].ToString();
                    cashBankAdjustment.AdjustmentDate  = DateTime.Parse(reader[2].ToString());
                    cashBankAdjustment.DepartementName = reader[3].ToString();
                    cashBankAdjustment.BankAccount     = reader[4].ToString();
                    cashBankAdjustment.AdjustmentType  = reader[5].ToString();
                    cashBankAdjustment.Nominal         = decimal.Parse(reader[6].ToString());
                    cashBankAdjustment.Note            = reader[7].ToString();
                    result.Add(cashBankAdjustment);
                }
            }
            return result as List<T>;
        }

        public T FindbyId(int id)
        {
            var cashBankAdjustment = new CashBankAdjustment();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_CASH_BANK_ADJUSTEMENT_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@CashBankAdjustmentId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    cashBankAdjustment.AdjustmentID   = int.Parse(reader[0].ToString());
                    cashBankAdjustment.VoucherCode    = reader[1].ToString();
                    cashBankAdjustment.DepartementId  = int.Parse(reader[2].ToString());
                    cashBankAdjustment.CashBankId     = int.Parse(reader[3].ToString());
                    cashBankAdjustment.AdjustmentType = reader[4].ToString();
                    cashBankAdjustment.Nominal        = decimal.Parse(reader[5].ToString());
                    cashBankAdjustment.Note           = reader[6].ToString();
                    cashBankAdjustment.AdjustmentDate = DateTime.Parse(reader[7].ToString());
                    cashBankAdjustment.LogObject      = new BankLogObject
                                                       {
                                                           CashBankID        = cashBankAdjustment.CashBankId,
                                                           AdjustmentVoucher = cashBankAdjustment.VoucherCode,
                                                           CreatedDate       = cashBankAdjustment.AdjustmentDate
                                                       };
                }
            }
            return cashBankAdjustment as T;
        }

        public DataSet[] GetReportData(int adjustmentId, string inWords)
        {
            DataSet[] dataSetArray = new DataSet[1];
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_GET_BANK_ADJUSTMENT_DATA") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@AdjustmentId", adjustmentId);
                DBClass.AddSimpleParameter(cmd, "@InWords", inWords);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet dataSetResult  = new DataSet();
                adapter.Fill(dataSetResult, "Adjustment");
                dataSetArray[0]        = dataSetResult;
            }
            return dataSetArray;
        }

        /* product code generator helper */
        public string GetVoucherCode()
        {
            string VoucherCode = "ADJB/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("GETCASHBANKADJUSTMENTCODENUMBER") as SqlCommand;
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
