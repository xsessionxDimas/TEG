using System;
using System.Collections.Generic;
using System.Linq;
using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class SalesInvoice : BaseEntityObject
    {
        private int invoiceID;
        [ByPassUpdateParam]
        public string VoucherCode       { get; set; }
        [ByPassUpdateParam]
        public DateTime SalesDate       { get; set; }
        [NullField]
        public int CustomerID           { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string CustomerName      { get; set; }
        [NullField]
        public int DepartementID        { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DepartementName   { get; set; }
        [NullField]
        public int MarketingID          { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string MarketingName     { get; set; }
        public string Note              { get; set; }
        public decimal Discount         { get; set; }
        public decimal Shipping         { get; set; }
        public decimal GrandTotal       { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public List<Items> Items        { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public List<Payments> Payments  { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted             { get; set; }
        [ByPassInsertParam]
        public int InvoiceID
        {
            get { return invoiceID; }
            set
            {
                if (invoiceID == 0)
                {
                    invoiceID = value;
                }
            }
        }

        public void CalculateTotal()
        {
            this.GrandTotal = default(decimal);
            this.GrandTotal += this.Shipping;
            this.GrandTotal += this.Items.Sum(item => item.SubTotal);
            this.GrandTotal -= this.Discount;
        }
    }

    public class Payments
    {
        private int    paymentID;
        public int     DepartementID       { get; set; }
        public int     PaymentType         { get; set; } // 1 : cash, 2 : debit, 3 : edc, 4 : deposit sales 
        public string  PaymentTypeString   { get; set; }
        public int?    CashBankID          { get; set; }
        public int?    DepositSalesID      { get; set; }
        public string  AccountNumber       { get; set; }
        public string  DepositSalesVoucher { get; set; }
        public decimal Nominal             { get; set; }
        public DateTime PaymentDate        { get; set; }
        public int     PaymentID
        {
            get { return paymentID; }
            set
            {
                if (paymentID == 0)
                {
                    paymentID = value;
                }
            }
        }
        public CashLogObject CashLogObject { get; set; }
        public BankLogObject BankLogObject { get; set; }
            
    }
}
