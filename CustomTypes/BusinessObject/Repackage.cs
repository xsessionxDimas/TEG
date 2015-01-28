using System;
using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class Repackage: BaseEntityObject
    {
        private int repackageID;
        [ByPassUpdateParam]
        public string VoucherCode     { get; set; }
        public int DepartementId      { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DepartementName { get; set; }
        public int SourceProduct      { get; set; }
        public decimal SourceQty      { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string SourceName      { get; set; }
        public int DestinationProduct { get; set; }
        public decimal DestinationQty { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DestinationName { get; set; }
        public string Note            { get; set; }
        public DateTime RepackageDate { get; set; }
        [ByPassInsertParam]
        public int RepackageID
        {
            get { return repackageID; }
            set
            {
                if (repackageID == 0)
                {
                    repackageID = value;
                }
            }
        }
    }
}
