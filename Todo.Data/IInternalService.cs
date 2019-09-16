using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Todo.Data
{
    interface IInternalService
    {
        Task SaveSync(TodoItem todo);
        Task<IList<TodoItem>> GetItemsToSync();
    }
}
