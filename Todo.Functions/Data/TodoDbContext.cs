using System;
using Microsoft.EntityFrameworkCore;
using Todo.Functions.Models;


namespace Todo.Functions.Data
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions options) : base(options) { }
        public DbSet<TodoItem> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var todo = modelBuilder.Entity<TodoItem>();
            todo.ToTable("TodoItems");
            todo.HasKey(x => x.Id);
            todo.Property(x => x.Id).HasColumnName("TodoItemId");

            var user = modelBuilder.Entity<User>();
            user.ToTable("Users");
            user.HasKey(x => x.Id);
            user.Property(x => x.Id).HasColumnName("UserId");
        }
    }
}
