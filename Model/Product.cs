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
            String line1 = String.Format("{0,-8}{1,-42}{2,-10}{3,-10}{4,-20}{5,-10}{6,-10}{7,-10}{8,-10}{9,-15}","","","Supplier","Category","Quantity","Unit","Units","Units","Reorder","");

            String line2 = String.Format("\n{0,-8}{1,-42}{2,-10}{3,-10}{4,-20}{5,-10}{6,-10}{7,-10}{8,-10}{9,-15}","ID","Product Name","ID","ID","Per Unit","Price","in Stock","on Order","Level","Discontinued");

            return line1+line2;
        }

        public override string ToString()
        {
            String str = String.Format("{0,-8}{1,-42}{2,-10}{3,-10}{4,-20}{5,-10}{6,-10}{7,-10}{8,-10}{9,-15}",ProductId,ProductName,SupplierId,CategoryId,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued);
            return str;
        }

        public virtual Category Category { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
