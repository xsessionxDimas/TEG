using System.Collections.Generic;

namespace Repository.abstraction
{
    public interface IProductUnitRepository
    {
        bool ProductUnitNameAvailable(List<Dictionary<string, object>> keyValueParam);
        bool ProductUnitNameAvailableExcept(List<Dictionary<string, object>> keyValueParam);
    }
}
