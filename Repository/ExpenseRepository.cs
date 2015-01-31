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
    public class ExpenseRepository<T> : RepositoryBase, IRepository<T> where T : BaseEntityObject
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

        public IEnumerable<T> FindAllCashExpense(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<Expense>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_CASH_EXPENSE") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var Expense = new Expense
                                      {
                                          ExpenseID     = int.Parse(reader[0].ToString()),
                                          VoucherCode   = reader[1].ToString(),
                                          ExpenseDate   = DateTime.Parse(reader[2].ToString()),
                                          Description   = reader[3].ToString(),
                                          Nominal       = decimal.Parse(reader[4].ToString()),
                                          Note          = reader[5].ToString()
                                      };
                    result.Add(Expense);
                }
            }
            return result as List<T>;
        }

        public T FindCashExpenseById(int id)
        {
            var Expense    = new Expense();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_CASH_EXPENSE_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@ExpenseId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {

                    Expense.ExpenseID     = int.Parse(reader[0].ToString());
                    Expense.VoucherCode   = reader[1].ToString();
                    Expense.CashID        = int.Parse(reader[2].ToString());
                    Expense.DepartementId = int.Parse(reader[3].ToString());
                    Expense.Description   = reader[4].ToString();
                    Expense.Nominal       = decimal.Parse(reader[5].ToString());
                    Expense.Note          = reader[6].ToString();
                    Expense.ExpenseDate   = DateTime.Parse(reader[7].ToString());
                    Expense.CashLogObject     = new CashLogObject
                                            {
                                                DepartementID  = Expense.DepartementId,
                                                CashID         = (int)Expense.CashID,
                                                ExpenseVoucher = Expense.VoucherCode
                                            };  
                }
            }
            return Expense as T;
        }

        public int SaveCashExpenseRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_EXPENSE_BY_CASH") as SqlCommand;
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

        public int UpdateCashExpenseRow(T param, string updatedBy)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    var cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_EXPENSE_BY_CASH") as SqlCommand;
                    RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Update);
                    DBClass.AddSimpleParameter(cmd, "@LastUpdatedBy", updatedBy);
                    DBClass.ExecuteNonQuery(cmd, txn);
                    txn.Commit();
                }
            }
            return 0;
        }

        public int DeleteCashExpenseRow(int id, string updatedBy)
        {
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction)DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_EXPENSE_BY_CASH") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@ExpenseId", id);
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
                DBClass.AddSimpleParameter(cmd, "@ExpenseVoucher", logObject.ExpenseVoucher);
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
                        DBClass.AddSimpleParameter(cmd, "@ExpenseVoucher", logObject.ExpenseVoucher);
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

        public IEnumerable<T> FindAllBankExpense(List<Dictionary<string, object>> keyValueParam)
        {
            var result = new List<Expense>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_BANK_EXPENSE") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var Expense = new Expense
                                      {
                                          ExpenseID     = int.Parse(reader[0].ToString()),
                                          VoucherCode   = reader[1].ToString(),
                                          ExpenseDate   = DateTime.Parse(reader[2].ToString()),
                                          Description   = reader[3].ToString(),
                                          Nominal       = decimal.Parse(reader[4].ToString()),
                                          Note          = reader[5].ToString()
                                      };
                    result.Add(Expense);
                }
            }
            return result as List<T>;
        }

        public T FindBankExpenseById(int id)
        {
            var Expense    = new Expense();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_BANK_EXPENSE_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@ExpenseId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {

                    Expense.ExpenseID     = int.Parse(reader[0].ToString());
                    Expense.VoucherCode   = reader[1].ToString();
                    Expense.CashBankID    = int.Parse(reader[2].ToString());
                    Expense.DepartementId = int.Parse(reader[3].ToString());
                    Expense.Description   = reader[4].ToString();
                    Expense.Nominal       = decimal.Parse(reader[5].ToString());
                    Expense.Note          = reader[6].ToString();
                    Expense.ExpenseDate   = DateTime.Parse(reader[7].ToString());
                    Expense.BankLogObject = new BankLogObject
                                            {
                                                CashBankID     = (int)Expense.CashBankID,
                                                ExpenseVoucher = Expense.VoucherCode
                                            };
                }
            }
            return Expense as T;
        }

        public int SaveBankExpenseRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_EXPENSE_BY_BANK") as SqlCommand;
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

        public int UpdateBankExpenseRow(T param, string updatedBy)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    var cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_EXPENSE_BY_BANK") as SqlCommand;
                    RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Update);
                    DBClass.AddSimpleParameter(cmd, "@LastUpdatedBy", updatedBy);
                    DBClass.ExecuteNonQuery(cmd, txn);
                    txn.Commit();
                }
            }
            return 0;
        }

        public int DeleteBankExpenseRow(int id, string updatedBy)
        {
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction)DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_EXPENSE_BY_BANK") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@ExpenseId", id);
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
                DBClass.AddSimpleParameter(cmd, "@ExpenseVoucher", logObject.ExpenseVoucher);
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
                        DBClass.AddSimpleParameter(cmd, "@ExpenseVoucher", logObject.ExpenseVoucher);
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

        public string GetCashExpenseNewVoucherCode(int cashId)
        {
            string voucherCode = "CSE/" + cashId + "/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("GETCASHEXPENSECODENUMBER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", cashId);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    voucherCode += reader[0].ToString();
                }
            }
            return voucherCode;
        }

        public string GetBankExpenseNewVoucherCode(int outletId)
        {
            string voucherCode = "BKE/" + outletId + "/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("GETBANKEXPENSECODENUMBER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", outletId);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    voucherCode += reader[0].ToString();
                }
            }
            return voucherCode;
        }

        public DataSet[] GetCashExpenseReportData(int expenseId, string inWords)
        {
            DataSet[] dataSetArray = new DataSet[1];
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_GET_EXPENSE_DATA") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@ExpenseId", expenseId);
                DBClass.AddSimpleParameter(cmd, "@InWords", inWords);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet dataSetResult  = new DataSet();
                adapter.Fill(dataSetResult, "Expense");
                dataSetArray[0]        = dataSetResult;
            }
            return dataSetArray;
        }

        public DataSet[] GetCashBankExpenseReportData(int expenseId, string inWords)
        {
            DataSet[] dataSetArray = new DataSet[1];
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_GET_BANK_EXPENSE_DATA") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@ExpenseId", expenseId);
                DBClass.AddSimpleParameter(cmd, "@InWords", inWords);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet dataSetResult = new DataSet();
                adapter.Fill(dataSetResult, "Expense");
                dataSetArray[0] = dataSetResult;
            }
            return dataSetArray;
        }
    }
}
