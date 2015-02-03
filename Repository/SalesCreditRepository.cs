using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using CustomTypes.Base;
using CustomTypes.Objects;
using Repository.DBClass;
using Repository.abstraction;
using Repository.tools;

namespace Repository
{
    public class SalesCreditRepository<T> : RepositoryBase,  IRepository<T> where T : BaseEntityObject
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
            var result     = new List<SalesCredit>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_SALES_CREDIT") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var salesCredit = new SalesCredit
                                          {
                                              CreditID          = int.Parse(reader[0].ToString()),
                                              SalesID           = int.Parse(reader[1].ToString()),
                                              SalesVoucherCode  = reader[2].ToString(),
                                              DepartementName   = reader[3].ToString(),
                                              CustomerName      = reader[4].ToString(),
                                              CreditDate        = DateTime.Parse(reader[5].ToString()),
                                              TotalCredit       = decimal.Parse(reader[6].ToString()),
                                              TotalInstallment  = decimal.Parse(reader[7].ToString()),
                                              CreditBalance     = decimal.Parse(reader[8].ToString())
                                          };
                    result.Add(salesCredit);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<T> FindAllEDC(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<SalesCredit>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_SALES_CREDIT_EDC") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var salesCredit              = new SalesCredit();
                    salesCredit.CreditID         = int.Parse(reader[0].ToString());
                    salesCredit.SalesVoucherCode = reader[1].ToString();
                    salesCredit.DepartementName  = reader[2].ToString();
                    salesCredit.CustomerName     = reader[3].ToString();
                    salesCredit.CreditDate       = DateTime.Parse(reader[4].ToString());
                    salesCredit.TotalCredit      = decimal.Parse(reader[5].ToString());
                    salesCredit.CreditBalance    = decimal.Parse(reader[6].ToString());
                    result.Add(salesCredit);
                }
            }
            return result as List<T>;
        }

        public void SavePayment(CreditPayment payment, string createdBy)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_SALES_CREDIT_PAYMENT") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@SalesCreditId", payment.SalesCreditID);
                        DBClass.AddSimpleParameter(cmd, "@SalesId", payment.SalesId);
                        DBClass.AddSimpleParameter(cmd, "@VoucherCode", payment.VoucherCode);
                        DBClass.AddSimpleParameter(cmd, "@PaymentType", payment.CreditPaymentType);
                        DBClass.AddSimpleParameter(cmd, "@Nominal", payment.Nominal);
                        DBClass.AddSimpleParameter(cmd, "@Note", payment.Note);
                        DBClass.AddSimpleParameter(cmd, "@PaymentDate", payment.CreditPaymentDate);
                        if (payment.CashBankID != null)
                            DBClass.AddSimpleParameter(cmd, "@CashBankId", payment.CashBankID);
                        if (payment.DepositSalesID != null)
                            DBClass.AddSimpleParameter(cmd, "@DepositSalesId", payment.DepositSalesID);
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

        public override int SaveCashCRUDLog(CashLogObject logObject)
        {
            int objID = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("SAVE_NEW_CASHFLOW") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", logObject.DepartementID);
                DBClass.AddSimpleParameter(cmd, "@Description", logObject.Description);
                DBClass.AddSimpleParameter(cmd, "@SalesVoucher", logObject.SalesVoucher);
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
                        DBClass.AddSimpleParameter(cmd, "@SalesVoucher", logObject.SalesVoucher);
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
                DBClass.AddSimpleParameter(cmd, "@SalesVoucher", logObject.SalesVoucher);
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
                        DBClass.AddSimpleParameter(cmd, "@SalesVoucher", logObject.SalesVoucher);
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

        public void SaveEDCPayment(CreditPayment payment, string createdBy)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_EDC_CREDIT_PAYMENT") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@SalesCreditId", payment.SalesCreditID);
                        DBClass.AddSimpleParameter(cmd, "@CreatedBy", createdBy);
                        DBClass.AddSimpleParameter(cmd, "@PaymentDate", payment.CreditPaymentDate);
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

        public T FindbyId(int id)
        {
            throw new NotImplementedException();
        }

        public CreditPayment GetEDCPaymentObject(int EDCCreditID, string createdBy, DateTime logDate)
        {
            var result     = new CreditPayment();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_EDC_PAYMENT") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@SalesCreditId", EDCCreditID);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    result.BankLogObject = new BankLogObject
                                               {
                                                    CashBankID   = int.Parse(reader[0].ToString()),
                                                    Description  = string.Format("Pelunasan pembayaran via EDC oleh {0}", reader[3]),
                                                    Deposit      = decimal.Parse(reader[1].ToString()),
                                                    Withdraw     = 0,
                                                    SalesVoucher = reader[2].ToString(),
                                                    Note         = "",
                                                    CreatedBy    = createdBy,
                                                    CreatedDate  = logDate

                                               };
                }
            }
            return result;
        }

        public IEnumerable<CreditPayment> GetSalesCreditPayments(int salesCreditID)
        {
            var result = new List<CreditPayment>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_SALES_CREDIT_PAYMENTS") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@SalesCreditId", salesCreditID);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var payment               = new CreditPayment();
                    payment.VoucherCode       = reader[0].ToString();
                    payment.PaymentTypeString = reader[1].ToString();
                    payment.AccountNumber     = reader[2].ToString();
                    payment.VoucherCode       = reader[3].ToString();
                    payment.Nominal           = decimal.Parse(reader[4].ToString());
                    payment.CreditPaymentDate = DateTime.Parse(reader[5].ToString());
                    payment.Note              = reader[6].ToString();
                    result.Add(payment);
                }
            }
            return result;
        }

        public IEnumerable<CreditPayment> GetSalesCreditPayment(int salesID)
        {
            var result = new List<CreditPayment>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_SALES_CREDIT_PAYMENT") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@SalesId", salesID);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var payment = new CreditPayment
                                      {
                                          CreditPaymentID   = int.Parse(reader[0].ToString()),
                                          CreditPaymentType = int.Parse(reader[1].ToString()),
                                          CashLogObject     = new CashLogObject
                                                              {
                                                                  DepartementID = int.Parse(reader[2].ToString()),
                                                                  CashID        = string.IsNullOrEmpty(reader[3].ToString()) ? 0 : int.Parse(reader[3].ToString()),
                                                                  SalesVoucher  = reader[5].ToString(),
                                                                  CreatedDate   = DateTime.Parse(reader[6].ToString())
                                                              },
                                          BankLogObject     = new BankLogObject
                                                              {
                                                                  CashBankID    = string.IsNullOrEmpty(reader[4].ToString()) ? 0 : int.Parse(reader[4].ToString()),
                                                                  SalesVoucher  = reader[5].ToString(),
                                                                  CreatedDate   = DateTime.Parse(reader[6].ToString())
                                                              }
                                      };
                    result.Add(payment);
                }
            }
            return result;
        }

        public List<decimal> GetCustomerCreditLimitAndSalesCredit(int customerID)
        {
            var result     = new List<decimal>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_CHECK_CUSTOMER_SALES_CREDIT_LIMIT") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@CustomerId", customerID);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                   result.Add(decimal.Parse(reader[0].ToString()));
                   result.Add(decimal.Parse(reader[1].ToString()));
                }
            }
            return result;
        }

        public IEnumerable<T> DashboardCreditPastDueAlert(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<SalesCredit>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_SALES_CREDIT_PAST_DUE") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var salesCredit              = new SalesCredit();
                    salesCredit.CreditID         = int.Parse(reader[0].ToString());
                    salesCredit.SalesVoucherCode = reader[1].ToString();
                    salesCredit.DepartementName  = reader[2].ToString();
                    salesCredit.CustomerName     = reader[3].ToString();
                    salesCredit.CreditDate       = DateTime.Parse(reader[4].ToString());
                    salesCredit.TotalCredit      = decimal.Parse(reader[5].ToString());
                    salesCredit.TotalInstallment = decimal.Parse(reader[6].ToString());
                    salesCredit.CreditBalance    = decimal.Parse(reader[7].ToString());
                    result.Add(salesCredit);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<T> DashboardCreditPastDueAlertNotification(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<SalesCredit>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_SALES_CREDIT_PAST_DUE_NOTIFICATION") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var salesCredit              = new SalesCredit();
                    salesCredit.CreditID         = int.Parse(reader[0].ToString());
                    salesCredit.SalesVoucherCode = reader[1].ToString();
                    salesCredit.DepartementName  = reader[2].ToString();
                    salesCredit.CustomerName     = reader[3].ToString();
                    salesCredit.CreditDate       = DateTime.Parse(reader[4].ToString());
                    salesCredit.TotalCredit      = decimal.Parse(reader[5].ToString());
                    salesCredit.TotalInstallment = decimal.Parse(reader[6].ToString());
                    salesCredit.CreditBalance    = decimal.Parse(reader[7].ToString());
                    result.Add(salesCredit);
                }
            }
            return result as List<T>;
        }

        /* code generator helper */
        public string GetNewCreditPaymentCode()
        {
            string Code = "CRP/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("GETCREDITPAYMENTCODENUMBER") as SqlCommand;
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    Code += reader[0].ToString();
                }
            }
            return Code;
        }

        public int DeleteSalesCreditPayment(int creditPaymentID)
        {
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction)DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_SALES_CREDIT_PAYMENT") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@SalesCreditPaymentId", creditPaymentID);
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
