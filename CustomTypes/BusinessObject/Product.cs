using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class Product : BaseEntityObject
    {
        private int productId;
        [NullField]
        public int CategoryID       { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string CategoryName  { get; set; }
        [NullField]
        public int UnitID           { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string UnitName      { get; set; }
        public string ProductCode   { get; set; }
        [UCWordsField]
        public string  ProductName  { get; set; }
        public decimal BronzePrice  { get; set; }
        public decimal SilverPrice  { get; set; }
        public decimal GoldPrice    { get; set; }
        [ActiveField]
        public bool Pricelist       { get; set; }
        [ActiveField]
        public bool Active          { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted         { get; set; }
        [ByPassInsertParam]
        public int ProductID
        {
            get { return productId; }
            set
            {
                if (productId == 0)
                {
                    productId = value;
                }
            }
        }
    }
}
