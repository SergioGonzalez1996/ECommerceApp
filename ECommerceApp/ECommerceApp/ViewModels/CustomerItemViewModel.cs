using ECommerceApp.Models;
using ECommerceApp.Services;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Xamarin.Forms;
using Plugin.Media;
using ECommerceApp.Classes;
using Plugin.Media.Abstractions;

namespace ECommerceApp.ViewModels
{
    public class CustomerItemViewModel : Customer, INotifyPropertyChanged
    {
        #region Attributes
        private NavigationService navigationService;
        private GeolocatorService geolocatorService;
        private DialogService dialogService;
        private DataService dataService;
        private ApiService apiService;
        private NetService netService;
        private ImageSource imageSource;
        private MediaFile file;
        private double latitude;
        private double longitude;
        private bool isRunning;
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public ObservableCollection<DepartmentItemViewModel> Departments { get; set; }
        public ObservableCollection<CityItemViewModel> Cities { get; set; }

        public ImageSource ImageSource
        {
            set
            {
                if (imageSource != value)
                {
                    imageSource = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageSource"));
                }
            }
            get
            {
                return imageSource;
            }
        }
        public bool IsRunning
        {
            set
            {
                if (isRunning != value)
                {
                    isRunning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRunning"));
                }
            }
            get
            {
                return isRunning;
            }

        }
        public new double Latitude
        {
            set
            {
                if (latitude != value)
                {
                    latitude = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Latitude"));
                }
            }
            get
            {
                return latitude;
            }

        }
        public new double Longitude
        {
            set
            {
                if (longitude != value)
                {
                    longitude = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Longitude"));
                }
            }
            get
            {
                return longitude;
            }

        }
        #endregion

        #region Constructors
        public CustomerItemViewModel()
        {
            navigationService = new NavigationService();
            geolocatorService = new GeolocatorService();
            dialogService = new DialogService();
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
        public ICommand TakePictureCommand { get { return new RelayCommand(TakePicture); } }
        private async void TakePicture()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await dialogService.ShowMessage("Error", "No se puede acceder a la camara.");
                return;
            }

            IsRunning = true;
            file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Photos",
                Name = "newCustomer.jpg"
            });

            if (file != null)
            {
                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    IsRunning = false;
                    return stream;
                });
            }
            IsRunning = false;
        }

        public ICommand NewCustomerCommand { get { return new RelayCommand(NewCustomer); } }
        private async void NewCustomer()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                await dialogService.ShowMessage("Error", "Debe ingresar un Email.");
                return;
            }
            if (!Utilities.IsValidEmail(UserName))
            {
                await dialogService.ShowMessage("Error", "Debe ingresar un Email valido.");
                return;
            }
            if (string.IsNullOrEmpty(FirstName))
            {
                await dialogService.ShowMessage("Error", "Debe ingresar un Nombres.");
                return;
            }
            if (string.IsNullOrEmpty(LastName))
            {
                await dialogService.ShowMessage("Error", "Debe ingresar un Apellidos.");
                return;
            }
            if (string.IsNullOrEmpty(Phone))
            {
                await dialogService.ShowMessage("Error", "Debe ingresar un Telefono.");
                return;
            }
            if (string.IsNullOrEmpty(Address))
            {
                await dialogService.ShowMessage("Error", "Debe ingresar una Direccion.");
                return;
            }
            if (DepartmentId == 0)
            {
                await dialogService.ShowMessage("Error", "Debe ingresar un Departamento.");
                return;
            }
            if (CityId == 0)
            {
                await dialogService.ShowMessage("Error", "Debe ingresar un Telefono.");
                return;
            }

            IsRunning = true;
            await geolocatorService.GetLocation();

            var customer = new Customer
            {
                Address = Address,
                CityId = CityId,
                DepartmentId = DepartmentId,
                FirstName = FirstName,
                LastName = LastName,
                IsUpdated = true,
                Phone = Phone,
                Latitude = geolocatorService.Latitude,
                Longitude = geolocatorService.Longitude,
                UserName = UserName,
            };

            var response = await apiService.NewCustomer(customer);

            if (response.IsSucces && file != null)
            {
                var newCustomer = (Customer)response.Result;
                var response2 = await apiService.SetPhoto(newCustomer.CustomerId, file.GetStream());
                var filenName = string.Format("{0}.jpg", newCustomer.CustomerId);
                var folder = "~/Content/Customers";
                var fullPath = string.Format("{0}/{1}", folder, filenName);
                customer.Photo = fullPath;
            }

            IsRunning = false;
            if (!response.IsSucces)
            {
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }

            await dialogService.ShowMessage("Confirmación.", response.Message);
            await navigationService.Back();
        }

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

        public ICommand UpdateCustomerLocationCommand { get { return new RelayCommand(UpdateCustomerLocation); } }
        private async void UpdateCustomerLocation()
        {
            IsRunning = true;
            await geolocatorService.GetLocation();
            var customer = dataService.Get<Customer>(false)
                .Where(c => c.UserName == UserName)
                .FirstOrDefault();
            if (customer != null && 
                geolocatorService.Latitude != 0 && 
                geolocatorService.Longitude != 0)
            {
                Latitude = geolocatorService.Latitude;
                Longitude = geolocatorService.Longitude;
                customer.Latitude = geolocatorService.Latitude;
                customer.Longitude = geolocatorService.Longitude;
                dataService.Update(customer);
                var response = await apiService.Update(customer, "Customers");
            }
            IsRunning = false;
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
