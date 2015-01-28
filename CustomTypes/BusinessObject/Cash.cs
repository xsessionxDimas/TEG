using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class Cash : BaseEntityObject
    {
        private int cashID;
        [NullField]
        public int DepartementID       { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DepartementName  { get; set; }
        public decimal StartingBalance { get; set; }
        public decimal CurrentBalance  { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted { get; set; }
        [ByPassInsertParam]
        public int CashID
        {
            get { return cashID; }
            set
            {
                if (cashID == 0)
                {
                    cashID = value;
                }
            }
        }
    }
}
