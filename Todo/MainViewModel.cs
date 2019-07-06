using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
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
                .ToPropertyEx(this, x => x.IsNetworkAvailable)
                .DisposeWith(this.DestroyWith);

            this.Add = navigator.NavigateCommand("EditPage");
            this.ToggleComplete = ReactiveCommand.Create(() => this.ShowCompleted = !this.ShowCompleted);

            this.Load = ReactiveCommand.CreateFromTask(async () =>
            {
                var todos = await dataService.GetAll(this.ShowCompleted);
                this.List = todos
                    .Select(item => new TodoItemViewModel(item, dataService, navigator))
                    .ToList();
            });
            this.WhenAnyValue(x => x.ShowCompleted)
                .Skip(1)
                .Subscribe(_ => ((ICommand)this.Load).Execute(null));

            this.WhenAnyValue(x => x.ShowCompleted)
                .Select(x => x ? "Hide Completed" : "Show Completed")
                .ToPropertyEx(this, x => x.ToggleText);
        }


        public ICommand Add { get; }
        public ICommand ToggleComplete { get; }
        public IReactiveCommand Load { get; }
        public bool IsNetworkAvailable { [ObservableAsProperty] get; }
        public string ToggleText { [ObservableAsProperty] get; }
        [Reactive] public bool ShowCompleted { get; set; }
        [Reactive] public IList<TodoItemViewModel> List { get; private set; }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            ((ICommand)this.Load).Execute(null);
        }
    }
}
