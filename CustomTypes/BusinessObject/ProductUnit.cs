using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class ProductUnit : BaseEntityObject
    {
        private int unitID;

        [LowerCaseField]
        public string UnitName     { get; set; }
        public string Abbreviation { get; set; }
        [ActiveField]
        public bool Active         { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool   Deleted      { get; set; }
        [ByPassInsertParam]
        public int UnitID
        {
            get { return unitID; }
            set
            {
                if (unitID == 0)
                {
                    unitID = value;
                }
            }
        }
    }
}
