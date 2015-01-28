using System;
using System.Collections.Generic;
using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class JobCosting : BaseEntityObject
    {
        private int jobCostingID;
        [ByPassUpdateParam]
        public string VoucherCode           { get; set; }
        [ByPassUpdateParam]
        public DateTime JobCostingDate      { get; set; }
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
        [NullField]
        public DateTime DestinationDate     { get; set; }
        public string Note                  { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public List<RawMaterial> Materials  { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted                 { get; set; }
        [ByPassInsertParam]
        public int JobCostingID
        {
            get { return jobCostingID; }
            set
            {
                if (jobCostingID == 0)
                {
                    jobCostingID = value;
                }
            }
        } 
    }

    public class RawMaterial
    {
        private int materialID;
        public int DepartementID  { get; set; }
        public int ProductID      { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal Qty        { get; set; }
        public DateTime Date      { get; set; }
        public int Type           { get; set; }
        public string UnitName    { get; set; }
        public int Material
        {
            get { return materialID; }
            set
            {
                if (materialID == 0)
                {
                    materialID = value;
                }
            }
        }
    }
}
