using CustomTypes.Objects;
using Repository.DBClass;

namespace Repository.abstraction
{
    public abstract class RepositoryBase
    {
        protected MSSQLDatabase DBClass;

        public virtual int SaveStockCRUDLog(CRUDLogObject logObject)
        {
            return default(int);
        }

        public virtual int SaveCashCRUDLog(CashLogObject logObject)
        {
            return default(int);
        }

        public virtual int SaveBankCRUDLog(BankLogObject logObject)
        {
            return default(int);
        }

        public virtual void DeleteStockCRUDLog(CRUDLogObject logObject)
        {
            
        }

        public virtual void DeleteCashCRUDLog(CashLogObject logObject)
        {

        }

        public virtual void DeleteBankCRUDLog(BankLogObject logObject)
        {

        }
    }
}
