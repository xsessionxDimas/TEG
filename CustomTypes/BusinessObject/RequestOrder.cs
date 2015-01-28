using System;
using System.Collections.Generic;
using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class RequestOrder : BaseEntityObject
    {
        private int requestOrderID;
        [ByPassUpdateParam]
        public string VoucherCode        { get; set; }
        [ByPassUpdateParam]
        public DateTime RequestDate      { get; set; }
        public int WarehouseID           { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string WarehouseName      { get; set; }
        public int DepartementID         { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DepartementName    { get; set; }
        public string Note               { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public List<RequestedItem> Items { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted              { get; set; }
        [ByPassInsertParam]
        public int RequestOrderID
        {
            get { return requestOrderID; }
            set
            {
                if (requestOrderID == 0)
                {
                    requestOrderID = value;
                }
            }
        }
    }

    public class RequestedItem : Items
    {
        public int ItemInID            { get; set; }
        public int ItemOutID           { get; set; }
        public int RequestOrderID      { get; set; }
        public decimal DeliverQty      { get; set; }
        public DateTime DeliverDate    { get; set; }
        public decimal  DeliveredQty   { get; set; }
        public DateTime DeliveredDate  { get; set; }
        public String SentVoucher      { get; set; }
        public String DeliveredVoucher { get; set; }
        public bool IsSent             { get; set; }
        public bool IsDelivered        { get; set; }
        public string Note             { get; set; }
        public string RequestVoucher   { get; set; }
    }
}
