using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class CashAdjustment : AdjustmentBase
    {
        public int CashId         { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public decimal Nominal    { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public new CashLogObject LogObject { get; set; }
    }
}
