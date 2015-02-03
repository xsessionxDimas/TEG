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
    public class PurchaseOrderRepository<T> : RepositoryBase, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                try
                {
                    var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_PURCHASE_ORDER") as SqlCommand;
                    RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                    DBClass.AddSimpleParameter(cmd, "@CreatedBy", createdBy);
                    var reader = DBClass.ExecuteReader(cmd);
                    while (reader.Read())
                    {
                        objID = int.Parse(reader[0].ToString());
                    }
                    var listItems = (param as PurchaseOrder).Items;
                    foreach (var item in listItems)
                    {
                        SavePurchasedItem(objID, item);
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

        private void SavePurchasedItem(int id, PurchasedItem item)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_PURCHASE_ORDER_ITEM") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@PurchaseOrderId", id);
                        DBClass.AddSimpleParameter(cmd, "@ProductId", item.ProductID);
                        DBClass.AddSimpleParameter(cmd, "@Qty", item.Qty);
                        DBClass.AddSimpleParameter(cmd, "@EstimatedDate", item.EstimatedDate);
                        DBClass.AddSimpleParameter(cmd, "@DeliveredStatus", 1);
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
                DBClass.AddSimpleParameter(cmd, "@PurchaseVoucher", obj.PurchaseVoucher);
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
                        var cmd = DBClass.GetStoredProcedureCommand("DELETE_STOCKFLOW_PO") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@ProductId", obj.ProductID);
                        DBClass.AddSimpleParameter(cmd, "@DepartementId", obj.DepartementID);
                        DBClass.AddSimpleParameter(cmd, "@PurchaseVoucher", obj.PurchaseVoucher);
                        DBClass.AddSimpleParameter(cmd, "@CreatedDate", obj.CreatedDate);
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

        public int SaveDeliveredPurchasedItem(PurchasedItem item, string createdBy)
        {
            int objID = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_PURCHASE_ORDER_ITEM_IN") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@PurchaseOrderId", item.PurchaseOrderID);
                DBClass.AddSimpleParameter(cmd, "@ProductId", item.ProductID);
                DBClass.AddSimpleParameter(cmd, "@Qty", item.Qty);
                DBClass.AddSimpleParameter(cmd, "@DeliveredDate", item.DeliveredDate);
                DBClass.AddSimpleParameter(cmd, "@Note", item.Note);
                DBClass.AddSimpleParameter(cmd, "@CreatedBy", createdBy);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    objID = int.Parse(reader[0].ToString());
                }
            }
            return objID;
        }

        public int UpdateRow(T param, string updatedBy)
        {
            var purchaseOrder = param as PurchaseOrder;
            var result        = default(int);
            using (DBClass    = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_PURCHASE_ORDER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", purchaseOrder.DepartementID);
                DBClass.AddSimpleParameter(cmd, "@SupplierId", purchaseOrder.SupplierID);
                DBClass.AddSimpleParameter(cmd, "@PurchaseDate", purchaseOrder.PurchaseDate);
                DBClass.AddSimpleParameter(cmd, "@Note", purchaseOrder.Note);
                DBClass.AddSimpleParameter(cmd, "@LastUpdatedBy", updatedBy);
                DBClass.AddSimpleParameter(cmd, "@PurchaseOrderId", purchaseOrder.PurchaseOrderID);
                try
                {
                    DBClass.ExecuteNonQuery(cmd);
                    var listItems = (param as PurchaseOrder).Items;
                    foreach (var item in listItems)
                    {
                        SavePurchasedItem(purchaseOrder.PurchaseOrderID, item);
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
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction) DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_PURCHASE_ORDER") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@PurchaseOrderId", id);
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

        public int DeletePurchaseOrderIn(int purchaseOrderInId)
        {
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction) DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_PURCHASE_ORDER_ITEM_IN") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@PurchaseOrderInId", purchaseOrderInId);
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
            var result     = new List<PurchaseOrder>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_PURCHASE_ORDER") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var purchaseOrder             = new PurchaseOrder();
                    purchaseOrder.PurchaseOrderID = int.Parse(reader[0].ToString());
                    purchaseOrder.VoucherCode     = reader[1].ToString();
                    purchaseOrder.PurchaseDate    = DateTime.Parse(reader[2].ToString());
                    purchaseOrder.DepartementName = reader[3].ToString();
                    purchaseOrder.SupplierName    = reader[4].ToString();
                    purchaseOrder.Note            = reader[5].ToString();
                    result.Add(purchaseOrder);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<T> FindAllForDashboard(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<PurchaseOrder>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_PURCHASE_ORDER_DASHBOARD") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader      = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var purchaseOrder = new PurchaseOrder
                                            {
                                                PurchaseOrderID = int.Parse(reader[0].ToString()),
                                                DepartementID   = int.Parse(reader[1].ToString()),
                                                VoucherCode     = reader[2].ToString(),
                                                PurchaseDate    = DateTime.Parse(reader[3].ToString()),
                                                DepartementName = reader[4].ToString(),
                                                SupplierName    = reader[5].ToString(),
                                                Note            = reader[6].ToString()
                                            };
                    result.Add(purchaseOrder);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<PurchasedItem> GetPurchasedItems(int purchaseOrderId)
        {
            var result     = new List<PurchasedItem>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_PURCHASE_ORDER_ITEM") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@PurchaseOrderId", purchaseOrderId);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var item = new PurchasedItem
                                   {
                                       ProductID       = int.Parse(reader[0].ToString()),
                                       PurchaseOrderID = int.Parse(reader[1].ToString()),
                                       ProductCode     = reader[2].ToString(),
                                       ProductName     = reader[3].ToString(),
                                       Qty             = decimal.Parse(reader[4].ToString()),
                                       DeliveredQty    = decimal.Parse(reader[5].ToString()),
                                       EstimatedDate   = DateTime.Parse(reader[6].ToString()),
                                       UnitID          = int.Parse(reader[7].ToString()),
                                       UnitName        = reader[8].ToString(),
                                       Status          = reader[9].ToString()
                                   };
                    result.Add(item);
                }
            }
            return result;
        }

        public IEnumerable<PurchasedItem> GetPurchasedItemIn(int purchaseOrderId, int productID)
        {
            var result     = new List<PurchasedItem>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_PURCHASE_ORDER_ITEM_IN") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@PurchaseOrderId", purchaseOrderId);
                DBClass.AddSimpleParameter(cmd, "@ProductId", productID);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var item = new PurchasedItem
                                   {
                                       ItemInID      = int.Parse(reader[0].ToString()),
                                       DeliveredQty  = decimal.Parse(reader[1].ToString()),
                                       DeliveredDate = DateTime.Parse(reader[2].ToString()),
                                       Note          = reader[3].ToString()
                                   };
                    result.Add(item);
                }
            }
            return result;
        }

        public PurchasedItem GetPurchasedItemInByID(int purchaseOrderInId)
        {
            var item = new PurchasedItem();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_PURCHASE_ORDER_ITEM_IN_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@PurchaseOrderInId", purchaseOrderInId);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    item.LogObject = new StockLogObject
                                         {
                                             DepartementID   = int.Parse(reader[0].ToString()),
                                             ProductID       = int.Parse(reader[1].ToString()),
                                             PurchaseVoucher = reader[2].ToString(),
                                             CreatedDate     = DateTime.Parse(reader[3].ToString())
                                         };
                }
            }
            return item;
        }   

        public T FindbyId(int id)
        {
            var purchaseOrder  = new PurchaseOrder();
            using (DBClass     = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_PURCHASE_ORDER_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@PurchaseOrderId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    purchaseOrder.PurchaseOrderID = int.Parse(reader[0].ToString());
                    purchaseOrder.VoucherCode     = reader[1].ToString();
                    purchaseOrder.PurchaseDate    = DateTime.Parse(reader[2].ToString());
                    purchaseOrder.DepartementID   = int.Parse(reader[3].ToString());
                    purchaseOrder.SupplierID      = int.Parse(reader[4].ToString());
                    purchaseOrder.SupplierName    = reader[5].ToString();
                    purchaseOrder.Note            = reader[6].ToString();
                }
            }
            return purchaseOrder as T;
        }

        public DataSet[] GetReportData(int proformaId)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_GET_PURCHASE_ORDER_DATA") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@PurchaseOrderId", proformaId);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "PurchaseOrder");
                dataSetArray[0] = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_GET_PURCHASE_ORDER_ITEM") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@PurchaseOrderId", proformaId);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "PurchaseOrderItem");
                dataSetArray[1] = dataSetResult;
            }
            return dataSetArray;
        }

        public string GetVoucherCode(int departementId)
        {
            string ProformaCode = "PO/" + departementId + "/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("GETPURCHASEORDERCODENUMBER") as SqlCommand;
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
