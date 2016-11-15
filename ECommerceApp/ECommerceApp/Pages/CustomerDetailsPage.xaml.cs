﻿using ECommerceApp.ViewModels;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace ECommerceApp.Pages
{
    public partial class CustomerDetailsPage : ContentPage
    {
        public CustomerDetailsPage()
        {
            InitializeComponent();

            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.SetGeolotation(
                mainViewModel.CurrentCustomer.FullName,
                mainViewModel.CurrentCustomer.Address,
                mainViewModel.CurrentCustomer.Latitude,
                mainViewModel.CurrentCustomer.Longitude);
            foreach (Pin item in mainViewModel.Pins)
            {
                MyMap.Pins.Add(item);
            }
            Locator();
        }

        private async void Locator()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            var location = await locator.GetPositionAsync(timeoutMilliseconds: 10000);
            var position = new Position(location.Latitude, location.Longitude);
            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMiles(.3)));
        }
    }
}
