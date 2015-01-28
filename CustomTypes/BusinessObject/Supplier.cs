using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class Supplier : BaseEntityObject
    {
        private int supplierID;

        [UCWordsField]
        public string SupplierName  { get; set; }
        public string Address       { get; set; }
        public string Phone         { get; set; }
        public string MobilePhone   { get; set; }
        [ActiveField]
        public bool Active          { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted         { get; set; }
        [ByPassInsertParam]
        public int SupplierID
        {
            get { return supplierID; }
            set
            {
                if (supplierID == 0)
                {
                    supplierID = value;
                }
            }
        }
    }
}
