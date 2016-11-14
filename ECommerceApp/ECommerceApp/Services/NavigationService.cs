using ECommerceApp.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Services
{
    public class NavigationService
    {
        public async Task Navigate (string pageName)
        {
            App.Master.IsPresented = false;

            switch (pageName)
            {
                case "ProductsPage":
                    await App.Navigator.PushAsync(new ProductsPage());
                    break;
                case "CustomersPage":
                    await App.Navigator.PushAsync(new CustomersPage());
                    break;
                case "OrdersPage":
                    await App.Navigator.PushAsync(new OrdersPage());
                    break;
                case "DeliveriesPage":
                    await App.Navigator.PushAsync(new DeliveriesPage());
                    break;
                case "SyncPage":
                    await App.Navigator.PushAsync(new SyncPage());
                    break;
                case "SetupPage":
                    await App.Navigator.PushAsync(new SetupPage());
                    break;

                case "LogoutPage":
                    await App.Navigator.PopToRootAsync();
                    break;
                case "UserPage":
                    await App.Navigator.PopToRootAsync();
                    break;
                default:
                    break;
            }

        }
    }
}
