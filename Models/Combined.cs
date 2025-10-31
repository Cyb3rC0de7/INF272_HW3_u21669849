using System.Collections.Generic;

namespace u21669849_HW3.Models
{
    public class Combined
    {
        public List<staffs> Staffs { get; set; }
        public List<customers> Customers { get; set; }
        public List<products> Products { get; set; }
        public List<brands> Brands { get; set; }
        public List<categories> Categories { get; set; }
    }

    public class SalesReport
    {
        public string ProductName { get; set; }
        public string CustomerName { get; set; }
        public string StaffName { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class PopularProducts
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int OrderCount { get; set; }
        public int TotalQuantity { get; set; }
    }

    public class ReportFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
    }
}