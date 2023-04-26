using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Northwind_console.Model
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category not created, a Name is required")]
        public string CategoryName { get; set; }
        public string Description { get; set; }

        public string DisplayHeader()
        {
            String str = String.Format("{0,-15}{1,-25}{2,-55}","Category ID","Category Name","Category Description");
            return str;
        }

        public override string ToString()
        {
            String str = String.Format("{0,-15}{1,-25}{2,-55}",CategoryId,CategoryName,Description);
            return str;
        }

        public virtual ICollection<Product> Products { get; set; }
    }
}
