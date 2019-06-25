using System;
using System.Windows.Input;
using ReactiveUI;
using Todo.Models;


namespace Todo
{
    public class TodoItemViewModel : ReactiveObject
    {
        public TodoItemViewModel(TodoItem todo)
        { 
            this.Item = todo;
        }


        public void MarkDirty()
        {
            this.RaisePropertyChanged(nameof(IsCompleted));
            this.RaisePropertyChanged(nameof(IsOverdue));
        }


        public ICommand Edit { get; set; }
        public ICommand Delete { get; set; }
        public ICommand MarkComplete { get; set; }
        public TodoItem Item { get; }

        public string Title => this.Item.Title;
        public DateTime? DueDate => this.Item.DueDate;

        public bool IsCompleted => this.Item.CompletionDate != null;
        public bool HasDueDate => this.Item.DueDate != null;
        public bool IsOverdue => this.Item.DueDate != null && 
                                 this.Item.CompletionDate == null &&
                                 this.Item.DueDate > DateTime.Now;
    }
}
