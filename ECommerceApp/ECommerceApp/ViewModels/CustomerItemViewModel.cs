using ECommerceApp.Models;
using ECommerceApp.Services;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceApp.ViewModels
{
    public class CustomerItemViewModel : Customer
    {
        #region Attributes
        private NavigationService navigationService;
        private DataService dataService;
        private ApiService apiService;
        private NetService netService;
        #endregion

        #region Properties
        public ObservableCollection<DepartmentItemViewModel> Departments { get; set; }
        public ObservableCollection<CityItemViewModel> Cities { get; set; }
        #endregion

        #region Constructors
        public CustomerItemViewModel()
        {
            navigationService = new NavigationService();
            dataService = new DataService();
            apiService = new ApiService();
            netService = new NetService();
            Departments = new ObservableCollection<DepartmentItemViewModel>();
            Cities = new ObservableCollection<CityItemViewModel>();

            LoadDepartments();
            LoadCities();
        }
        #endregion

        #region Commands
        public ICommand CustomerDetailCommand { get { return new RelayCommand(CustomerDetail); } }
        private async void CustomerDetail()
        {
            var customerItemViewModel = new CustomerItemViewModel
            {
                Address = Address,
                City = City,
                CityId = CityId,
                CompanyCustomers = CompanyCustomers,
                CustomerId = CustomerId,
                Department = Department,
                DepartmentId = DepartmentId,
                FirstName = FirstName,
                IsUpdated = IsUpdated,
                LastName = LastName,
                Latitude = Latitude,
                Longitude = Longitude,
                Orders = Orders,
                Phone = Phone,
                Photo = Photo,
                Sales = Sales,
                UserName = UserName,
            };

            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.SetCurrentCustomer(customerItemViewModel);
            await navigationService.Navigate("CustomerDetailsPage");
        }
        #endregion

        #region Methods
        private async void LoadDepartments()
        {
            var departments = new List<Department>();
            if (netService.IsConnected())
            {
                departments = await apiService.Get<Department>("Departments");
                dataService.Save(departments);
            }
            else
            {
                departments = dataService.Get<Department>(true);
            }
            ReloadDepartments(departments);
        }

        private void ReloadDepartments(List<Department> departments)
        {
            Departments.Clear();
            foreach (var department in departments.OrderBy(p => p.Name))
            {
                Departments.Add(new DepartmentItemViewModel
                {
                    Cities = department.Cities,
                    Customers = department.Customers,
                    DepartmentId = department.DepartmentId,
                    Name = department.Name,
                });
            }
        }

        private async void LoadCities()
        {
            var cities = new List<City>();
            if (netService.IsConnected())
            {
                cities = await apiService.Get<City>("Cities");
                dataService.Save(cities);
            }
            else
            {
                cities = dataService.Get<City>(true);
            }
            ReloadCities(cities);
        }

        private void ReloadCities(List<City> cities)
        {
            // TODO: Cascada de Ciudades
            Cities.Clear();
            foreach (var city in cities.OrderBy(p => p.Name))
            {
                Cities.Add(new CityItemViewModel
                {
                    CityId = city.CityId,
                    Customers = city.Customers,
                    Department = city.Department,
                    DepartmentId = city.DepartmentId,
                    Name = city.Name,
                });
            }
        }
        #endregion
    }
}
