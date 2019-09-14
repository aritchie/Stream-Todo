using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Todo
{
    public interface ITodoService
    {
        Task Save(TodoItem todo);
        Task Remove(Guid todoItemId);
        Task<IList<TodoItem>> GetList(bool includeCompleted);
    }
}
