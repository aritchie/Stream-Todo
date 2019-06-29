using System;
using System.IO;
using Shiny.IO;
using SQLite;


namespace Todo
{
    public class TodoSqliteConnection : SQLiteAsyncConnection
    {
        public TodoSqliteConnection(IFileSystem fileSystem)
            : base(Path.Combine(fileSystem.AppData.FullName, "todo.db"))
        {
            //this.GetConnection().CreateTable<TodoItem>();
        }


        //public AsyncTableQuery<TodoItem> Todos => this.Table<TodoItem>();
    }
}
