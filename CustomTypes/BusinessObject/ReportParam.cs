using System;

namespace CustomTypes.BusinessObject
{
    public class ReportParam
    {
        public int? DepartementId  { get; private set; }
        public int? CashId         { get; private set; }
        public int? CashBankId     { get; private set; }
        public int? ProductId      { get; private set; }
        public DateTime? DateStart { get; private set; }
        public DateTime? DateEnd   { get; private set; }
        public short? Kwartal      { get; private set; }
        public int? Year           { get; private set; }
        public DateTime PrintDate  { get; private set; }
    }
}
