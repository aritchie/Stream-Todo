using System;
using SQLite;


namespace Todo.Data
{
    class SqliteTodoItem : TodoItem
    {
        [PrimaryKey]
        public new Guid Id { get; set; }

        public bool IsDeleted { get; set; }
    }
}
