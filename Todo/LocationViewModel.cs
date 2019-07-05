using System;
using System.Windows.Input;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Todo
{
    public class LocationViewModel : ViewModel
    {
        public LocationViewModel(INavigationService navigator)
        {
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
    }
}
