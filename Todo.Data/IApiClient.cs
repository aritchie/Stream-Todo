using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;


namespace Todo.Data
{
    public interface IApiClient
    {
        [Post("/api/Save")]
        Task Save(TodoItem item);


        [Get("/api/Get?IncludeCompleted={includeCompleted}&Delta={deltaDate}")]
        Task<IEnumerable<TodoItem>> Get(bool includeCompleted, DateTime? deltaDate);
    }
}
