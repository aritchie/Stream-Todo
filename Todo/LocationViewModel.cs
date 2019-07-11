using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny.Locations;

namespace Todo
{
    public class LocationViewModel : ViewModel
    {
        readonly IGpsManager gpsManager;


        public LocationViewModel(INavigationService navigator, 
                                 IGpsManager gpsManager)
        {
            this.gpsManager = gpsManager;

            this.PickLocation = navigator.GoBackCommand(
                false,
                p => p
                    .Set("Latitude", this.Latitude)
                    .Set("Longitude", this.Longitude),
                this.WhenAny(
                    x => x.Latitude,
                    x => x.Longitude,
                    (lat, lng) => true
                )
            );
        }


        public ICommand PickLocation { get; }
        [Reactive] public double Latitude { get; set; } = 43.6425662;
        [Reactive] public double Longitude { get; set; } = -79.3892455;


        public override async Task InitializeAsync(INavigationParameters parameters)
        {
            //// TODO: also watch for editing
            //try
            //{
            //    var reading = await this.gpsManager
            //        .GetLastReading()
            //        .Timeout(TimeSpan.FromSeconds(3));

            //    this.Latitude = reading.Position.Latitude;
            //    this.Longitude = reading.Position.Longitude;
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.Write(ex.ToString());
            //}
            this.Latitude = 43.6425662;
            this.Longitude = -79.3892455;
            await base.InitializeAsync(parameters);
        }
    }
}
