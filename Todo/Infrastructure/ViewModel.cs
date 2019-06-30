using System;
using System.Reactive.Disposables;
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


        protected CompositeDisposable DestroyWith { get; } = new CompositeDisposable();

        CompositeDisposable deactivateWith;
        protected CompositeDisposable DeactivateWith
        {
            get
            {
                if (this.deactivateWith == null)
                    this.deactivateWith = new CompositeDisposable();

                return this.deactivateWith;
            }
        }


        protected void BindBusy(IReactiveCommand command) =>
            command.IsExecuting.Subscribe(
                x => this.IsBusy = x,
                _ => this.IsBusy = false,
                () => this.IsBusy = false
            );


        public virtual void Destroy()
        {
            this.DestroyWith?.Dispose();
        }


        public virtual Task InitializeAsync(INavigationParameters parameters)
            => Task.CompletedTask;


        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
            this.deactivateWith?.Dispose();
            this.deactivateWith = null;
        }


        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            // /NavigationPage/EditPage?Key=1&Key2=string
            //parameters.GetValue<MyObject>("Key1")
        }
    }
}
