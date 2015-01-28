using System;
using CustomTypes.Attributes;

namespace CustomTypes.Objects
{
    public class CRUDLogObject
    {
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public int LogID            { get; set; }
        public string Description   { get; set; }
        public decimal Deposit      { get; set; }
        public decimal Withdraw     { get; set; }
        public decimal Balance      { get; set; }
        public string Note          { get; set; }
        public string CreatedBy     { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class StockLogObject : CRUDLogObject
    {
        public int DepartementID        { get; set; }
        public int ProductID            { get; set; }
        public int StockID              { get; set; }
        public string SalesVoucher      { get; set; }
        public string PurchaseVoucher   { get; set; }
        public string RequestVoucher    { get; set; }
        public string ProductionVoucher { get; set; }
        public string AdjustmentVoucher { get; set; }
    }

    public class CashLogObject : CRUDLogObject
    {
        public int DepartementID          { get; set; }
        public int CashID                 { get; set; }
        public string SalesVoucher        { get; set; }
        public string IncomeVoucher       { get; set; }
        public string ExpenseVoucher      { get; set; }
        public string DepositSalesVoucher { get; set; }
        public string AdjustmentVoucher   { get; set; }
    }

    public class BankLogObject : CRUDLogObject
    {
        public int DepartementID          { get; set; }
        public int CashBankID             { get; set; }
        public string SalesVoucher        { get; set; }
        public string IncomeVoucher       { get; set; }
        public string ExpenseVoucher      { get; set; }
        public string DepositSalesVoucher { get; set; }
        public string AdjustmentVoucher   { get; set; }
    }
}
