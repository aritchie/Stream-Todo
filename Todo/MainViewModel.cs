using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Acr.UserDialogs.Forms;
using Shiny.Net;


namespace Todo
{
    public class MainViewModel : ViewModel
    {
        readonly IUserDialogs dialogs;
        readonly INavigationService navigator;
        readonly ITodoService todoService;


        public MainViewModel(ITodoService todoService,
                             IUserDialogs dialogs,
                             IConnectivity connectivity,
                             INavigationService navigator)
        {
            this.todoService = todoService;
            this.dialogs = dialogs;
            this.navigator = navigator;

            connectivity
                .WhenInternetStatusChanged()
                .ToPropertyEx(this, x => x.IsNetworkAvailable)
                .DisposeWith(this.DestroyWith);

            this.Add = navigator.NavigateCommand("EditPage");

            this.ToggleShowCompleted = ReactiveCommand.Create(() =>
            {
                this.ShowCompleted = !this.ShowCompleted;
                this.DoLoad();
            });

            this.Load = ReactiveCommand.CreateFromTask(async () =>
            {
                var todos = await todoService.GetList(this.ShowCompleted);
                this.List = todos.Select(ToViewModel).ToList();
            });

            this.WhenAnyValue(x => x.ShowCompleted)
                .Select(x => x ? "Hide Completed" : "Show Completed")
                .ToPropertyEx(this, x => x.ToggleShowText);
        }


        public ICommand Add { get; }
        public ICommand ToggleShowCompleted { get; }
        public IReactiveCommand Load { get; }
        public bool IsNetworkAvailable { [ObservableAsProperty] get; }
        public string ToggleShowText { [ObservableAsProperty] get; }
        [Reactive] public bool ShowCompleted { get; set; }
        [Reactive] public IList<TodoItemViewModel> List { get; private set; }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            this.DoLoad();
        }


        void DoLoad() => ((ICommand)this.Load).Execute(null);


        TodoItemViewModel ToViewModel(TodoItem item)
        {
            var vm = new TodoItemViewModel(item);

            vm.Edit = navigator.NavigateCommand(
                "EditPage",
                p => p.Add("Item", item)
            );
            vm.Delete = ReactiveCommand.CreateFromTask(async () =>
            {
                var confirm = await this.dialogs.Confirm($"Are you sure you wish to delete '${item.Title}'");
                if (confirm)
                {
                    await this.todoService.Remove(item.Id);
                    this.DoLoad();
                }
            });
            vm.MarkComplete = ReactiveCommand.CreateFromTask(async () =>
            {
                if (item.CompletionDateUtc == null)
                    item.CompletionDateUtc = DateTime.UtcNow;
                else
                    item.CompletionDateUtc = null;

                await this.todoService.Save(item);

                vm.RaisePropertyChanged(nameof(vm.IsCompleted));
                vm.RaisePropertyChanged(nameof(vm.IsOverdue));
            });
            return vm;
        }
    }
}
