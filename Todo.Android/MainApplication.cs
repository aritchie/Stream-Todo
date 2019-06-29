using System;
using Shiny;
using Android.App;
using Android.Runtime;


namespace Todo.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = false)]
#endif
    public class MainApplication : Application
    {
        public MainApplication() : base() { }
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }


        public override void OnCreate()
        {
            base.OnCreate();
            AndroidShinyHost.Init(
                this,
                new ShinyStartup()
            );
        }
    }
}