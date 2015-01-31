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
    public class SalesInvoiceRepository : RepositoryBase, IRepository<SalesInvoice>
    {
        public int SaveRow(SalesInvoice param, string createdBy)
        {
            int objID     = 0;
            using (DBClass = new MSSQLDatabase())
            {
                try
                {
                    var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_SALES") as SqlCommand;
                    RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                    DBClass.AddSimpleParameter(cmd, "@CreatedBy", createdBy);
                    var reader = DBClass.ExecuteReader(cmd);
                    while (reader.Read())
                    {
                        objID = int.Parse(reader[0].ToString());
                    }
                    var listItems    = param.Items;
                    var listPayments = param.Payments;
                    foreach (var item in listItems)
                    {
                        if (item.SubTotal > 0)
                        {
                            SaveItem(objID, item);
                        }
                        else
                        {
                            SavePresent(objID, item);
                        }
                    }
                    if (listPayments.Count < 1)
                    {
                        SaveSalesCredit(objID, param.GrandTotal, param.SalesDate);
                    }
                    else
                    {
                        foreach (var payment in listPayments)
                        {
                            SavePayment(objID, payment);
                        }
                    }
                }
                catch(Exception)
                {
                    DeleteRow(objID, "system");
                    objID = 0;
                }
                
            }
            return objID;
        }

        private void SaveItem(int id, Items item)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_SALES_ITEM") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@SalesId", id);
                        DBClass.AddSimpleParameter(cmd, "@DepartementId", item.DepartementID);
                        DBClass.AddSimpleParameter(cmd, "@ProductId", item.ProductID);
                        DBClass.AddSimpleParameter(cmd, "@Qty", item.Qty);
                        DBClass.AddSimpleParameter(cmd, "@Price", item.Price);
                        DBClass.AddSimpleParameter(cmd, "@SubTotal", item.SubTotal);
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

        private void SavePresent(int id, Items item)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_SALES_PRESENT") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@SalesId", id);
                        DBClass.AddSimpleParameter(cmd, "@DepartementId", item.DepartementID);
                        DBClass.AddSimpleParameter(cmd, "@ProductId", item.ProductID);
                        DBClass.AddSimpleParameter(cmd, "@Qty", item.Qty);
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
                var cmd = DBClass.GetStoredProcedureCommand("SAVE_NEW_STOCKFLOW") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", obj.DepartementID);
                DBClass.AddSimpleParameter(cmd, "@ProductId", obj.ProductID);
                DBClass.AddSimpleParameter(cmd, "@Description", obj.Description);
                DBClass.AddSimpleParameter(cmd, "@SalesVoucher", obj.SalesVoucher);
                DBClass.AddSimpleParameter(cmd, "@Deposit", obj.Deposit);
                DBClass.AddSimpleParameter(cmd, "@Withdraw", obj.Withdraw);
                DBClass.AddSimpleParameter(cmd, "@Note", obj.Note);
                DBClass.AddSimpleParameter(cmd, "@CreatedBy", obj.CreatedBy);
                DBClass.AddSimpleParameter(cmd, "@CreatedDate", obj.CreatedDate);
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

        public override void DeleteStockCRUDLog(CRUDLogObject logObject)
        {
            var obj        = logObject as StockLogObject;
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("DELETE_STOCKFLOW") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@ProductId", obj.ProductID);
                        DBClass.AddSimpleParameter(cmd, "@DepartementId", obj.DepartementID);
                        DBClass.AddSimpleParameter(cmd, "@SalesVoucher", obj.SalesVoucher);
                        var affectedRows = DBClass.ExecuteNonQuery(cmd, txn);
                        if(affectedRows == 0)
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

        private void SavePayment(int id, Payments payment)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_SALES_PAYMENT") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@SalesId", id);
                        DBClass.AddSimpleParameter(cmd, "@PaymentType", payment.PaymentType);
                        DBClass.AddSimpleParameter(cmd, "@Nominal", payment.Nominal);
                        DBClass.AddSimpleParameter(cmd, "@PaymentDate", payment.PaymentDate);
                        if(payment.CashBankID != null)
                            DBClass.AddSimpleParameter(cmd, "@CashBankId", payment.CashBankID);
                        if(payment.DepositSalesID != null)
                            DBClass.AddSimpleParameter(cmd, "@DepositId", payment.DepositSalesID);
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

        private void SaveSalesCredit(int id, decimal grandTotal, DateTime creditDate)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_SALES_CREDIT") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@SalesId", id);
                        DBClass.AddSimpleParameter(cmd, "@GrandTotal", grandTotal);
                        DBClass.AddSimpleParameter(cmd, "@CreditDate", creditDate);
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

        public int UpdateRow(SalesInvoice param, string updatedBy)
        {
            throw new NotImplementedException();
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
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_SALES_INVOICE") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@SalesId", id);
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

        public IEnumerable<SalesInvoice> FindAll(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<SalesInvoice>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_GET_ALL_SALES") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var salesInvoice = new SalesInvoice
                                           {
                                               InvoiceID        = int.Parse(reader[0].ToString()),
                                               VoucherCode      = reader[1].ToString(),
                                               SalesDate        = DateTime.Parse(reader[2].ToString()),
                                               CustomerName     = reader[3].ToString(),
                                               DepartementName  = reader[4].ToString(),
                                               MarketingName    = reader[5].ToString(),
                                               Note             = reader[6].ToString(),
                                               Discount         = decimal.Parse(reader[7].ToString()),
                                               Shipping         = decimal.Parse(reader[8].ToString()),
                                               GrandTotal       = decimal.Parse(reader[9].ToString())
                                           };
                    result.Add(salesInvoice);
                }
            }
            return result;
        }

        public SalesInvoice FindbyId(int id)
        {
            var salesInvoice = new SalesInvoice();
            using (DBClass   = new MSSQLDatabase())
            {
                var cmd      = DBClass.GetStoredProcedureCommand("APP_GET_SALES_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@SalesInvoiceId", id);
                var reader   = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    
                    salesInvoice.InvoiceID       = int.Parse(reader[0].ToString());
                    salesInvoice.VoucherCode     = reader[1].ToString();
                    salesInvoice.DepartementID   = int.Parse(reader[2].ToString());
                    salesInvoice.CustomerName    = reader[3].ToString();
                    salesInvoice.Items           = (List<Items>)GetSalesItems(salesInvoice.InvoiceID);
                    salesInvoice.Payments        = (List<Payments>)GetSalesPayment(salesInvoice.InvoiceID);
                }
            }
            return salesInvoice;
        }

        public IEnumerable<Items> GetSalesItems(int salesID)
        {
            var result     = new List<Items>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_GET_SALES_ITEM_AND_PRESENT") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@SalesId", salesID);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var item = new Items
                                   {
                                       SalesID      = salesID,
                                       ItemID       = int.Parse(reader[0].ToString()),
                                       ProductID    = int.Parse(reader[1].ToString()),
                                       ProductCode  = reader[2].ToString(),
                                       ProductName  = reader[3].ToString(),
                                       Qty          = decimal.Parse(reader[4].ToString()),
                                       Price        = decimal.Parse(reader[5].ToString()),
                                       SubTotal     = decimal.Parse(reader[6].ToString()),
                                       Keterangan   = reader[7].ToString(),
                                       UnitName     = reader[9].ToString(),
                                       LogObject    = new StockLogObject
                                                       {
                                                           DepartementID = int.Parse(reader[10].ToString()),
                                                           ProductID     = int.Parse(reader[1].ToString()),
                                                           SalesVoucher  = reader[11].ToString()
                                                       }
                                   };
                    result.Add(item);
                }
            }
            return result;
        }

        public IEnumerable<Payments> GetSalesPayments(int salesID)
        {
            var result     = new List<Payments>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_GET_SALES_PAYMENTS") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@SalesId", salesID);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var payment = new Payments
                                      {
                                          PaymentTypeString   = reader[0].ToString(),
                                          AccountNumber       = reader[1].ToString(),
                                          DepositSalesVoucher = reader[2].ToString(),
                                          Nominal             = decimal.Parse(reader[3].ToString()),
                                          PaymentDate         = DateTime.Parse(reader[4].ToString())
                                      };
                    result.Add(payment);
                }
            }
            return result;
        }

        public IEnumerable<Payments> GetSalesPayment(int salesID)
        {
            var result     = new List<Payments>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_GET_SALES_PAYMENT") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@SalesId", salesID);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var payment = new Payments
                    {
                        PaymentID     = int.Parse(reader[0].ToString()),
                        PaymentType   = int.Parse(reader[1].ToString()),
                        CashLogObject = new CashLogObject
                                            {
                                                DepartementID = int.Parse(reader[2].ToString()),
                                                CashID        = string.IsNullOrEmpty(reader[3].ToString()) ? 0 : int.Parse(reader[3].ToString()),
                                                SalesVoucher  = reader[5].ToString(),
                                                CreatedDate   = DateTime.Parse(reader[6].ToString())
                                            },
                        BankLogObject =  new BankLogObject
                                             {
                                                 CashBankID   = string.IsNullOrEmpty(reader[4].ToString()) ? 0 : int.Parse(reader[4].ToString()),
                                                 SalesVoucher = reader[5].ToString(),
                                                 CreatedDate  = DateTime.Parse(reader[6].ToString())
                                             }
                    };
                    result.Add(payment);
                }
            }
            return result;
        }

        public DataSet[] GetReportData(int salesId, string inWord)
        {
            var dataSetArray = new DataSet[3];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd         = DBClass.GetStoredProcedureCommand("REPORT_GET_SALES_INVOICE_DATA") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@InvoiceId", salesId);
                DBClass.AddSimpleParameter(cmd, "@InWord", inWord);
                var adapter     = new SqlDataAdapter(cmd);
                dataSetResult   = new DataSet();
                adapter.Fill(dataSetResult, "SalesInvoiceReport");
                dataSetArray[0] = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd         = DBClass.GetStoredProcedureCommand("REPORT_GET_SALES_INVOICE_ITEM_AND_PRESENT_DATA") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@InvoiceId", salesId);
                var adapter     = new SqlDataAdapter(cmd);
                dataSetResult   = new DataSet();
                adapter.Fill(dataSetResult, "SalesInvoiceReportDetail");
                dataSetArray[1] = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd         = DBClass.GetStoredProcedureCommand("REPORT_GET_SALES_INVOICE_PAYMENTS") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@InvoiceId", salesId);
                var adapter     = new SqlDataAdapter(cmd);
                dataSetResult   = new DataSet();
                adapter.Fill(dataSetResult, "SalesInvoicePayments");
                dataSetArray[2] = dataSetResult;
            }
            return dataSetArray;
        }

        public string GetNewInvoiceCode(int outletId)
        {
            string ProformaCode = "INV/" + outletId + "/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass      = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("GETINVOICECODENUMBER") as SqlCommand;
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    ProformaCode += reader[0].ToString();
                }
            }
            return ProformaCode;
        }

        public int DeleteSalesItem(int salesID, int productID)
        {
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction)DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_SALES_ITEM") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@SalesId", salesID);
                        DBClass.AddSimpleParameter(cmd, "@ProductId", productID);
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

        public int DeleteSalesItemPresent(int salesID, int productID)
        {
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction)DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_SALES_ITEM_PRESENT") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@SalesId", salesID);
                        DBClass.AddSimpleParameter(cmd, "@ProductId", productID);
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

        public int DeleteSalesPayment(int paymentID)
        {
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction)DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_SALES_PAYMENT") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@SalesPaymentId", paymentID);
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
