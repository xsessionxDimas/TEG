using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class PublicIP : BaseEntityObject
    {
        private int publicIPID;
        public string IPAddress { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted     { get; set; }
        [ByPassInsertParam]
        public int PublicIPID
        {
            get { return publicIPID; }
            set
            {
                if (publicIPID == 0)
                {
                    publicIPID = value;
                }
            }
        }
    }
}
