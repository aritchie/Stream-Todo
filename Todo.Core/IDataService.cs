using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Todo
{
    public interface IDataService
    {
        Task<IList<ITodoItem>> GetAll(bool includeCompleted);
        Task<ITodoItem> GetById(Guid itemId);
        Task Delete(Guid itemId);
        Task Update(ITodoItem item);
        Task<ITodoItem> Create(Action<ITodoItem> item);
    }
}
