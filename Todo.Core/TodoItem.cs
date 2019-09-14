using System;


namespace Todo
{
    public class TodoItem
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Notes { get; set; }

        public double? GpsLatitude { get; set; }
        public double? GpsLongitude { get; set; }

        public DateTime? DueDateUtc { get; set; }
        public DateTime? CompletionDateUtc { get; set; }
        public DateTime DateUpdatedUtc { get; set; }
    }
}
