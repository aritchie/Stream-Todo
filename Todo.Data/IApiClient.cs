using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Todo.Data
{
    public interface IApiClient
    {
        Task Save(TodoItem item);
        Task<IEnumerable<TodoItem>> Get(DateTime? deltaDate);
    }
}
