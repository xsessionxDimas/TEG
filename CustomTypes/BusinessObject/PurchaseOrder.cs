using System;
using System.Collections.Generic;
using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class PurchaseOrder : BaseEntityObject
    {
        private int purchaseOrderID;
        [ByPassUpdateParam]
        public string VoucherCode        { get; set; }
        [ByPassUpdateParam]
        public DateTime PurchaseDate     { get; set; }
        public int SupplierID            { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string SupplierName       { get; set; }
        public int DepartementID         { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DepartementName    { get; set; }
        public string Note               { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public List<PurchasedItem> Items { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted              { get; set; }
        [ByPassInsertParam]
        public int PurchaseOrderID
        {
            get { return purchaseOrderID; }
            set
            {
                if (purchaseOrderID == 0)
                {
                    purchaseOrderID = value;
                }
            }
        }
    }

    public class PurchasedItem : Items
    {
        public int ItemInID            { get; set; }
        public int PurchaseOrderID     { get; set; }
        public int UnitID              { get; set; }
        public decimal DeliveredQty    { get; set; }
        public DateTime DeliveredDate  { get; set; }
        public DateTime EstimatedDate  { get; set; }
        public string Status           { get; set; }
        public string Note             { get; set; }
        public string PurchaseVoucher  { get; set; }
    }
}
