using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class ProductCategory : BaseEntityObject
    {
        private int categoryID;

        [UCWordsField]
        public string CategoryName { get; set; }
        [ActiveField]
        public bool Active         { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted        { get; set; }
        [ByPassInsertParam]
        public int CategoryID
        {
            get { return categoryID; }
            set
            {
                if (categoryID == 0)
                {
                    categoryID = value;
                }
            }
        }
    }
}
