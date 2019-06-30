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
        public MainViewModel(IDataService dataService,
                             IConnectivity connectivity,
                             INavigationService navigator)
        {
            connectivity
                .WhenInternetStatusChanged()
                .ToPropertyEx(this, x => x.IsNetworkAvailable);

            this.Load = ReactiveCommand.CreateFromTask(async () =>
            {
                var todos = await dataService.GetAll(false);

                this.List = todos
                    .Select(item =>
                    {
                        var vm = new TodoItemViewModel(item);
                        vm.MarkComplete = ReactiveCommand.CreateFromTask(async () =>
                        {
                            if (item.CompletionDateUtc == null)
                                item.CompletionDateUtc = DateTime.UtcNow;
                            else
                                item.CompletionDateUtc = null;

                            await dataService.Update(item);

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
