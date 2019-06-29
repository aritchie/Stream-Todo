using System;
using System.Threading.Tasks;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Todo
{
    public abstract class ViewModel : ReactiveObject, 
                                      INavigatedAware,
                                      IInitializeAsync,
                                      IDestructible
    {
        [Reactive] public bool IsBusy { get; protected set; }
        [Reactive] public string Title { get; protected set; }


        protected void BindBusy(IReactiveCommand command) =>
            command.IsExecuting.Subscribe(
                x => this.IsBusy = x,
                _ => this.IsBusy = false,
                () => this.IsBusy = false
            );


        public virtual void Destroy()
        {
        }


        public virtual Task InitializeAsync(INavigationParameters parameters)
            => Task.CompletedTask;


        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }


        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            // /NavigationPage/EditPage?Key=1&Key2=string
            //parameters.GetValue<MyObject>("Key1")
        }
    }
}
