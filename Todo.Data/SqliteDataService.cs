using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Todo.Data
{
    class SqliteDataService : IDataService
    {
        readonly TodoSqliteConnection conn;
        public SqliteDataService(TodoSqliteConnection conn)
            => this.conn = conn;


        public Task Create(TodoItem item) => this.conn.InsertAsync(item);
        public Task Update(TodoItem item) => this.conn.UpdateAsync(item);


        public async Task<TodoItem> GetById(Guid itemId)
        {
            var result = await this.conn.GetAsync<SqliteTodoItem>(itemId);
            return result as TodoItem;
        }


        public async Task Delete(Guid itemId)
        {
            var item = await this.conn.GetAsync<SqliteTodoItem>(itemId);
            if (item != null)
            {
                item.IsDeleted = true;
                await this.conn.UpdateAsync(item);
            }
        }


        public async Task<IList<TodoItem>> GetAll(bool includeCompleted)
        {
            var query = this.conn
                .Todos
                .Where(x => !x.IsDeleted)
                //.OrderByDescending(x => x.DateCreatedUtc)
                .OrderBy(x => x.DueDateUtc);

            if (!includeCompleted)
                query = query.Where(x => x.CompletionDateUtc == null);

            var results = await query.ToListAsync();
            var list = results
                .OfType<TodoItem>()
                .ToList();

            return list;
        }
    }
}
