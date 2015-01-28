using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class Notification : BaseEntityObject
    {
        [ByPassInsertParam]
        public int NotificationID { get; set; }
        public int DepartementId    { get; set; }
        public string VoucherCode   { get; set; }
        public string Note          { get; set; }
        public string Url           { get; set; }
        public string CreatedDate   { get; set; }
    }
}
