using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class BankAccount : BaseEntityObject
    {
        private int bankAccountID;
        [ByPassUpdateParam]
        [NullField]
        public int DepartementID        { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DepartementName   { get; set; }
        [ByPassUpdateParam]
        [NullField]
        public int BankID               { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string BankName          { get; set; }
        [ByPassUpdateParam]
        public string AccountNumber     { get; set; }
        [ByPassUpdateParam]
        [UCWordsField]
        public string AccountName       { get; set; }
        [ByPassUpdateParam]
        public decimal StartingBalance  { get; set; }
        [ByPassUpdateParam]
        public decimal CurrentBalance   { get; set; }
        [ActiveField]
        public bool Active              { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted             { get; set; }
        [ByPassInsertParam]
        public int BankAccountID
        {
            get { return bankAccountID; }
            set
            {
                if (bankAccountID == 0)
                {
                    bankAccountID = value;
                }
            }
        }
    }
}
