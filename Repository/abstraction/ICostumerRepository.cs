using System.Collections.Generic;

namespace Repository.abstraction
{
    public interface ICostumerRepository
    {
        bool CostumerNameAvailable(List<Dictionary<string, object>> keyValueParam);
        bool CostumerNameAvailableExcept(List<Dictionary<string, object>> keyValueParam);
    }
}
