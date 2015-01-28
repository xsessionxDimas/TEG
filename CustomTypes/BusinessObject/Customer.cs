using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class Customer : BaseEntityObject
    {
        private int customerID;
        [NullField]
        public int DepartementID      { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DepartementName { get; set; }
        [UCWordsField]
        public string FirstName       { get; set; }
        [UCWordsField]
        public string LastName        { get; set; }
        public string Address         { get; set; }
        public string Phone           { get; set; }
        public string MobilePhone     { get; set; }
        public string Email           { get; set; }
        [NullField]
        public int StatusId           { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string StatusName      { get; set; }
        [ActiveField]
        public bool CreditFeature     { get; set; }
        public decimal CreditLimit    { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public decimal CreditTotal    { get; set; }
        public int DueDate            { get; set; }
        [ActiveField]
        public bool Active            { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted           { get; set; }
        [ByPassInsertParam]
        public int CustomerID
        {
            get { return customerID; }
            set
            {
                if (customerID == 0)
                {
                    customerID = value;
                }
            }
        }

    }
}
