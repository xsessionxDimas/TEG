using System.Collections.Generic;

namespace Repository.abstraction
{
    public interface IUniqueValidation
    {
        bool UniqueNameAvailable(List<Dictionary<string, object>> keyValueParam);
        bool UniqueNameAvailableExcept(List<Dictionary<string, object>> keyValueParam);
    }
}
