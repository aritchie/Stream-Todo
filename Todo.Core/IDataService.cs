using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Todo
{
    public interface IDataService
    {
        Task<IList<TodoItem>> GetAll(bool includeCompleted);
        Task<TodoItem> GetById(Guid itemId);
        Task Delete(Guid itemId);
        Task Update(TodoItem item);
        Task Create(TodoItem item);
    }
}
