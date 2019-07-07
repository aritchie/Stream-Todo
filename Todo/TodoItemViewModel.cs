using System;
using System.Windows.Input;
using ReactiveUI;
using Shiny;


namespace Todo
{
    public class TodoItemViewModel : ReactiveObject
    {
        public TodoItemViewModel(ITodoItem item) => this.Item = item;

        public ICommand Edit { get; set; }
        public ICommand Delete { get; set; }
        public ICommand MarkComplete { get; set; }
        public ITodoItem Item { get; }

        public string Title => this.Item.Title;
        public string Notes => this.Item.Notes;
        public DateTime? DueDate => this.Item.DueDateUtc?.ToLocalTime();

        public bool HasNotes => !this.Item.Notes.IsEmpty();
        public string Location => $"({this.Item.GpsLatitude} - {this.Item.GpsLongitude})";
        public bool HasLocation => this.Item.GpsLongitude != null;
        public bool IsCompleted => this.Item.CompletionDateUtc != null;
        public bool HasDueDate => this.Item.DueDateUtc != null;
        public bool IsOverdue => this.Item.DueDateUtc != null &&
                                 this.Item.CompletionDateUtc == null &&
                                 this.Item.DueDateUtc < DateTime.UtcNow;
    }
}
