using System;
using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class DepositSales : BaseEntityObject
    {
        private int depositAccountID;
        [ByPassUpdateParam]
        public string VoucherCode        { get; set; }
        public int DepartementID         { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string  DepartementName   { get; set; }
        public int CashBankId            { get; set; }
        public string  AccountByName     { get; set; }
        [ByPassUpdateParam]
        public decimal StartingBalance   { get; set; }
        [ByPassUpdateParam]
        public decimal Balance           { get; set; }
        public string Note               { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted              { get; set; }
        [ByPassUpdateParam]
        public DateTime CreatedDate      { get; set; }
        [ByPassInsertParam]
        public int DepositAccountID
        {
            get { return depositAccountID; }
            set
            {
                if (depositAccountID == 0)
                {
                    depositAccountID = value;
                }
            }
        }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public BankLogObject CashLogObject { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public BankLogObject BankLogObject { get; set; }
    }
}
