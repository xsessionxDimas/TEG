using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class Marketing : BaseEntityObject
    {
        private int marketingID;

        [UCWordsField]
        public string MarketingName   { get; set; }
        [NullField]
        public int DepartementID      { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DepartementName { get; set; }
        [ActiveField]
        public bool Active            { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted           { get; set; }
        [ByPassInsertParam]
        public int MarketingID
        {
            get { return marketingID; }
            set
            {
                if(marketingID == 0)
                {
                    marketingID = value;
                }
            }
        }
    }
}
