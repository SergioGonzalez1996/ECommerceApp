using ECommerceApp.Models;
using ECommerceApp.Services;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System;
using System.Linq;
using Xamarin.Forms.Maps;

namespace ECommerceApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Attributes
        private NavigationService navigationService;
        private DataService dataService;
        private ApiService apiService;
        private NetService netService;
        private string productsFilter;
        private string customersFilter;
        private bool isRefreshingCustomers;
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public ObservableCollection<Pin> Pins { get; set; }
        public ObservableCollection<MenuItemViewModel> Menu { get; set; }
        public ObservableCollection<ProductItemViewModel> Products { get; set; }
        public ObservableCollection<CustomerItemViewModel> Customers { get; set; }
        public LoginViewModel NewLogin { get; set; }
        public UserViewModel UserLoged { get; set; }
        public CustomerItemViewModel CurrentCustomer { get; set; }
        public CustomerItemViewModel NewCustomer { get; set; }

        public string ProductsFilter
        {
            set
            {
                if (productsFilter != value)
                {
                    productsFilter = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProductsFilter"));
                    if (string.IsNullOrEmpty(productsFilter))
                    {
                        LoadLocalProducts();
                    }
                }
            }
            get
            {
                return productsFilter;
            }
        }
        public string CustomersFilter
        {
            set
            {
                if (customersFilter != value)
                {
                    customersFilter = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CustomersFilter"));
                    if (string.IsNullOrEmpty(customersFilter))
                    {
                        LoadLocalCustomers();
                    }
                }
            }
            get
            {
                return customersFilter;
            }
        }
        public bool IsRefreshingCustomers
        {
            set
            {
                if (isRefreshingCustomers != value)
                {
                    isRefreshingCustomers = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRefreshingCustomers"));
                }
            }
            get
            {
                return isRefreshingCustomers;
            }
        }
        #endregion

        #region Constructors
        public MainViewModel()
        {
            // Singleton
            instance = this;

            // Instance services
            navigationService = new NavigationService();
            dataService = new DataService();
            apiService = new ApiService();
            netService = new NetService();

            // Create Observable Collections
            Pins = new ObservableCollection<Pin>();
            Menu = new ObservableCollection<MenuItemViewModel>();
            Products = new ObservableCollection<ProductItemViewModel>();
            Customers = new ObservableCollection<CustomerItemViewModel>();

            // Create Views
            NewLogin = new LoginViewModel();
            UserLoged = new UserViewModel();
            NewCustomer = new CustomerItemViewModel();
            CurrentCustomer = new CustomerItemViewModel();

            // Load Data
            LoadMenu();
            LoadProducts();
            LoadCustomers();
        }

        #endregion

        #region Singleton
        static MainViewModel instance;

        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new MainViewModel();
            }
            return instance;
        }
        #endregion

        #region Commands
        public ICommand SearchProductCommand { get { return new RelayCommand(SearchProduct); } }
        private void SearchProduct()
        {
            var products = dataService.GetProducts(ProductsFilter);
            Products.Clear();
            ReloadProducts(products);
        }

        public ICommand SearchCustomerCommand { get { return new RelayCommand(SearchCustomer); } }
        private void SearchCustomer()
        {
            var customers = dataService.GetCustomers(CustomersFilter);
            Customers.Clear();
            ReloadCustomers(customers);
        }

        public ICommand NewCustomerCommand { get { return new RelayCommand(CustomerNew); } }
        private async void CustomerNew()
        {
            await navigationService.Navigate("NewCustomerPage");
        }

        public ICommand RefreshCustomersCommand { get { return new RelayCommand(RefreshCustomers); } }
        private void RefreshCustomers()
        {
            LoadCustomers();
            IsRefreshingCustomers = false;
        }
        #endregion

        #region Methods
        public void LoadUser(User user)
        {
            UserLoged.FullName = user.FullName;
            UserLoged.Photo = user.PhotoFullPath;
        }

        private void ReloadProducts(List<Product> products)
        {
            Products.Clear();
            foreach (var product in products.OrderBy(p => p.Description))
            {
                Products.Add(new ProductItemViewModel
                {
                    BarCode = product.BarCode,
                    Category = product.Category,
                    CategoryId = product.CategoryId,
                    Company = product.Company,
                    CompanyId = product.CompanyId,
                    Description = product.Description,
                    Image = product.Image,
                    Inventories = product.Inventories,
                    Price = product.Price,
                    ProductId = product.ProductId,
                    Remarks = product.Remarks,
                    Stock = product.Stock,
                    Tax = product.Tax,
                    TaxId = product.TaxId,
                });
            }
        }

        private void LoadLocalProducts()
        {
            var products = dataService.Get<Product>(true);
            ReloadProducts(products);
        }

        private async void LoadProducts()
        {
            var products = new List<Product>();
            if (netService.IsConnected())
            {
                products = await apiService.Get<Product>("Products");
                dataService.Save(products);
            }
            else
            {
                products = dataService.Get<Product>(true);
            }
            ReloadProducts(products);
        }

        private void ReloadCustomers(List<Customer> customers)
        {
            Customers.Clear();
            foreach (var customer in customers.OrderBy(c => c.FirstName).ThenBy(c => c.LastName))
            {
                Customers.Add(new CustomerItemViewModel
                {
                    Address = customer.Address,
                    City = customer.City,
                    CityId = customer.CityId,
                    CompanyCustomers = customer.CompanyCustomers,
                    CustomerId = customer.CustomerId,
                    Department = customer.Department,
                    DepartmentId = customer.DepartmentId,
                    FirstName = customer.FirstName,
                    IsUpdated = customer.IsUpdated,
                    LastName = customer.LastName,
                    Latitude = customer.Latitude,
                    Longitude = customer.Longitude,
                    Orders = customer.Orders,
                    Phone = customer.Phone,
                    Photo = customer.Photo,
                    Sales = customer.Sales,
                    UserName = customer.UserName,
                });
            }
        }

        private async void LoadCustomers()
        {
            var customers = new List<Customer>();
            if (netService.IsConnected())
            {
                customers = await apiService.Get<Customer>("Customers");
                dataService.Save(customers);
            }
            else
            {
                customers = dataService.Get<Customer>(true);
            }
            ReloadCustomers(customers);

        }

        private void LoadLocalCustomers()
        {
            var customers = dataService.Get<Customer>(true);
            ReloadCustomers(customers);
        }

        public void SetCurrentCustomer(CustomerItemViewModel customerItemViewModel)
        {
            CurrentCustomer = customerItemViewModel;
        }

        public void SetGeolotation(string name, string address, double latitude, double longitude )
        {
            Pins.Clear();
            var position = new Position(latitude, longitude);
            var pin = new Pin
            {
                Type = PinType.Place,
                Position = position,
                Label = name,
                Address = address,
            };
            Pins.Add(pin);
        }
        #endregion

        #region LoadMenu
        private void LoadMenu()
        {
            Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_action_product.png",
                PageName = "ProductsPage",
                Title = "Productos",
            });

            Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_action_customer.png",
                PageName = "CustomersPage",
                Title = "Clientes",
            });
            Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_action_orders.png",
                PageName = "OrdersPage",
                Title = "Pedidos",
            });
            Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_action_delivery.png",
                PageName = "DeliveriesPage",
                Title = "Entregas",
            });
            Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_action_sync.png",
                PageName = "SyncPage",
                Title = "Sincronizar",
            });
            Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_action_settings.png",
                PageName = "SetupPage",
                Title = "Configuración",
            });
            Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_action_logout.png",
                PageName = "LogoutPage",
                Title = "Cerrar Sesion",
            });
        } 
        #endregion
    }
}
