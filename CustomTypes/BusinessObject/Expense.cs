using System;
using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class Expense: BaseEntityObject
    {
        private int expenseID;
        [ByPassUpdateParam]
        public string VoucherCode     { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public int DepartementId      { get; set; }
        [NullField]
        public int? CashID            { get; set; }
        [NullField]
        public int? CashBankID        { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public int Type               { get; set; }
        [NullField]
        public int? DepositSalesID    { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DepositSalesVoucher { get; set; }
        public string Description     { get; set; }
        public decimal Nominal        { get; set; }
        public string Note            { get; set; }
        public DateTime ExpenseDate   { get; set; }
        [ByPassInsertParam]
        public int ExpenseID
        {
            get { return expenseID; }
            set
            {
                if (expenseID == 0)
                {
                    expenseID = value;
                }
            }
        }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public CashLogObject CashLogObject { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public BankLogObject BankLogObject { get; set; }
    }
}
