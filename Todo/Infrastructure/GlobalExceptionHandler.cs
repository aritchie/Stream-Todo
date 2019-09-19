using System;
using XF.Material.Forms.UI.Dialogs;
using ReactiveUI;
using Shiny;
using Shiny.Logging;


namespace Todo.Infrastructure
{
    public class GlobalExceptionHandler : IObserver<Exception>, IShinyStartupTask
    {
        readonly IMaterialDialog dialogs;
        public GlobalExceptionHandler(IMaterialDialog dialogs) => this.dialogs = dialogs;


        public void Start() => RxApp.DefaultExceptionHandler = this;
        public void OnCompleted() { }
        public void OnError(Exception error) { }


        public async void OnNext(Exception value)
        {
            Log.Write(value);
            await this.dialogs.AlertAsync(value.ToString(), "ERROR");
        }
    }
}