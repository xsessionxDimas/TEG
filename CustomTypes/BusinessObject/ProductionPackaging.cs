using System;
using System.Collections.Generic;
using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class ProductionPackaging : BaseEntityObject
    {
        private int productionPackagingID;
        [ByPassUpdateParam]
        public string VoucherCode               { get; set; }
        [ByPassUpdateParam]
        public DateTime ProductionPackagingDate { get; set; }
        public int DepartementID                { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DepartementName           { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DestinationCode           { get; set; }
        public int DestinationProduct           { get; set; }
        public decimal DestinationQty           { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DestinationName           { get; set; }
        public string Note                      { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public List<RawMaterial> Materials      { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted                     { get; set; }
        [ByPassInsertParam]
        public int ProductionPackagingID
        {
            get { return productionPackagingID; }
            set
            {
                if (productionPackagingID == 0)
                {
                    productionPackagingID = value;
                }
            }
        } 
    }

    
}
