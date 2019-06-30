using System;

#if MOBILE
namespace Todo.Data
#else
namespace Todo.Functions.Data
#endif
{

#if MOBILE
    public class TodoItem : ITodoItem
    {
        public TodoItem()
        {
            this.Id = Guid.NewGuid();
        }


        [SQLite.PrimaryKey]
        public Guid Id { get; set; }
#else
    public class TodoItem
    {
        public Guid Id { get; set; }
#endif

        public string Title { get; set; }
        public string Notes { get; set; }

        public double? GpsLatitude { get; set; }
        public double? GpsLongitude { get; set; }

        public DateTime? DueDateUtc { get; set; }
        public DateTime? CompletionDateUtc { get; set; }
        public DateTime DateUpdatedUtc { get; set; }
    }
}
