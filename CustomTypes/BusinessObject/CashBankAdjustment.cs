using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class CashBankAdjustment : AdjustmentBase
    {
        public int CashBankId      { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string BankAccount  { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public decimal Nominal     { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public new BankLogObject LogObject { get; set; }
    }
}
