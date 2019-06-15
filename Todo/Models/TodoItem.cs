using System;
using SQLite;

namespace Todo.Models
{
    public class TodoItem
    {
        public TodoItem()
        {
            this.Id = Guid.NewGuid();
        }


        [PrimaryKey]
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Notes { get; set; }

        public double? GpsLatitude { get; set; }
        public double? GpsLongitude { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? CompletionDate { get; set; }
    }
}
