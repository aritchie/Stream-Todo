using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny.Locations;
using Shiny.Logging;

namespace Todo
{
    public class LocationViewModel : ViewModel
    {
        readonly IGpsManager gpsManager;


        public LocationViewModel(INavigationService navigator, IGpsManager gpsManager)
        {
            this.gpsManager = gpsManager;
            this.Cancel = navigator.GoBackCommand();

            this.PickLocation = navigator.GoBackCommand(
                false,
                p => p
                    .Set("Latitude", this.Latitude)
                    .Set("Longitude", this.Longitude),
                this.WhenAny(
                    x => x.Latitude,
                    x => x.Longitude,
                    (lat, lng) =>
                    {
                        var latv = lat.GetValue();
                        if (latv != null && (latv >= 89.9 || latv <= -89.9))
                            return false;

                        var lngv = lng.GetValue();
                        if (lngv != null && (lngv >= 179.9 || lngv <= -179.9))
                            return false;

                        return true;
                    }
                )
            );
        }


        public ICommand PickLocation { get; }
        public ICommand Cancel { get; }
        [Reactive] public double? Latitude { get; set; }
        [Reactive] public double? Longitude { get; set; }


        public override async Task InitializeAsync(INavigationParameters parameters)
        {
            await this.TrySetLocation();
            await base.InitializeAsync(parameters);
        }


        async Task TrySetLocation()
        {
            try
            {
                var reading = await this.gpsManager
                    .GetLastReading()
                    .Timeout(TimeSpan.FromSeconds(5));

                if (reading != null)
                {
                    this.Latitude = reading.Position.Latitude;
                    this.Longitude = reading.Position.Longitude;
                }
            }
            catch (Exception ex)
            {
                Log.Write(
                    "NoLocation",
                    "Unable to retrieve GPS coordinates",
                    ("Error", ex.ToString())
                );
            }
        }
    }
}
