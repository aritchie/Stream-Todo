using System;
using Foundation;
using UIKit;
using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Shiny;


namespace Todo.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            iOSShinyHost.Init(new ShinyStartup());
            Forms.Init();
            FormsMaps.Init();
            this.LoadApplication(new App());
            AiForms.Renderers.iOS.SettingsViewInit.Init();
            Rg.Plugins.Popup.Popup.Init();

            return base.FinishedLaunching(app, options);
        }


        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
            => Shiny.Jobs.JobManager.OnBackgroundFetch(completionHandler);
    }
}
