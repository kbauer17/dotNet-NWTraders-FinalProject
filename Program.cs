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
        UserMenu();
        choice = Console.ReadLine();
            //Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            logger.Info($"Option {choice} selected");
            Console.ForegroundColor = ConsoleColor.Black;
        
        if (choice == "1")      //Display Categories
        {
            DisplayCategories(db);
                Console.ForegroundColor = ConsoleColor.Black;
        }
        else if (choice == "2") //Add Category
        {
            // obtain and validate the category name
            Category CreateCategory = InputCategory(db, logger);
                if(CreateCategory != null)
                {
                    // Name entered is valid, enter a category description
                    Console.WriteLine("Enter the Category Description:");
                    CreateCategory.Description = Console.ReadLine();
                        logger.Info("Validation passed");
                    // save category to db
                    db.AddCategory(CreateCategory);
                    logger.Info(" Category added - {name}",CreateCategory.CategoryName);
                }
        }        
        else if (choice == "3") //Display a Category and its active products
        {
            Console.WriteLine("\nSelect by CategoryId the Category whose active products you want to display:");
            DisplayCategories(db);
                Console.ForegroundColor = ConsoleColor.DarkGray;

                // obtain user selection of which category to display
            if(int.TryParse(Console.ReadLine(),out int id)) // if not an actual number, breaks out
            {
                    logger.Info($"CategoryId {id} selected");
                    Console.ForegroundColor = ConsoleColor.Magenta;

                    // create an instance using the number entered but checking to see if that number actually is an id number
                    // if it is not, the instance is not created
                Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);

                if(category != null) //if the instance was created, do these statements
                {
                    Console.WriteLine($"{category.CategoryName} - {category.Description}");
                    foreach (Product p in category.Products)
                    {
                        if(p.Discontinued == false)
                            Console.WriteLine($"  {p.ProductName}");
                    }
                        Console.ForegroundColor = ConsoleColor.Black; // completes this section and returns to menu
                }
            }else{
                // jumps here if the user entered either non-numeric or a numeric which does not match a current id
            logger.Error("Invalid Id");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Black;
            }
        }
        else if (choice == "4") //Display all Categories with their active products
        {
                Console.WriteLine();
                
            var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
            foreach (var item in query)
            {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"{item.CategoryName}");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                foreach (Product p in item.Products)
                {
                    if(p.Discontinued == false)
                    Console.WriteLine($"  {p.ProductName}");
                }
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Black;
        }
        else if (choice == "5") //Edit a Category Name
        {
            Console.WriteLine("Enter the ID of the category name to edit:");
            DisplayCategories(db);

            if (int.TryParse(Console.ReadLine(), out int CategoryId))
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                logger.Info($"CategoryId {CategoryId} selected");
                Console.ForegroundColor = ConsoleColor.Green;
                
                Category editCategory = db.Categories.FirstOrDefault(c => c.CategoryId == CategoryId);
                if(editCategory != null)
                {
                    Category UpdatedCategory = InputCategory(db, logger);
                    if(UpdatedCategory != null)
                    {
                        UpdatedCategory.CategoryId = editCategory.CategoryId;
                        db.EditCategory(UpdatedCategory);
                        logger.Info($"Product (id: {editCategory.CategoryId}) updated");
                    }
                } else {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    logger.Error("Invalid ID number entered");
                    Console.ForegroundColor = ConsoleColor.Black;
                }
            } else {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                logger.Error("Invalid input");
                Console.ForegroundColor = ConsoleColor.Black;
            }
           
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

               // obtain user selection of which category to display
            if(int.TryParse(Console.ReadLine(),out int id)) // if not an actual number, breaks out
            {
                    logger.Info($"ProductId {id} selected");
                    Console.ForegroundColor = ConsoleColor.Magenta;

                    // create an instance using the number entered but checking to see if that number actually is an id number
                    // if it is not, the instance is not created
                Product showProduct = db.Products.FirstOrDefault(p => p.ProductId == id);

                if(showProduct != null) //if the instance was created, do these statements
                {
                    //Console.WriteLine($"{product.ProductId}:  {product.ProductName}");
                    Console.WriteLine(showProduct.DisplayHeader());
                    Console.WriteLine(showProduct);
                        Console.ForegroundColor = ConsoleColor.Black; // completes this section and returns to menu
                }
            }else{
                // jumps here if the user entered either non-numeric or a numeric which does not match a current id
            logger.Error("Invalid Input");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Black;
            }
        }
        else if (choice == "7") //Display All Products; user select all, active, or discontinued
        {
            Console.WriteLine("Please select which products to display: \n1: All Products\n2: All Active Products\n3: Discontinued Products");
            choice = Console.ReadLine();
            //Console.Clear();
            logger.Info($"Option {choice} selected");
            if(choice == "1")
            {
                var query = db.Products.OrderBy(p => p.ProductId);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{query.Count()} records returned - All Products");
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
                Console.WriteLine($"{query.Count()} records returned - Active Products Only");
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
                Console.WriteLine($"{query.Count()} records returned - Discontinued Products Only");
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

                // obtain user input for all fields
            Console.WriteLine("\nEnter the Supplier ID:"); 
            DisplaySuppliers(db);
                newProduct.SupplierId=Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("\nEnter the Category ID:"); 
            DisplayCategories(db);
                newProduct.CategoryId=Convert.ToInt32(Console.ReadLine());
            
            /*
            Console.Write("\nEnter the Quantity per Unit>>  "); // no default value, max length 20
                newProduct.QuantityPerUnit=Console.ReadLine();
            Console.Write("\nEnter the Unit Price>>  "); // has default value
                newProduct.UnitPrice=Convert.ToDecimal(Console.ReadLine());
            Console.Write("\nEnter the Units in Stock>>  "); // has default value
                newProduct.UnitsInStock=Convert.ToInt16(Console.ReadLine());
            Console.Write("\nEnter the Units on Order>>  "); // has default value
                newProduct.UnitsOnOrder=Convert.ToInt16(Console.ReadLine());
            Console.Write("\nEnter the Reorder Level>>  "); // has default value
                newProduct.ReorderLevel=Convert.ToInt16(Console.ReadLine());                */

            Console.WriteLine(newProduct.DisplayHeader());
            Console.WriteLine(newProduct);

                db.AddProduct(newProduct);
                logger.Info("Product added - {name}",newProduct.ProductName);
            }


        }
        else if (choice == "9") //Edit a Product
        {
            Console.WriteLine("Choose a product to edit:");
            DisplayProducts(db);
            
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
        else if (choice == "10") //Delete a Product
        {
            Console.WriteLine("Choose a product to delete:");
            DisplayProducts(db);
            
            if (int.TryParse(Console.ReadLine(), out int ProductId))
            {
                Product deleteProduct = db.Products.FirstOrDefault(p => p.ProductId == ProductId);
                if(deleteProduct != null)
                {
                    db.DeleteProduct(deleteProduct);
                    logger.Info($"Product (id: {deleteProduct.ProductId}) deleted");
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
            // generate validation error if the name is a duplicat
            isValid = false;
            results.Add(new ValidationResult("Product name exists", new string[] { "Name" }));
        } else if(product.ProductName.Length > 40){
            // generate validation error if the name exceeds the maximum number of characters allowed
            isValid = false;
            results.Add(new ValidationResult("Product name length exceeds maximum 40 characters", new string[] { "Name" }));
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

static Category InputCategory(NWConsole_23_kjbContext db, Logger logger)
{
    Category category = new Category();
    Console.WriteLine("Enter the Category name");
    category.CategoryName = Console.ReadLine();

    ValidationContext context = new ValidationContext(category, null, null);
    List<ValidationResult> results = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(category, context, results, true);
    if (isValid)
    {
        // prevent duplicate category names
        if (db.Categories.Any(c => c.CategoryName == category.CategoryName)) {
            // generate error
            isValid = false;
            results.Add(new ValidationResult("Category name exists", new string[] { "Name" }));
        } else if(category.CategoryName.Length > 15){
            // generate error
            isValid = false;
            results.Add(new ValidationResult("Category name length exceeds maximum 15 characters", new string[] { "Name" }));
        } else {
            return category;
        }
    }
    if (!isValid)
    {
        foreach (var result in results)
        {
            logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
        }
    }
    return null;
}

static void UserMenu(){
        Console.ForegroundColor = ConsoleColor.Black;
    Console.WriteLine("1) Display Categories");
    Console.WriteLine("2) Add Category");
    Console.WriteLine("3) Display a Category with its active products");
    Console.WriteLine("4) Display all Categories with their active products");
    Console.WriteLine("5) Edit a Category Name");
    Console.WriteLine("6) Display a Product");
    Console.WriteLine("7) Display All Products");
    Console.WriteLine("8) Add Product");
    Console.WriteLine("9) Edit a Product Name");
    Console.WriteLine("10) Delete a Product");
    Console.WriteLine("\"q\" to quit");
}

static void DisplayCategories(NWConsole_23_kjbContext db){
    var query = db.Categories.OrderBy(c => c.CategoryId);
        Console.ForegroundColor = ConsoleColor.DarkCyan;

        // display selected category and all of its fields
    Category category = new Category();
    Console.WriteLine(category.DisplayHeader());
        Console.ForegroundColor = ConsoleColor.Magenta;

    foreach (var item in query)
    {
        Console.WriteLine(item.ToString());
    }
}

static void DisplayProducts(NWConsole_23_kjbContext db){
    var query = db.Products.OrderBy(p => p.ProductId);
        Console.ForegroundColor = ConsoleColor.DarkCyan;

        // display selected category and all of its fields
    Product product = new Product();
    Console.WriteLine($"{0,-5}{1,-42}","Product ID","Product Name");
        Console.ForegroundColor = ConsoleColor.Magenta;

    foreach (var item in query)
    {
        //Console.WriteLine(item.ToString());
        Console.WriteLine($"{item.ProductId}: {item.ProductName}");
    }
}

static void DisplaySuppliers(NWConsole_23_kjbContext db){
    var query = db.Suppliers.OrderBy(s => s.SupplierId);
        Console.ForegroundColor = ConsoleColor.DarkCyan;

        // display selected category and all of its fields
    Supplier supplier = new Supplier();
    Console.WriteLine(supplier.DisplayHeader());
        Console.ForegroundColor = ConsoleColor.Magenta;

    foreach (var item in query)
    {
        Console.WriteLine(item.ToString());
    }
}
