using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class StockAdjustment : AdjustmentBase
    {
        public int ProductId      { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string ProductCode { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string ProductName { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public decimal Qty        { get; set; }
    }
}
