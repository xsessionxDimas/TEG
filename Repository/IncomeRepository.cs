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
    public class IncomeRepository<T> : RepositoryBase, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            throw new NotImplementedException();
        }

        public int UpdateRow(T param, string updatedBy)
        {
            throw new NotImplementedException();
        }

        public int DeleteRow(int id, string updatedBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindAll(List<Dictionary<string, object>> keyValueParam)
        {
            throw new NotImplementedException();
        }

        public T FindbyId(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindAllCashIncome(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<Income>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_CASH_INCOME") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var income = new Income
                                     {
                                         IncomeID    = int.Parse(reader[0].ToString()),
                                         VoucherCode = reader[1].ToString(),
                                         IncomeDate  = DateTime.Parse(reader[2].ToString()),
                                         Description = reader[3].ToString(),
                                         Nominal     = decimal.Parse(reader[4].ToString()),
                                         Note        = reader[5].ToString()
                                     };
                    result.Add(income);
                }
            }
            return result as List<T>;
        }

        public T FindCashIncomeById(int id)
        {
            var income     = new Income();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_CASH_INCOME_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@IncomeId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    income.IncomeID      = int.Parse(reader[0].ToString());
                    income.VoucherCode   = reader[1].ToString();
                    income.CashID        = int.Parse(reader[2].ToString());
                    income.DepartementId = int.Parse(reader[3].ToString());
                    income.Description   = reader[4].ToString();
                    income.Nominal       = decimal.Parse(reader[5].ToString());
                    income.Note          = reader[6].ToString();
                    income.IncomeDate    = DateTime.Parse(reader[7].ToString());
                    income.CashLogObject     = new CashLogObject
                                               {
                                                   DepartementID = income.DepartementId,
                                                   CashID        = (int)income.CashID,
                                                   IncomeVoucher = income.VoucherCode
                                               };
                }
            }
            return income as T;
        }

        public int SaveCashIncomeRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_INCOME_BY_CASH") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                DBClass.AddSimpleParameter(cmd, "@CreatedBy", createdBy);
                var reader      = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    objID = int.Parse(reader[0].ToString());
                }
            }
            return objID;
        }

        public int UpdateCashIncomeRow(T param, string updatedBy)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    var cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_INCOME_BY_CASH") as SqlCommand;
                    RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Update);
                    DBClass.AddSimpleParameter(cmd, "@LastUpdatedBy", updatedBy);
                    DBClass.ExecuteNonQuery(cmd, txn);
                    txn.Commit();
                }
            }
            return 0;
        }

        public int DeleteCashIncomeRow(int id, string updatedBy)
        {
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction)DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_INCOME_BY_CASH") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@IncomeId", id);
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
            int objID = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("SAVE_NEW_CASHFLOW") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", logObject.DepartementID);
                DBClass.AddSimpleParameter(cmd, "@Description", logObject.Description);
                DBClass.AddSimpleParameter(cmd, "@IncomeVoucher", logObject.IncomeVoucher);
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
                        DBClass.AddSimpleParameter(cmd, "@IncomeVoucher", logObject.IncomeVoucher);
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

        public IEnumerable<T> FindAllBankIncome(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<Income>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_BANK_INCOME") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var income          = new Income();
                    income.IncomeID     = int.Parse(reader[0].ToString());
                    income.VoucherCode  = reader[1].ToString();
                    income.IncomeDate   = DateTime.Parse(reader[2].ToString());
                    income.Description  = reader[3].ToString();
                    income.Nominal      = decimal.Parse(reader[4].ToString());
                    income.Note         = reader[5].ToString();
                    result.Add(income);
                }
            }
            return result as List<T>;
        }

        public T FindBankIncomeById(int id)
        {
            var income     = new Income();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_BANK_INCOME_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@IncomeId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    
                    income.IncomeID      = int.Parse(reader[0].ToString());
                    income.VoucherCode   = reader[1].ToString();
                    income.CashBankID    = int.Parse(reader[2].ToString());
                    income.DepartementId = int.Parse(reader[3].ToString());
                    income.Description   = reader[4].ToString();
                    income.Nominal       = decimal.Parse(reader[5].ToString());
                    income.Note          = reader[6].ToString();
                    income.IncomeDate    = DateTime.Parse(reader[7].ToString());
                    income.BankLogObject = new BankLogObject
                                               {
                                                   CashBankID    = (int)income.CashBankID,
                                                   IncomeVoucher = income.VoucherCode,
                                                   CreatedDate   = income.IncomeDate 
                                               };
                }
            }
            return income as T;
        }

        public int SaveBankIncomeRow(T param, string createdBy)
        {
            int objID = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_INCOME_BY_BANK") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                DBClass.AddSimpleParameter(cmd, "@CreatedBy", createdBy);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    objID = int.Parse(reader[0].ToString());
                }
            }
            return objID;
        }

        public int UpdateBankIncomeRow(T param, string updatedBy)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    var cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_INCOME_BY_BANK") as SqlCommand;
                    RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Update);
                    DBClass.AddSimpleParameter(cmd, "@LastUpdatedBy", updatedBy);
                    DBClass.ExecuteNonQuery(cmd, txn);
                    txn.Commit();
                }
            }
            /* bypass compiler error need to be updated soon */
            return 0;
        }

        public int DeleteBankIncomeRow(int id, string updatedBy)
        {
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction)DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_INCOME_BY_BANK") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@IncomeId", id);
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
                DBClass.AddSimpleParameter(cmd, "@IncomeVoucher", logObject.IncomeVoucher);
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
                        DBClass.AddSimpleParameter(cmd, "@IncomeVoucher", logObject.IncomeVoucher);
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

        public string GetCashIncomeNewVoucherCode(int cashId)
        {
            string voucherCode = "CSI/" + cashId + "/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("GETCASHINCOMECODENUMBER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", cashId);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    voucherCode += reader[0].ToString();
                }
            }
            return voucherCode;
        }

        public string GetBankIncomeNewVoucherCode(int outletId)
        {
            string voucherCode = "BKI/" + outletId + "/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("GETBANKINCOMECODENUMBER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", outletId);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    voucherCode += reader[0].ToString();
                }
            }
            return voucherCode;
        }

        public DataSet[] GetCashIncomeReportData(int expenseId, string inWords)
        {
            DataSet[] dataSetArray = new DataSet[1];
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_GET_INCOME_DATA") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@IncomeId", expenseId);
                DBClass.AddSimpleParameter(cmd, "@InWords", inWords);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet dataSetResult  = new DataSet();
                adapter.Fill(dataSetResult, "Income");
                dataSetArray[0]        = dataSetResult;
            }
            return dataSetArray;
        }

        public DataSet[] GetCashBankIncomeReportData(int expenseId, string inWords)
        {
            DataSet[] dataSetArray = new DataSet[1];
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_GET_BANK_INCOME_DATA") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@IncomeId", expenseId);
                DBClass.AddSimpleParameter(cmd, "@InWords", inWords);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet dataSetResult  = new DataSet();
                adapter.Fill(dataSetResult, "Income");
                dataSetArray[0]        = dataSetResult;
            }
            return dataSetArray;
        }
    }
}
