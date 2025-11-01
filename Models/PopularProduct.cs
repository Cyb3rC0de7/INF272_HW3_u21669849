using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u21669849_HW3.Models
{
    public class PopularProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int OrderCount { get; set; }
        public int TotalQuantity { get; set; }
    }
}