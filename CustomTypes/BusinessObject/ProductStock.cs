using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class ProductStock : BaseEntityObject
    {
        private int stockId;
        [NullField]
        [ByPassUpdateParam]
        public int ProductID           { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string ProductCode      { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string ProductName      { get; set; }
        [NullField]
        [ByPassUpdateParam]
        public int DepartementID       { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DepartementName  { get; set; }
        public decimal MinimumQuantity { get; set; }
        [ByPassUpdateParam]
        public decimal StartQuantity   { get; set; }
        [ByPassUpdateParam]
        public decimal CurrentQuantity { get; set; }
        [ByPassInsertParam]
        public int StockID
        {
            get { return stockId; }
            set
            {
                if (stockId == 0)
                {
                    stockId = value;
                }
            }
        }
    }
}
