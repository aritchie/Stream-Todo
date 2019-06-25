using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny.Net;

namespace Todo
{
    public class MainViewModel : ViewModel
    {
        readonly TodoSqliteConnection conn;


        public MainViewModel(TodoSqliteConnection conn, 
                             IConnectivity connectivity,
                             INavigationService navigator)
        {
            this.conn = conn;

            connectivity
                .WhenInternetStatusChanged()
                .ToPropertyEx(this, x => x.IsNetworkAvailable);

            this.Load = ReactiveCommand.CreateFromTask(async () =>
            {
                var todos = await conn.Todos.ToListAsync();

                this.List = todos
                    .Select(item =>
                    {
                        var vm = new TodoItemViewModel(item);
                        vm.MarkComplete = ReactiveCommand.CreateFromTask(async () =>
                        {
                            if (item.CompletionDate == null)
                                item.CompletionDate = DateTime.Now;
                            else
                                item.CompletionDate = null;

                            await conn.UpdateAsync(item);

                            // TODO: for unit testing later
                            // TODO: cancel notification & geofence if completed
                            //item.MarkDirty();
                        });
                        // TODO: pass item
                        vm.Edit = navigator.NavigateCommand("EditPage");

                        return vm;
                    })
                    .ToList();
            });

        }


        public IReactiveCommand Load { get; }
        public bool IsNetworkAvailable { [ObservableAsProperty] get; }
        [Reactive] public IList<TodoItemViewModel> List { get; private set; }
    }
}
