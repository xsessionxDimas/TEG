using System;
using System.Collections.Generic;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class SalesCredit : BaseEntityObject
    {
        public int CreditID             { get; set; }
        public int SalesID              { get; set; }
        public string SalesVoucherCode  { get; set; }
        public int DepartementID        { get; set; }
        public string DepartementName   { get; set; }
        public int CustomerID           { get; set; }
        public string CustomerName      { get; set; }
        public DateTime CreditDate      { get; set; }
        public decimal TotalCredit      { get; set; }
        public decimal TotalInstallment { get; set; }
        public decimal CreditBalance    { get; set; }
        public List<CreditPayment> CreditPayment { get; set; }
    }

    public class CreditPayment
    {
        private int     creditPaymentID;
        public int      SalesCreditID      { get; set; }
        public string   VoucherCode        { get; set; }
        public int      SalesId            { get; set; }
        public int      CreditPaymentType  { get; set; } // 1 : cash, 2 : debit
        public string   PaymentTypeString  { get; set; }
        public int?     CashBankID         { get; set; }
        public string   AccountNumber      { get; set; }
        public int?     DepositSalesID     { get; set; }
        public string   DepositVoucherCode { get; set; }
        public decimal  Nominal            { get; set; }
        public string   Note               { get; set; }
        public DateTime CreditPaymentDate  { get; set; }
        public int      CreditPaymentID
        {
            get { return creditPaymentID; }
            set
            {
                if (creditPaymentID == 0)
                {
                    creditPaymentID = value;
                }
            }
        }
        public int DepartementID           { get; set; }
        public CashLogObject CashLogObject { get; set; }
        public BankLogObject BankLogObject { get; set; }
    }
}
