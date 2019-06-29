using System;


namespace Todo
{
    public interface ITodoItem
    {
        Guid Id { get; set; }

        string Title { get; set; }
        string Notes { get; set; }

        double? GpsLatitude { get; set; }
        double? GpsLongitude { get; set; }

        DateTime? DueDateUtc { get; set; }
        DateTime? CompletionDateUtc { get; set; }
        DateTime DateUpdatedUtc { get; set; }
    }
}
