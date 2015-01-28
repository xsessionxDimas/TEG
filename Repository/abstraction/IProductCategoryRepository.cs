using System.Collections.Generic;

namespace Repository.abstraction
{
    public interface IProductCategoryRepository
    {
        bool CategoryNameAvailable(List<Dictionary<string, object>> keyValueParam);
        bool CategoryNameAvailableExcept(List<Dictionary<string, object>> keyValueParam);
    }
}
