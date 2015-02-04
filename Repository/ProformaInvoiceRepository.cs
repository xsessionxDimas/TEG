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
    public class ProformaInvoiceRepository<T> : RepositoryBase,  IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                try
                {
                    SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_PROFORMA");
                    RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                    var reader = DBClass.ExecuteReader(cmd);
                    while (reader.Read())
                    {
                        objID = int.Parse(reader[0].ToString());
                    }
                    var listItems = (param as ProformaInvoice).Items;
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
                }
                catch(Exception)
                {
                    DeleteRow(objID, "System");
                    objID = 0;
                }
            }
            return objID;
        }

        public int UpdateRow(T param, string updatedBy)
        {
            var proforma   = param as ProformaInvoice;
            var result     = default(int);
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_PROFORMA_PRICE");
                cmd.Parameters.AddWithValue("@Shipping",   proforma.Shipping);
                cmd.Parameters.AddWithValue("@Discount",   proforma.Discount);
                cmd.Parameters.AddWithValue("@GrandTotal", proforma.GrandTotal);
                cmd.Parameters.AddWithValue("@ProformaId", proforma.ProformaID);
                DBClass.ExecuteNonQuery(cmd);
                DeleteItemAndPresent(proforma.ProformaID);
                var listItems = proforma.Items;
                try
                {
                    foreach (var item in listItems)
                    {
                        if (item.SubTotal > 0)
                        {
                            SaveItem(proforma.ProformaID, item);
                        }
                        else
                        {
                            SavePresent(proforma.ProformaID, item);
                        }
                    }
                }
                catch (Exception)
                {
                    result = 1;
                }
            }
            return result;
        }

        public int DeleteRow(int id, string updatedBy)
        {
            var result = 1;
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_PROFORMA_SALES");
                        cmd.Parameters.AddWithValue("@ProformaId", id);
                        DBClass.ExecuteNonQuery(cmd, txn);
                        txn.Commit();
                    }
                    catch (Exception)
                    {
                        txn.Rollback();
                        result = 0;
                    }
                }
            }
            return result;
        }

        private void SaveItem(int id, Items item)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_PROFORMA_ITEM");
                        cmd.Parameters.AddWithValue("@ProformaId", id);
                        cmd.Parameters.AddWithValue("@ProductId", item.ProductID);
                        cmd.Parameters.AddWithValue("@Qty", item.Qty);
                        cmd.Parameters.AddWithValue("@Price", item.Price);
                        cmd.Parameters.AddWithValue("@SubTotal", item.SubTotal);
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
                        var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_PROFORMA_PRESENT");
                        cmd.Parameters.AddWithValue("@ProformaId", id);
                        cmd.Parameters.AddWithValue("@ProductId", item.ProductID);
                        cmd.Parameters.AddWithValue("@Qty", item.Qty);
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

        private void DeleteItemAndPresent(int id)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_PROFORMA_SALES_ITEM_AND_PRESENT");
                        cmd.Parameters.AddWithValue("@ProformaId", id);
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

        public IEnumerable<T> FindAll(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<ProformaInvoice>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_PROFORMA_INVOICE");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var proforma             = new ProformaInvoice();
                    proforma.ProformaID      = int.Parse(reader[0].ToString());
                    proforma.VoucherCode     = reader[1].ToString();
                    proforma.SalesDate       = DateTime.Parse(reader[2].ToString());
                    proforma.CustomerName    = reader[3].ToString();
                    proforma.DepartementName = reader[4].ToString();
                    proforma.MarketingName   = reader[5].ToString();
                    proforma.Note            = reader[6].ToString();
                    proforma.Discount        = decimal.Parse(reader[7].ToString());
                    proforma.Shipping        = decimal.Parse(reader[8].ToString());
                    proforma.GrandTotal      = decimal.Parse(reader[9].ToString());
                    result.Add(proforma);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<Items> GetProformaItems(int proformaID)
        {
            var result     = new List<Items>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_PROFORMA_ITEM_AND_PRESENT");
                cmd.Parameters.AddWithValue("@ProformaId", proformaID);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var item         = new Items();
                    item.ItemID      = int.Parse(reader[0].ToString());
                    item.ProductID   = int.Parse(reader[1].ToString());
                    item.ProductCode = reader[2].ToString();
                    item.ProductName = reader[3].ToString();
                    item.Qty         = decimal.Parse(reader[4].ToString());
                    item.Price       = decimal.Parse(reader[5].ToString());
                    item.SubTotal    = decimal.Parse(reader[6].ToString());
                    item.Keterangan  = reader[7].ToString();
                    item.UnitName    = reader[9].ToString();
                    result.Add(item);
                }
            }
            return result;
        }

        public T FindbyId(int id)
        {
            var proforma   = new ProformaInvoice();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_PROFORMA_INVOICE_BY_ID");
                cmd.Parameters.AddWithValue("@ProformaId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                   
                    proforma.ProformaID      = id;
                    proforma.VoucherCode     = reader[0].ToString();
                    proforma.SalesDate       = DateTime.Parse(reader[1].ToString());
                    proforma.CustomerName    = reader[2].ToString();
                    proforma.DepartementName = reader[3].ToString();
                    proforma.MarketingName   = reader[4].ToString();
                    proforma.Note            = reader[5].ToString();
                    proforma.Discount        = decimal.Parse(reader[6].ToString());
                    proforma.Shipping        = decimal.Parse(reader[7].ToString());
                    proforma.GrandTotal      = decimal.Parse(reader[8].ToString());
                }
            }
            return proforma as T;
        }

        public ProformaInvoice GetDetailProformaInvoice(int proformaId)
        {
            var proforma   = new ProformaInvoice();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_FULL_PROFORMA_INVOICE_BY_ID");
                cmd.Parameters.AddWithValue("@ProformaId", proformaId);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {

                    proforma.ProformaID      = proformaId;
                    proforma.VoucherCode     = reader[0].ToString();
                    proforma.SalesDate       = DateTime.Parse(reader[1].ToString());
                    proforma.CustomerID      = int.Parse(reader[2].ToString());
                    proforma.CustomerName    = reader[3].ToString();
                    proforma.DepartementID   = int.Parse(reader[4].ToString());
                    proforma.DepartementName = reader[5].ToString();
                    proforma.MarketingID     = int.Parse(reader[6].ToString());
                    proforma.MarketingName   = reader[7].ToString();
                    proforma.Note            = reader[8].ToString();
                    proforma.Discount        = decimal.Parse(reader[9].ToString());
                    proforma.Shipping        = decimal.Parse(reader[10].ToString());
                    proforma.GrandTotal      = decimal.Parse(reader[11].ToString());
                }
            }
            return proforma;
        }

        public DataSet[] GetReportData(int proformaId, string inWord)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_GET_PROFORMA_INVOICE_DATA");
                cmd.Parameters.AddWithValue("@ProformaInvoiceId", proformaId);
                cmd.Parameters.AddWithValue("@InWord", inWord);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "InvoiceReport");
                dataSetArray[0] = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_GET_PROFORMA_INVOICE_ITEM_AND_PRESENT_DATA");
                cmd.Parameters.AddWithValue("@ProformaInvoiceId", proformaId);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "InvoiceReportDetail");
                dataSetArray[1] = dataSetResult;
            }
            return dataSetArray;
        }

        public string GetNewProformaVoucherCode()
        {
            string ProformaCode = "PRO/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("GETPROFORMACODENUMBER");
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    ProformaCode += reader[0].ToString();
                }
            }
            return ProformaCode;
        }
    }
}
