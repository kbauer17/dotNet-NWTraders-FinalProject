using NLog;
using System.Linq;
using Northwind_console.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory() + "\\nlog.config";

// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
logger.Info("Program started");

try
{
    var db = new NWConsole_23_kjbContext();
    string choice;
    do
    {
        Console.WriteLine("1) Display Categories");
        Console.WriteLine("2) Add Category");
        Console.WriteLine("3) Display Category and related products");
        Console.WriteLine("4) Display all Categories and their related products");
        Console.WriteLine("5) Edit a Category Name");
        Console.WriteLine("6) Display a Product");
        Console.WriteLine("7) Display All Products");
        Console.WriteLine("8) Add Product");
        Console.WriteLine("9) Edit a Product Name");
        Console.WriteLine("\"q\" to quit");
        choice = Console.ReadLine();
        Console.Clear();
        logger.Info($"Option {choice} selected");
        if (choice == "1")  //Display Categories
        {
            var query = db.Categories.OrderBy(p => p.CategoryName);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{query.Count()} records returned");
            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryName} - {item.Description}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
                else if (choice == "2") //Add Category
        {
            Category category = new Category();
            Console.WriteLine("Enter Category Name:");
            category.CategoryName = Console.ReadLine();
            Console.WriteLine("Enter the Category Description:");
            category.Description = Console.ReadLine();
            ValidationContext context = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                }
                else
                {
                    logger.Info("Validation passed");
                    // save category to db
                    db.AddCategory(category);
                    logger.Info(" Category added - {name}",category.CategoryName);
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
        }        else if (choice == "3")    //Display Category and related active products
        {
            var query = db.Categories.OrderBy(p => p.CategoryId);

            Console.WriteLine("Select the category whose products you want to display:");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
            int id = int.Parse(Console.ReadLine());
            Console.Clear();
            logger.Info($"CategoryId {id} selected");
            Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
            Console.WriteLine($"{category.CategoryName} - {category.Description}");
            foreach (Product p in category.Products)
            {
                if(p.Discontinued == false)
                    Console.WriteLine($"\t{p.ProductName}");
            }
        }
                else if (choice == "4") //Display all Categories and their related active products
        {
            var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryName}");
                foreach (Product p in item.Products)
                {
                    if(p.Discontinued == false)
                    Console.WriteLine($"\t{p.ProductName}");
                }
            }
        }
                else if (choice == "5") //Edit a Category Name
        {
            
        }
                else if (choice == "6") //Display a Product and all of its fields
        {
            var query = db.Products.OrderBy(p => p.ProductId);

            Console.WriteLine("Select the product you want to display:");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.ProductId}) {item.ProductName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
            int id = int.Parse(Console.ReadLine());
            Console.Clear();
            logger.Info($"ProductId {id} selected");
                // display selected product and all of its fields
            Product showProduct = db.Products.FirstOrDefault(p=>p.ProductId==id);
            Console.WriteLine(showProduct.DisplayHeader());
            Console.WriteLine(showProduct);

        }
                else if (choice == "7") //Display All Products; user select all, active, or discontinued
        {
            Console.WriteLine("Please select which products to display: \n1: All Products\n2: All Active Products\n3: Discontinued Products");
            choice = Console.ReadLine();
            Console.Clear();
            logger.Info($"Option {choice} selected");
            if(choice == "1")
            {
                var query = db.Products.OrderBy(p => p.ProductId);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{query.Count()} records returned");
                Console.ForegroundColor = ConsoleColor.Magenta;
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.ProductId} - {item.ProductName}");
                }
                Console.ForegroundColor = ConsoleColor.Black;
            } else if (choice == "2")
            {
                var query = db.Products.OrderBy(p => p.ProductId).Where(p => p.Discontinued == false);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{query.Count()} records returned");
                Console.ForegroundColor = ConsoleColor.Magenta;
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.ProductId} - {item.ProductName}");
                }
                Console.ForegroundColor = ConsoleColor.Black;
            } else if (choice == "3")
            {
                var query = db.Products.OrderBy(p => p.ProductId).Where(p => p.Discontinued == true);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{query.Count()} records returned");
                Console.ForegroundColor = ConsoleColor.Magenta;
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.ProductId} - {item.ProductName}");
                }
                Console.ForegroundColor = ConsoleColor.Black;
            }

        }
                else if (choice == "8") //Add Product
        {
            Product newProduct = InputProduct(db, logger);
            if(newProduct != null)
            {
                // auto-generate the new ProductID
            var query = db.Products.OrderByDescending(p => p.ProductId).FirstOrDefault();
            newProduct.ProductId=query.ProductId+1;

                // obtain user input for all other fields
            Console.Write("\nEnter the Supplier ID>>  ");
                newProduct.SupplierId=Convert.ToInt32(Console.ReadLine());
            Console.Write("\nEnter the Category ID>>  ");
                newProduct.CategoryId=Convert.ToInt32(Console.ReadLine());
            Console.Write("\nEnter the Quantity per Unit>>  ");
                newProduct.QuantityPerUnit=Console.ReadLine();
            Console.Write("\nEnter the Unit Price>>  ");
                newProduct.UnitPrice=Convert.ToDecimal(Console.ReadLine());
            Console.Write("\nEnter the Units in Stock>>  ");
                newProduct.UnitsInStock=Convert.ToInt16(Console.ReadLine());
            Console.Write("\nEnter the Units on Order>>  ");
                newProduct.UnitsOnOrder=Convert.ToInt16(Console.ReadLine());
            Console.Write("\nEnter the Reorder Level>>  ");
                newProduct.ReorderLevel=Convert.ToInt16(Console.ReadLine());

            Console.WriteLine(newProduct.DisplayHeader());
            Console.WriteLine(newProduct);

                db.AddProduct(newProduct);
                logger.Info("Product added - {name}",newProduct.ProductName);
            }


        }
                else if (choice == "9") //Edit a Product
        {
            Console.WriteLine("Choose a product to edit:");
            var products = db.Products.OrderBy(p => p.ProductId);
            foreach (var item in products)
            {
                Console.WriteLine($"{item.ProductId}: {item.ProductName}");
            }
            if (int.TryParse(Console.ReadLine(), out int ProductId))
            {
                Product editProduct = db.Products.FirstOrDefault(p => p.ProductId == ProductId);
                if(editProduct != null)
                {
                    Product UpdatedProduct = InputProduct(db, logger);
                    if(UpdatedProduct != null)
                    {
                        UpdatedProduct.ProductId = editProduct.ProductId;
                        db.EditProduct(UpdatedProduct);
                        logger.Info($"Product (id: {editProduct.ProductId}) updated");
                    }
                }
            } else {
                logger.Error("Invalid Product ID");
            }

        }
        
        Console.WriteLine();

    } while (choice.ToLower() != "q");
}
catch (Exception ex)
{
    logger.Error(ex.Message);
}

logger.Info("Program ended");


static Product InputProduct(NWConsole_23_kjbContext db, Logger logger)
{
    Product product = new Product();
    Console.WriteLine("Enter the Product name");
    product.ProductName = Console.ReadLine();

    ValidationContext context = new ValidationContext(product, null, null);
    List<ValidationResult> results = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(product, context, results, true);
    if (isValid)
    {
        // prevent duplicate product names
        if (db.Products.Any(p => p.ProductName == product.ProductName)) {
            // generate error
             results.Add(new ValidationResult("Product name exists", new string[] { "Name" }));
        } else {
            return product;
        }
    }
     foreach (var result in results)
    {
        logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
    }
    return null;
}
