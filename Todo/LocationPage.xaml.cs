using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Maps;


namespace Todo
{
    public partial class LocationPage : ContentPage
    {
        public LocationPage()
        {
            this.InitializeComponent();
        }


        LocationViewModel ViewModel => (LocationViewModel)this.BindingContext;


        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.ViewModel
                .WhenAnyValue(
                    x => x.Latitude,
                    x => x.Longitude
                )
                .Skip(1)
                .Subscribe(x =>
                {
                    this.myMap.Pins.Clear();
                    this.myMap.Pins.Add(new Pin
                    {
                        Label = "Somewhere",
                        Position = new Position(x.Item1, x.Item2)
                    });
                });
                //.DisposeWith(this.ViewModel.Dis);

        }


        void OnMapClicked(object sender, MapClickedEventArgs args)
        {
            var vm = (LocationViewModel)this.BindingContext;
            vm.Latitude = args.Position.Latitude;
            vm.Longitude = args.Position.Longitude;
        }
    }
}