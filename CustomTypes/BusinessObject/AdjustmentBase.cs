using System;
using CustomTypes.Attributes;
using CustomTypes.Objects;

namespace CustomTypes.Base
{
    public abstract class AdjustmentBase : BaseEntityObject
    {
        private int _adjustmentId;
        [ByPassUpdateParam]
        public string VoucherCode       { get; set; }
        public int DepartementId        { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DepartementName   { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string AdjustmentType    { get; set; }
        public decimal DepositQty       { get; set; }
        public decimal WithdrawQty      { get; set; }
        public string Note              { get; set; }
        public DateTime AdjustmentDate  { get; set; }
        [ByPassInsertParam]
        public int AdjustmentID
        {
            get { return _adjustmentId; }
            set
            {
                if (_adjustmentId == 0)
                {
                    _adjustmentId = value;
                }
            }
        }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public CRUDLogObject LogObject { get; set; }
    }
}

