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
        IDisposable sub;


        public LocationPage()
        {
            this.InitializeComponent();
        }


        LocationViewModel ViewModel => (LocationViewModel)this.BindingContext;


        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.sub = this.ViewModel
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
                        Position = new Position(x.Item1.Value, x.Item2.Value)
                    });
                });
        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.sub?.Dispose();
        }


        void OnMapClicked(object sender, MapClickedEventArgs args)
        {
            var vm = (LocationViewModel)this.BindingContext;
            vm.Latitude = args.Position.Latitude;
            vm.Longitude = args.Position.Longitude;
        }
    }
}