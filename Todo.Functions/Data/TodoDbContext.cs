using System;
using Microsoft.EntityFrameworkCore;


namespace Todo.Functions.Data
{
    public class TodoDbContext : DbContext
    {
        public DbSet<TodoItem> Items { get; set; }
    }
}
