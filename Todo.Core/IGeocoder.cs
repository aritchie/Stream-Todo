using System;
using System.Threading.Tasks;

namespace Todo
{
    public interface IGeocoder
    {
        Task<string> GetAddressInfo(double latitude, double longitude);
    }
}
