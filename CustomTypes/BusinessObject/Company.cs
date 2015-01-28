using System.IO;
using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class Company : BaseEntityObject
    {
        private int companyID;

        [UCWordsField]
        public string CompanyName  { get; set; }
        [NullField]
        public Stream CompanyLogo  { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted        { get; set; }
        [ActiveField]
        public bool Active         { get; set; }
        [ByPassInsertParam]
        public int CompanyID
        {
            get { return companyID; }
            set
            {
                if (companyID == 0)
                {
                    companyID = value;
                }
            }
        }
    }
}
