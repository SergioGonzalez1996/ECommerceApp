using ECommerceApp.Interfaces;
using Xamarin.Forms;

namespace ECommerceApp.Services
{
    public class NetService
    {
        public bool IsConnected()
        {
            var networkConnection = DependencyService.Get<INetworkConnection>();
            networkConnection.CheckNetworkConnection();
            return networkConnection.IsConnected;
        }
    }
}
