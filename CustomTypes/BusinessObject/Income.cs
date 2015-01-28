using System;
using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class Income : BaseEntityObject
    {
        private int incomeID;
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
        public int? SalesID           { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string SalesVoucher    { get; set; }
        public string Description     { get; set; }
        public decimal Nominal        { get; set; }
        public string Note            { get; set; }
        public DateTime IncomeDate    { get; set; }
        [ByPassInsertParam]
        public int IncomeID
        {
            get { return incomeID; }
            set
            {
                if (incomeID == 0)
                {
                    incomeID = value;
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
