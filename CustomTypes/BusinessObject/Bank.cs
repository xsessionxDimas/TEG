using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class Bank : BaseEntityObject
    {
        private int bankID;
        [UpperCaseField]
        public string BankName { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted    { get; set; }
        [ByPassInsertParam]
        public int BankID
        {
            get { return bankID; }
            set
            {
                if (bankID == 0)
                {
                    bankID = value;
                }
            }
        }
    }
}
