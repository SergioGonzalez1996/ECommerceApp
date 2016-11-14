using ECommerceApp.Models;
using ECommerceApp.Pages;
using System.Threading.Tasks;
using System;
using ECommerceApp.ViewModels;

namespace ECommerceApp.Services
{
    public class NavigationService
    {
        #region Attributes
        private DataService dataService; 
        #endregion

        #region Constructor
        public NavigationService()
        {
            dataService = new DataService();
        }
        #endregion

        #region Navigation
        public async Task Navigate(string pageName)
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
                    Logout();
                    break;
                case "UserPage":
                    await App.Navigator.PopToRootAsync();
                    break;
                default:
                    break;
            }

        }
        #endregion

        #region Methods
        private void Logout()
        {
            App.CurrentUser.IsRemembered = false;
            dataService.UpdateUser(App.CurrentUser);
            App.Current.MainPage = new LoginPage();
        }

        internal void SetMainPage(User user)
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.LoadUser(user);
            App.CurrentUser = user;
            App.Current.MainPage = new MasterPage();
        }

        public User GetCurrentUser()
        {
            return App.CurrentUser;
        }
        #endregion
    }
}
