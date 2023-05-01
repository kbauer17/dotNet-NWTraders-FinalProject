using System;
using System.Collections.Generic;

namespace Northwind_console.Model
{
    public partial class Supplier
    {
        public Supplier()
        {
            Products = new HashSet<Product>();
        }

        public int SupplierId { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }

        public string DisplayHeader()
        {
            String str = String.Format("{0,-15}{1,-25}","Supplier ID","Company Name");
            return str;
        }

        public override string ToString()
        {
            String str = String.Format("{0,-15}{1,-25}",SupplierId,CompanyName);
            return str;
        }


        public virtual ICollection<Product> Products { get; set; }
    }
}
