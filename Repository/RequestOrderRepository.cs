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
    public class RequestOrderRepository<T> : RepositoryBase, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_REQUEST_ORDER") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                DBClass.AddSimpleParameter(cmd, "@CreatedBy", createdBy);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    objID = int.Parse(reader[0].ToString());
                }
                var listItems = (param as RequestOrder).Items;
                try
                {
                    foreach (var item in listItems)
                    {
                        SaveRequestedItem(objID, item);
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

        private void SaveRequestedItem(int id, RequestedItem item)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_REQUEST_ORDER_ITEM") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@RequestOrderId", id);
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

        public int SaveDeliverRequestedItem(RequestedItem item, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_REQUEST_ORDER_ITEM_OUT") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@RequestOrderId", item.RequestOrderID);
                DBClass.AddSimpleParameter(cmd, "@ProductId", item.ProductID);
                DBClass.AddSimpleParameter(cmd, "@Qty", item.Qty);
                DBClass.AddSimpleParameter(cmd, "@DeliverDate", item.DeliveredDate);
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

        public int SaveDeliveredRequestedItem(RequestedItem item, string createdBy)
        {
            int objID = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_REQUEST_ORDER_ITEM_IN") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@RequestOrderId", item.RequestOrderID);
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
                DBClass.AddSimpleParameter(cmd, "@RequestVoucher", obj.RequestVoucher);
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
                        var cmd = DBClass.GetStoredProcedureCommand("DELETE_STOCKFLOW") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@ProductId", obj.ProductID);
                        DBClass.AddSimpleParameter(cmd, "@DepartementId", obj.DepartementID);
                        DBClass.AddSimpleParameter(cmd, "@RequestVoucher", obj.RequestVoucher);
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

        public int UpdateRow(T param, string updatedBy)
        {
            var RequestOrder = param as RequestOrder;
            var result        = default(int);
            using (DBClass    = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_REQUEST_ORDER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", RequestOrder.DepartementID);
                DBClass.AddSimpleParameter(cmd, "@WarehouseId", RequestOrder.WarehouseID);
                DBClass.AddSimpleParameter(cmd, "@RequestDate", RequestOrder.RequestDate);
                DBClass.AddSimpleParameter(cmd, "@Note", RequestOrder.Note);
                DBClass.AddSimpleParameter(cmd, "@LastUpdatedBy", updatedBy);
                DBClass.AddSimpleParameter(cmd, "@RequestOrderId", RequestOrder.RequestOrderID);
                try
                {
                    DBClass.ExecuteNonQuery(cmd);
                    var listItems = (param as RequestOrder).Items;
                    foreach (var item in listItems)
                    {
                        SaveRequestedItem(RequestOrder.RequestOrderID, item);
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
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_REQUEST_ORDER") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@RequestOrderId", id);
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

        public int DeleteRequestOrderIn(int RequestOrderInId)
        {
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction) DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_REQUEST_ORDER_ITEM_IN") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@RequestOrderInId", RequestOrderInId);
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

        public int DeleteRequestOrderOut(int RequestOrderOutId)
        {
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction)DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_REQUEST_ORDER_ITEM_OUT") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@RequestOrderOutId", RequestOrderOutId);
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
            var result     = new List<RequestOrder>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_REQUEST_ORDER") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var RequestOrder             = new RequestOrder();
                    RequestOrder.RequestOrderID  = int.Parse(reader[0].ToString());
                    RequestOrder.VoucherCode     = reader[1].ToString();
                    RequestOrder.RequestDate     = DateTime.Parse(reader[2].ToString());
                    RequestOrder.WarehouseName   = reader[3].ToString();
                    RequestOrder.DepartementName = reader[4].ToString();
                    RequestOrder.Note            = reader[5].ToString();
                    result.Add(RequestOrder);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<T> GetCompletedRequestOrderRequestor(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<RequestOrder>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_COMPLETED_REQUEST_ORDER_REQUESTOR") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var RequestOrder             = new RequestOrder();
                    RequestOrder.RequestOrderID  = int.Parse(reader[0].ToString());
                    RequestOrder.VoucherCode     = reader[1].ToString();
                    RequestOrder.RequestDate     = DateTime.Parse(reader[2].ToString());
                    RequestOrder.WarehouseName   = reader[3].ToString();
                    RequestOrder.DepartementName = reader[4].ToString();
                    RequestOrder.Note            = reader[5].ToString();
                    result.Add(RequestOrder);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<T> GetRequestOrderRequestorHistory(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<RequestOrder>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_REQUEST_ORDER_REQUESTOR_HISTORY") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var RequestOrder             = new RequestOrder();
                    RequestOrder.RequestOrderID  = int.Parse(reader[0].ToString());
                    RequestOrder.VoucherCode     = reader[1].ToString();
                    RequestOrder.RequestDate     = DateTime.Parse(reader[2].ToString());
                    RequestOrder.WarehouseName   = reader[3].ToString();
                    RequestOrder.DepartementName = reader[4].ToString();
                    RequestOrder.Note            = reader[5].ToString();
                    result.Add(RequestOrder);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<T> GetCompletedRequestOrderWarehouse(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<RequestOrder>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_COMPLETED_REQUEST_ORDER_WAREHOUSE") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var RequestOrder             = new RequestOrder();
                    RequestOrder.RequestOrderID  = int.Parse(reader[0].ToString());
                    RequestOrder.VoucherCode     = reader[1].ToString();
                    RequestOrder.RequestDate     = DateTime.Parse(reader[2].ToString());
                    RequestOrder.WarehouseName   = reader[3].ToString();
                    RequestOrder.DepartementName = reader[4].ToString();
                    RequestOrder.Note            = reader[5].ToString();
                    result.Add(RequestOrder);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<T> FindWarehouseDashboardData(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<RequestOrder>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_REQUEST_ORDER_WAREHOUSE") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var RequestOrder             = new RequestOrder();
                    RequestOrder.RequestOrderID  = int.Parse(reader[0].ToString());
                    RequestOrder.VoucherCode     = reader[1].ToString();
                    RequestOrder.RequestDate     = DateTime.Parse(reader[2].ToString());
                    RequestOrder.WarehouseID     = int.Parse(reader[3].ToString());
                    RequestOrder.WarehouseName   = reader[4].ToString();
                    RequestOrder.DepartementID   = int.Parse(reader[5].ToString());
                    RequestOrder.DepartementName = reader[6].ToString();
                    RequestOrder.Note            = reader[7].ToString();
                    result.Add(RequestOrder);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<T> FindRequestorDashboardData(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<RequestOrder>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_REQUEST_ORDER_REQUESTOR") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var RequestOrder             = new RequestOrder();
                    RequestOrder.RequestOrderID  = int.Parse(reader[0].ToString());
                    RequestOrder.VoucherCode     = reader[1].ToString();
                    RequestOrder.RequestDate     = DateTime.Parse(reader[2].ToString());
                    RequestOrder.WarehouseName   = reader[3].ToString();
                    RequestOrder.DepartementID   = int.Parse(reader[4].ToString());
                    RequestOrder.DepartementName = reader[5].ToString();
                    RequestOrder.Note            = reader[6].ToString();
                    result.Add(RequestOrder);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<RequestedItem> GetRequestedItems(int RequestOrderId)
        {
            var result     = new List<RequestedItem>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_REQUEST_ORDER_ITEM") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@RequestOrderId", RequestOrderId);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var item = new RequestedItem
                                   {
                                       ProductID        = int.Parse(reader[0].ToString()),
                                       RequestOrderID   = int.Parse(reader[1].ToString()),
                                       ProductCode      = reader[2].ToString(),
                                       ProductName      = reader[3].ToString(),
                                       Qty              = decimal.Parse(reader[4].ToString()),
                                       SentVoucher      = reader[5].ToString(),
                                       DeliveredVoucher = reader[6].ToString(),
                                       UnitName         = reader[7].ToString()
                                   };
                    result.Add(item);
                }
            }
            return result;
        }

        public IEnumerable<RequestedItem> GetItemsNeedToBeSent(int RequestOrderId)
        {
            var result     = new List<RequestedItem>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_REQUEST_ORDER_NEED_TOBE_SENT") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@RequestOrderId", RequestOrderId);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var item            = new RequestedItem();
                    item.ProductID      = int.Parse(reader[0].ToString());
                    item.RequestOrderID = int.Parse(reader[1].ToString());
                    item.ProductCode    = reader[2].ToString();
                    item.ProductName    = reader[3].ToString();
                    item.Qty            = decimal.Parse(reader[4].ToString());
                    item.IsSent         = bool.Parse(reader[5].ToString());
                    item.IsDelivered    = bool.Parse(reader[6].ToString());
                    item.UnitName       = reader[7].ToString();
                    item.DepartementID  = int.Parse(reader[8].ToString());
                    result.Add(item);
                }
            }
            return result;
        }

        public IEnumerable<RequestedItem> GetItemsNeedToBeReceived(int RequestOrderId)
        {
            var result     = new List<RequestedItem>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_REQUEST_ORDER_NEED_TOBE_RECEIVED") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@RequestOrderId", RequestOrderId);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var item            = new RequestedItem();
                    item.ProductID      = int.Parse(reader[0].ToString());
                    item.RequestOrderID = int.Parse(reader[1].ToString());
                    item.ProductCode    = reader[2].ToString();
                    item.ProductName    = reader[3].ToString();
                    item.Qty            = decimal.Parse(reader[4].ToString());
                    item.IsSent         = bool.Parse(reader[5].ToString());
                    item.IsDelivered    = bool.Parse(reader[6].ToString());
                    item.UnitName       = reader[7].ToString();
                    item.Note           = reader[8].ToString();
                    item.DeliverDate    = DateTime.Parse(reader[9].ToString());
                    result.Add(item);
                }
            }
            return result;
        }

        public IEnumerable<RequestedItem> GetRequestOrderItemHistoryWarehouse(int RequestOrderId)
        {
            var result     = new List<RequestedItem>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_REQUEST_ORDER_WAREHOUSE_HISTORY") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@RequestOrderId", RequestOrderId);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var item            = new RequestedItem();
                    item.ProductID      = int.Parse(reader[0].ToString());
                    item.RequestOrderID = int.Parse(reader[1].ToString());
                    item.ProductCode    = reader[2].ToString();
                    item.ProductName    = reader[3].ToString();
                    item.Qty            = decimal.Parse(reader[4].ToString());
                    item.DeliverQty     = decimal.Parse(reader[5].ToString());
                    item.Note           = reader[9].ToString();
                    item.DeliverDate    = DateTime.Parse(reader[10].ToString());
                    result.Add(item);
                }
            }
            return result;
        }

        public IEnumerable<RequestedItem> GetRequestOrderItemHistory(int RequestOrderId)
        {
            var result     = new List<RequestedItem>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_REQUEST_ORDER_HISTORY") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@RequestOrderId", RequestOrderId);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var item            = new RequestedItem();
                    item.ProductID      = int.Parse(reader[0].ToString());
                    item.RequestOrderID = int.Parse(reader[1].ToString());
                    item.ProductCode    = reader[2].ToString();
                    item.ProductName    = reader[3].ToString();
                    item.Qty            = decimal.Parse(reader[4].ToString());
                    item.DeliverQty     = decimal.Parse(reader[5].ToString());
                    item.Note           = reader[9].ToString();
                    item.DeliverDate    = DateTime.Parse(reader[10].ToString());
                    result.Add(item);
                }
            }
            return result;
        }

        public IEnumerable<RequestedItem> GetRequestedItemIn(int RequestOrderId, int productID)
        {
            var result     = new List<RequestedItem>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_REQUEST_ORDER_ITEM_IN") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@RequestOrderId", RequestOrderId);
                DBClass.AddSimpleParameter(cmd, "@ProductId", productID);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var item             = new RequestedItem();
                    item.ItemInID        = int.Parse(reader[0].ToString());
                    item.DeliveredQty    = decimal.Parse(reader[1].ToString());
                    item.DeliveredDate   = DateTime.Parse(reader[2].ToString());
                    item.Note            = reader[3].ToString();
                    result.Add(item);
                }
            }
            return result;
        }

        public IEnumerable<RequestedItem> GetRequestedItemOut(int RequestOrderId, int productID)
        {
            var result     = new List<RequestedItem>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_REQUEST_ORDER_ITEM_OUT") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@RequestOrderId", RequestOrderId);
                DBClass.AddSimpleParameter(cmd, "@ProductId", productID);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var item         = new RequestedItem();
                    item.ItemOutID   = int.Parse(reader[0].ToString());
                    item.DeliverQty  = decimal.Parse(reader[1].ToString());
                    item.DeliverDate = DateTime.Parse(reader[2].ToString());
                    item.Note        = reader[3].ToString();
                    result.Add(item);
                }
            }
            return result;
        }

        public RequestedItem GetRequestedItemInById(int requestOrderInID)
        {
            var item       = new RequestedItem();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_REQUEST_ORDER_ITEM_IN_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@RequestOrderInId", requestOrderInID);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    item.LogObject = new StockLogObject
                                        {
                                            DepartementID  = int.Parse(reader[0].ToString()),
                                            ProductID      = int.Parse(reader[1].ToString()),
                                            RequestVoucher = reader[2].ToString()
                                        };
                }
            }
            return item;
        }

        public RequestedItem GetRequestedItemOutById(int requestOrderInID)
        {
            var item = new RequestedItem();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_REQUEST_ORDER_ITEM_OUT_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@RequestOrderOutId", requestOrderInID);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    item.LogObject = new StockLogObject
                                        {
                                            DepartementID  = int.Parse(reader[0].ToString()),
                                            ProductID      = int.Parse(reader[1].ToString()),
                                            RequestVoucher = reader[2].ToString()
                                        };
                }
            }
            return item;
        }

        public T FindbyId(int id)
        {
            var RequestOrder   = new RequestOrder();
            using (DBClass     = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_REQUEST_ORDER_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@RequestOrderId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    RequestOrder.RequestOrderID  = int.Parse(reader[0].ToString());
                    RequestOrder.VoucherCode     = reader[1].ToString();
                    RequestOrder.RequestDate     = DateTime.Parse(reader[2].ToString());
                    RequestOrder.WarehouseID     = int.Parse(reader[3].ToString());
                    RequestOrder.WarehouseName   = reader[4].ToString();
                    RequestOrder.DepartementID   = int.Parse(reader[5].ToString());
                    RequestOrder.DepartementName = reader[6].ToString();
                    RequestOrder.Note            = reader[7].ToString();
                }
            }
            return RequestOrder as T;
        }

        public DataSet[] GetReportData(int proformaId)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_GET_REQUEST_ORDER_DATA") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@RequestOrderId", proformaId);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "RequestOrder");
                dataSetArray[0] = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_GET_REQUEST_ORDER_ITEM") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@RequestOrderId", proformaId);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "RequestOrderItem");
                dataSetArray[1] = dataSetResult;
            }
            return dataSetArray;
        }

        public string GetVoucherCode(int outletId)
        {
            string ProformaCode = "REQ/" + outletId + "/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("GETREQUESTORDERCODENUMBER") as SqlCommand;
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
