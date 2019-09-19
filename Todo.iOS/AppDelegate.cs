using System;
using Foundation;
using UIKit;
using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XF.Material.iOS;
using Shiny;
using AiForms.Renderers.iOS;


namespace Todo.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            iOSShinyHost.Init(new Startup());
            Forms.Init();
            FormsMaps.Init();
            this.LoadApplication(new App());
            SettingsViewInit.Init();
            Material.Init();

            return base.FinishedLaunching(app, options);
        }


        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
            => Shiny.Jobs.JobManager.OnBackgroundFetch(completionHandler);
    }
}
