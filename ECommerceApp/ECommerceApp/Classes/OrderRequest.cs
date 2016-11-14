using System;
using System.Collections.Generic;

namespace ECommerceApp.Classes
{
    public class OrderRequest
    {
        public string UserName { get; set; }

        public int CompanyId { get; set; }

        public int CustomerId { get; set; }

        public DateTime Date { get; set; }

        public string Remarks { get; set; }

        public List<OrderDetailRequest> Details { get; set; }
    }
}
