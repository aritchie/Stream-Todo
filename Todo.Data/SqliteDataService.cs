using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Todo.Data
{
    public class SqliteDataService : IDataService
    {
        readonly TodoSqliteConnection conn;


        public SqliteDataService(TodoSqliteConnection conn)
        {
            this.conn = conn;
        }


        public async Task<ITodoItem> Create(Action<ITodoItem> itemAction)
        {
            var item = new TodoItem();
            itemAction(item);
            await this.conn.InsertAsync(item);
            return item;
        }


        public async Task Delete(Guid itemId)
        {
            var item = await this.conn.GetAsync<TodoItem>(itemId);
            if (item != null)
            {
                item.IsDeleted = true;
                await this.conn.UpdateAsync(item);
            }
        }


        public async Task<IList<ITodoItem>> GetAll(bool includeCompleted)
        {
            var query = this.conn
                .Todos
                .Where(x => !x.IsDeleted);

            if (!includeCompleted)
                query = query.Where(x => x.CompletionDateUtc == null);

            var results = await query.ToListAsync();
            var list = results
                .OfType<ITodoItem>()
                .ToList();

            return list;
        }

        public async Task<ITodoItem> GetById(Guid itemId)
            => await this.conn.GetAsync<TodoItem>(itemId);


        public Task Update(ITodoItem item)
            => this.conn.UpdateAsync(item);
    }
}
