using System;
using System.Data;

namespace Repository.abstraction
{
    public abstract class BaseServiceClass
    {
        public virtual DataSet[] GetReportDataSource(int keyID, string inWords)
        {
            throw new NotImplementedException();
        }
    }
}
