using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class Departement : BaseEntityObject
    {
        private int departementID;

        [UCWordsField]
        public string DepartementName { get; set; }
        [NullField]
        public int CompanyID          { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string CompanyName     { get; set; }
        [NullField]
        public int? SupervisorID      { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string SupervisorName  { get; set; }
        [NullField]
        public int? WarehouseID       { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string WarehouseName   { get; set; }
        public string Address         { get; set; }
        public string Phone           { get; set; }
        public bool IsSupervisor      { get; set; }
        public bool IsTreasurer       { get; set; }
        public bool IsWarehouse       { get; set; }
        public bool IsOutlet          { get; set; }
        [UCWordsField]
        public string Head            { get; set; }
        [ActiveField]
        public bool Active            { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted           { get; set; }
        [ByPassInsertParam]
        public int DepartementID
        {
            get { return departementID; }
            set
            {
                if(departementID == 0)
                {
                    departementID = value;
                }
            }
        }
    }
}
