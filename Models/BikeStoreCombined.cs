using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u21669849_HW3.Models
{
    public class BikeStoreCombined
    {
        public IEnumerable<staffs> Staff { get; set; }
        public IEnumerable<customers> Customers { get; set; }
        public IEnumerable<products> Products { get; set; }
        public IEnumerable<brands> Brands { get; set; }
        public IEnumerable<categories> Categories { get; set; }
        public IEnumerable<orders> Orders { get; set; }
        public IEnumerable<order_items> OrderItems { get; set; }
        public IEnumerable<stores> Stores { get; set; }
    }
}