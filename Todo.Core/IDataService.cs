using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Todo.Core
{
    public interface DataService
    {
        Task<IList<ITodoItem>> GetAll(bool includeCompleted);
        Task<ITodoItem> GetById(Guid itemId);
        Task Delete(Guid itemId);
        Task Update(ITodoItem item);
        Task Create(Action<ITodoItem> item);
    }
}
