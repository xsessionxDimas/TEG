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
    public class DepositSalesRepository<T> : RepositoryBase, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_DEPOSIT_SALES_ACCOUNT") as SqlCommand;
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
                    var cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_DEPOSIT_SALES") as SqlCommand;
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
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_DEPOSIT_SALES") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@DepositSalesId", id);
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
            var result     = new List<DepositSales>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_DEPOSIT_SALES") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader      = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var depositSales                = new DepositSales();
                    depositSales.DepositAccountID   = int.Parse(reader[0].ToString());
                    depositSales.VoucherCode        = reader[1].ToString();
                    depositSales.DepartementName    = reader[2].ToString();
                    depositSales.AccountByName      = reader[3].ToString();
                    depositSales.StartingBalance    = decimal.Parse(reader[4].ToString());
                    depositSales.Balance            = decimal.Parse(reader[5].ToString());
                    depositSales.Note               = reader[6].ToString();
                    depositSales.CreatedDate        = DateTime.Parse(reader[7].ToString());
                    result.Add(depositSales);
                }
            }
            return result as List<T>;
        }

        public T FindbyId(int id)
        {
            var depositSales = new DepositSales();
            using (DBClass   = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_DEPOSIT_SALES_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepositSalesId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    depositSales.DepositAccountID = int.Parse(reader[0].ToString());
                    depositSales.VoucherCode      = reader[1].ToString();
                    depositSales.DepartementID    = int.Parse(reader[2].ToString());
                    depositSales.CashBankId       = int.Parse(reader[3].ToString());
                    depositSales.AccountByName    = reader[4].ToString();
                    depositSales.StartingBalance  = decimal.Parse(reader[5].ToString());
                    depositSales.Balance          = decimal.Parse(reader[6].ToString());
                    depositSales.Note             = reader[7].ToString();
                    depositSales.BankLogObject    = new BankLogObject
                                                    {
                                                        CashBankID          = depositSales.CashBankId,
                                                        DepositSalesVoucher = depositSales.VoucherCode
                                                    };

                    
                }
            }
            return depositSales as T;
        }

        public T FindbyVoucherCode(string voucherCode, decimal nominalToPaid)
        {
            var depositSales = new DepositSales();
            using (DBClass   = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_DEPOSIT_SALES_BY_VOUCHER_CODE") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@VoucherCode", voucherCode);
                DBClass.AddSimpleParameter(cmd, "@NominalToPaid", nominalToPaid);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    depositSales.DepositAccountID = int.Parse(reader[0].ToString());
                    depositSales.VoucherCode      = reader[1].ToString();
                    depositSales.DepartementID    = int.Parse(reader[2].ToString());
                    depositSales.CashBankId       = int.Parse(reader[3].ToString());
                    depositSales.AccountByName    = reader[4].ToString();
                    depositSales.StartingBalance  = decimal.Parse(reader[5].ToString());
                    depositSales.Balance          = decimal.Parse(reader[6].ToString());
                    depositSales.Note             = reader[7].ToString();
                    
                }
            }
            return depositSales as T;
        }

        public override int SaveCashCRUDLog(CashLogObject logObject)
        {
            int objID = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("SAVE_NEW_CASHFLOW") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", logObject.DepartementID);
                DBClass.AddSimpleParameter(cmd, "@Description", logObject.Description);
                DBClass.AddSimpleParameter(cmd, "@DepositSalesVoucher", logObject.DepositSalesVoucher);
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
                        DBClass.AddSimpleParameter(cmd, "@DepositSalesVoucher", logObject.DepositSalesVoucher);
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

        public override int SaveBankCRUDLog(BankLogObject logObject)
        {
            int objID = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("SAVE_NEW_BANKFLOW") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@CashBankId", logObject.CashBankID);
                DBClass.AddSimpleParameter(cmd, "@Description", logObject.Description);
                DBClass.AddSimpleParameter(cmd, "@DepositSalesVoucher", logObject.DepositSalesVoucher);
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
                        DBClass.AddSimpleParameter(cmd, "@DepositSalesVoucher", logObject.DepositSalesVoucher);
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

        public void PaidWithDepositSales(List<Dictionary<string, object>> keyValueParam)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("PAID_WITH_DEPOSIT_SALES") as SqlCommand;
                        RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                        DBClass.ExecuteNonQuery(cmd, txn);
                        txn.Commit();
                    }
                    catch(Exception)
                    {
                        
                    }
                    
                }
            }
        }

        public void RefundDepositSales(int depositSalesId, int type, int departementId, decimal nominal, string note, DateTime refundDate, string createdBy, int? bankAccountId = null)
        {
           try
            {
                if (type == 1)
                    RefundByCash(depositSalesId, departementId, nominal, note, refundDate, createdBy);
                else 
                    // ReSharper disable PossibleInvalidOperationException
                    RefundByBankAccount(depositSalesId, (int)bankAccountId, nominal, note, refundDate, createdBy);
                    // ReSharper restore PossibleInvalidOperationException
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        private void RefundByCash(int depositSalesId, int departementId, decimal nominal, string note, DateTime refundDate, string createdBy)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_REFUND_DEPOSIT_SALES_BY_CASH") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                        DBClass.AddSimpleParameter(cmd, "@DepositSalesId", depositSalesId);
                        DBClass.AddSimpleParameter(cmd, "@Nominal", nominal);
                        DBClass.AddSimpleParameter(cmd, "@Note", note);
                        DBClass.AddSimpleParameter(cmd, "@RefundDate", refundDate);
                        DBClass.AddSimpleParameter(cmd, "@CreatedBy", createdBy);
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

        private void RefundByBankAccount(int depositSalesId, int bankAccountId, decimal nominal, string note, DateTime refundDate, string createdBy)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_REFUND_DEPOSIT_SALES_BY_BANK") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@BankAccountId", bankAccountId);
                        DBClass.AddSimpleParameter(cmd, "@DepositSalesId", depositSalesId);
                        DBClass.AddSimpleParameter(cmd, "@Nominal", nominal);
                        DBClass.AddSimpleParameter(cmd, "@Note", note);
                        DBClass.AddSimpleParameter(cmd, "@RefundDate", refundDate);
                        DBClass.AddSimpleParameter(cmd, "@CreatedBy", createdBy);
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

        public DataSet[] GetReportData(int depositSalesId, string inWords)
        {
            DataSet[] dataSetArray = new DataSet[1];
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_GET_DEPOSIT_SALES_DATA") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepositSalesId", depositSalesId);
                DBClass.AddSimpleParameter(cmd, "@InWords", inWords);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet dataSetResult  = new DataSet();
                adapter.Fill(dataSetResult, "DepositSales");
                dataSetArray[0]        = dataSetResult;
            }
            return dataSetArray;
        }
        
        /* product code generator helper */
        public string GetVoucherCode(int outletId)
        {
            string VoucherCode = "DPS/" + outletId + "/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass     = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("GETDEPOSITSALESCODENUMBER") as SqlCommand;
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    VoucherCode += reader[0].ToString();
                }
            }
            return VoucherCode;
        }

    }
}
