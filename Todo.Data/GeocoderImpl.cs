using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;


namespace Todo.Data
{
    public class GeocoderImpl : IGeocoder
    {
        public async Task<string> GetAddressInfo(double latitude, double longitude)
        {
            var placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);
            var info = placemarks.FirstOrDefault();
            return info == null ? String.Empty : $"{info.Locality} - {info.AdminArea}";
        }
    }
}
