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
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_SALES_CREDIT");
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
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_SALES_CREDIT_EDC");
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
                        var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_SALES_CREDIT_PAYMENT");
                        cmd.Parameters.AddWithValue("@SalesCreditId", payment.SalesCreditID);
                        cmd.Parameters.AddWithValue("@SalesId", payment.SalesId);
                        cmd.Parameters.AddWithValue("@VoucherCode", payment.VoucherCode);
                        cmd.Parameters.AddWithValue("@PaymentType", payment.CreditPaymentType);
                        cmd.Parameters.AddWithValue("@Nominal", payment.Nominal);
                        cmd.Parameters.AddWithValue("@Note", payment.Note);
                        cmd.Parameters.AddWithValue("@PaymentDate", payment.CreditPaymentDate);
                        if (payment.CashBankID != null)
                            cmd.Parameters.AddWithValue("@CashBankId", payment.CashBankID);
                        if (payment.DepositSalesID != null)
                            cmd.Parameters.AddWithValue("@DepositSalesId", payment.DepositSalesID);
                        cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
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
                var cmd = DBClass.GetStoredProcedureCommand("SAVE_NEW_CASHFLOW");
                cmd.Parameters.AddWithValue("@DepartementId", logObject.DepartementID);
                cmd.Parameters.AddWithValue("@Description", logObject.Description);
                cmd.Parameters.AddWithValue("@SalesVoucher", logObject.SalesVoucher);
                cmd.Parameters.AddWithValue("@Deposit", logObject.Deposit);
                cmd.Parameters.AddWithValue("@Withdraw", logObject.Withdraw);
                cmd.Parameters.AddWithValue("@Note", logObject.Note);
                cmd.Parameters.AddWithValue("@CreatedBy", logObject.CreatedBy);
                cmd.Parameters.AddWithValue("@CreatedDate", logObject.CreatedDate);
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
                        var cmd = DBClass.GetStoredProcedureCommand("DELETE_CASHFLOW");
                        cmd.Parameters.AddWithValue("@CashId", logObject.CashID);
                        cmd.Parameters.AddWithValue("@SalesVoucher", logObject.SalesVoucher);
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
                var cmd = DBClass.GetStoredProcedureCommand("SAVE_NEW_BANKFLOW");
                cmd.Parameters.AddWithValue("@CashBankId", logObject.CashBankID);
                cmd.Parameters.AddWithValue("@Description", logObject.Description);
                cmd.Parameters.AddWithValue("@SalesVoucher", logObject.SalesVoucher);
                cmd.Parameters.AddWithValue("@Deposit", logObject.Deposit);
                cmd.Parameters.AddWithValue("@Withdraw", logObject.Withdraw);
                cmd.Parameters.AddWithValue("@Note", logObject.Note);
                cmd.Parameters.AddWithValue("@CreatedBy", logObject.CreatedBy);
                cmd.Parameters.AddWithValue("@CreatedDate", logObject.CreatedDate);
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
                        var cmd = DBClass.GetStoredProcedureCommand("DELETE_BANKFLOW");
                        cmd.Parameters.AddWithValue("@CashBankId", logObject.CashBankID);
                        cmd.Parameters.AddWithValue("@SalesVoucher", logObject.SalesVoucher);
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
                        var cmd = DBClass.GetStoredProcedureCommand("APP_EDC_CREDIT_PAYMENT");
                        cmd.Parameters.AddWithValue("@SalesCreditId", payment.SalesCreditID);
                        cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                        cmd.Parameters.AddWithValue("@PaymentDate", payment.CreditPaymentDate);
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
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_EDC_PAYMENT");
                cmd.Parameters.AddWithValue("@SalesCreditId", EDCCreditID);
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
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_SALES_CREDIT_PAYMENTS");
                cmd.Parameters.AddWithValue("@SalesCreditId", salesCreditID);
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
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_SALES_CREDIT_PAYMENT");
                cmd.Parameters.AddWithValue("@SalesId", salesID);
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
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_CHECK_CUSTOMER_SALES_CREDIT_LIMIT");
                cmd.Parameters.AddWithValue("@CustomerId", customerID);
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
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_SALES_CREDIT_PAST_DUE");
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
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_SALES_CREDIT_PAST_DUE_NOTIFICATION");
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
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("GETCREDITPAYMENTCODENUMBER");
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
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_SALES_CREDIT_PAYMENT");
                        cmd.Parameters.AddWithValue("@SalesCreditPaymentId", creditPaymentID);
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
