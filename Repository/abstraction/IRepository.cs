using System.Collections.Generic;

namespace Repository.abstraction
{
    public interface IRepository<T>
    {
        int SaveRow(T param, string createdBy);
        int UpdateRow(T param, string updatedBy);
        int DeleteRow(int id, string updatedBy);
        IEnumerable<T> FindAll(List<Dictionary<string, object>> keyValueParam);
        T FindbyId(int id);
    }
}
