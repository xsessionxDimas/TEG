using System;
using System.Collections.Generic;
using System.Linq;
using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class ProformaInvoice : BaseEntityObject
    {
        private int proformaID;
        [ByPassUpdateParam]
        public string VoucherCode     { get; set; }
        [ByPassUpdateParam]
        public DateTime SalesDate     { get; set; }
        [NullField]
        public int CustomerID         { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string CustomerName    { get; set; }
        [NullField]
        public int DepartementID      { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DepartementName { get; set; }
        [NullField]
        public int MarketingID        { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string MarketingName   { get; set; }
        public string Note            { get; set; }
        public decimal Discount       { get; set; }
        public decimal Shipping       { get; set; }
        public decimal GrandTotal     { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public List<Items> Items      { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted           { get; set; }
        [ByPassInsertParam]
        public int ProformaID
        {
            get { return proformaID; }
            set
            {
                if (proformaID == 0)
                {
                    proformaID = value;
                }
            }
        } 

        public void CalculateTotal()
        {
            this.GrandTotal  = default(decimal);
            this.GrandTotal += this.Shipping;
            this.GrandTotal += this.Items.Sum(item => item.SubTotal);
            this.GrandTotal -= this.Discount;
        }
    }


    public class Items
    {
        private int itemID;
        public int SalesID        { get; set; }
        public int DepartementID  { get; set; }
        public int ProductID      { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal Qty        { get; set; }
        public decimal Price      { get; set; }
        public decimal SubTotal   { get; set; }
        public string Keterangan  { get; set; }
        public int ItemID
        {
            get { return itemID; }
            set
            {
                if (itemID == 0)
                {
                    itemID = value;
                }
            }
        }
        public string UnitName    { get; set; }
        public CRUDLogObject LogObject { get; set; }
    }
}
