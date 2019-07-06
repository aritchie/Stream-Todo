using System;
using System.Windows.Input;
using Prism.Navigation;
using ReactiveUI;
using Shiny;

namespace Todo
{
    public class TodoItemViewModel : ReactiveObject
    {
        public TodoItemViewModel(ITodoItem item,
                                 IDataService data,
                                 INavigationService navigator)
        {
            this.Item = item;

            this.Edit = navigator.NavigateCommand<TodoItemViewModel>(
                "EditPage",
                (ivm, p) => p.Add("Item", ivm.Item)
            );
            this.MarkComplete = ReactiveCommand.CreateFromTask(async () =>
            {
                if (item.CompletionDateUtc == null)
                    item.CompletionDateUtc = DateTime.UtcNow;
                else
                    item.CompletionDateUtc = null;

                await data.Update(item);

                this.RaisePropertyChanged(nameof(this.IsCompleted));
                this.RaisePropertyChanged(nameof(this.IsOverdue));

                // TODO: for unit testing later
                // TODO: cancel notification & geofence if completed
            });
        }


        public ICommand Edit { get; }
        public ICommand Delete { get; }
        public ICommand MarkComplete { get; }
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
                                 this.Item.DueDateUtc > DateTime.Now;
    }
}
