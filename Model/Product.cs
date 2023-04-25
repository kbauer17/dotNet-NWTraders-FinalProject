using System;
using System.Collections.Generic;

namespace Northwind_console.Model
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int? SupplierId { get; set; }
        public int? CategoryId { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        public string DisplayHeader()
        {
            String str = String.Format("{0,-15}{1,-45}{2,-15}{3,-15}{4,-20}{5,-15}{6,-20}{7,-20}{8,-15}{9,-15}","Product ID","Product Name","Supplier ID","Category ID","Quantity Per Unit","Unit Price","Units in Stock","Units on Order","Reorder Level","Discontinued");
            return str;
        }

        public override string ToString()
        {
            String str = String.Format("{0,-15}{1,-45}{2,-15}{3,-15}{4,-20}{5,-15}{6,-20}{7,-20}{8,-15}{9,-15}",ProductId,ProductName,SupplierId,CategoryId,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued);
            return str;
        }

        public virtual Category Category { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
