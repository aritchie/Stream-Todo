using System;
using Microsoft.EntityFrameworkCore;


namespace Todo.Functions.Data
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions options) : base(options) { }
        public DbSet<TodoItem> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var table = modelBuilder.Entity<TodoItem>();
            table.ToTable("TodoItems");
            table.HasKey(x => x.Id);
            table.Property(x => x.Id).HasColumnName("TodoItemId");
        }
    }
}
