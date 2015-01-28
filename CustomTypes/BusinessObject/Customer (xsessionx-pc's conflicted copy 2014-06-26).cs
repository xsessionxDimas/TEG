using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class Customer : BaseEntityObject
    {
        /*
         *  region class fields
         * 
         */
        private int customerID;
        /*
         * 
         *  region class properties
         */
        [UCWordsField]
        public string FirstName    { get; set; }
        [UCWordsField]
        public string LastName     { get; set; }
        public string Address      { get; set; }
        [NullField]
        public string Phone        { get; set; }
        [NullField]
        public string MobilePhone  { get; set; }
        [NullField]
        public string Email        { get; set; }
        public int    StatusId     { get; set; }
        public decimal CreditLimit { get; set; }
        [ActiveField]
        public bool   Active       { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted        { get; set; }
        [ByPassInsertParam]
        public int CustomerId
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
