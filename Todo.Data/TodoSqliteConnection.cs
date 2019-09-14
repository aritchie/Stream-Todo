using System;
using System.IO;
using Shiny.IO;
using SQLite;


namespace Todo.Data
{
    class TodoSqliteConnection : SQLiteAsyncConnection
    {
        public TodoSqliteConnection(IFileSystem fileSystem)
            : base(Path.Combine(fileSystem.AppData.FullName, "todo.db"))
        {
            this.GetConnection().CreateTable<SqliteTodoItem>();
        }


        public AsyncTableQuery<SqliteTodoItem> Todos => this.Table<SqliteTodoItem>();
    }
}
