using System;
using System.Collections.Generic;
using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class Packaging : BaseEntityObject
    {
        private int packagingID;
        [ByPassUpdateParam]
        public string VoucherCode           { get; set; }
        [ByPassUpdateParam]
        public DateTime PackagingDate       { get; set; }
        public int DepartementID            { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DepartementName       { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DestinationCode       { get; set; }
        public int DestinationProduct       { get; set; }
        public decimal DestinationQty       { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DestinationName       { get; set; }
        public string Note                  { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public List<RawMaterial> Materials  { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted                 { get; set; }
        [ByPassInsertParam]
        public int PackagingID
        {
            get { return packagingID; }
            set
            {
                if (packagingID == 0)
                {
                    packagingID = value;
                }
            }
        } 
    }

    
}
