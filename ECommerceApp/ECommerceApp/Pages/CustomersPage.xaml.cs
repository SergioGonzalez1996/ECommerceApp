using ECommerceApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ECommerceApp.Pages
{
    public partial class CustomersPage : ContentPage
    {
        public CustomersPage()
        {
            InitializeComponent();

            var main = (MainViewModel)this.BindingContext;
            base.Appearing += (object sender, EventArgs e) =>
            {
                main.RefreshCustomersCommand.Execute(this);
            };

        }
    }
}
