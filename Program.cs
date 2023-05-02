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
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.DarkGray;
logger.Info("Program started");
    Console.ForegroundColor = ConsoleColor.Black;
    Console.WriteLine();

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
                        Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Enter the Category Description:");
                        Console.ForegroundColor = ConsoleColor.Black;
                    CreateCategory.Description = Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        logger.Info("Validation passed");
                    // save category to db
                    db.AddCategory(CreateCategory);
                        logger.Info(" Category added - {name}",CreateCategory.CategoryName);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                }
        }        
        else if (choice == "3") //Display a Category and its active products
        {
                Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nSelect by CategoryId the Category whose active products you want to display:");
                Console.ForegroundColor = ConsoleColor.Black;
            DisplayCategories(db);


                // obtain user selection of which category to display
            if(int.TryParse(Console.ReadLine(),out int id)) // if not an actual number, breaks out
            {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    logger.Info($"CategoryId {id} selected");


                    // create an instance using the number entered but checking to see if that number actually is an id number
                    // if it is not, the instance is not created
                Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);

                if(category != null) //if the instance was created, do these statements
                {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"{category.CategoryName} - {category.Description}");
                    foreach (Product p in category.Products)
                    {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                        if(p.Discontinued == false)
                            Console.WriteLine($"  {p.ProductName}");
                    }
                        Console.ForegroundColor = ConsoleColor.Black; // completes this section and returns to menu
                }
            }else{
                // jumps here if the user entered either non-numeric or a numeric which does not match a current id
                Console.ForegroundColor = ConsoleColor.DarkRed;
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
                    Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{item.CategoryName}");
                    Console.ForegroundColor = ConsoleColor.Magenta;
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
                Console.ForegroundColor = ConsoleColor.Green;
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
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                        logger.Info($"Product (id: {editCategory.CategoryId}) updated");
                            Console.ForegroundColor = ConsoleColor.Black;
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
                Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Select the product you want to display:");
            DisplayProducts(db);
                Console.ForegroundColor = ConsoleColor.Black;

               // obtain user selection of which category to display
            if(int.TryParse(Console.ReadLine(),out int id)) // if not an actual number, breaks out
            {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    logger.Info($"ProductId {id} selected");
                        Console.ForegroundColor = ConsoleColor.Cyan;

                    // create an instance using the number entered but checking to see if that number actually is an id number
                    // if it is not, the instance is not created
                Product showProduct = db.Products.FirstOrDefault(p => p.ProductId == id);

                if(showProduct != null) //if the instance was created, do these statements
                {
                    //Console.WriteLine($"{product.ProductId}:  {product.ProductName}");
                    Console.WriteLine(showProduct.DisplayHeader());
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(showProduct);
                        Console.ForegroundColor = ConsoleColor.Black; // completes this section and returns to menu
                }
            }else{
                // jumps here if the user entered either non-numeric or a numeric which does not match a current id
                Console.ForegroundColor = ConsoleColor.DarkRed;
            logger.Error("Invalid Input");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Black;
            }
        }
        else if (choice == "7") //Display All Products; user select all, active, or discontinued
        {
            do
            {
                    Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Please select which products to display:");
                    Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("1: All Products\n2: Active Products Only\n3: Discontinued Products\n\"c\" to cancel");
            
                choice = Console.ReadLine();
    
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                logger.Info($"Option {choice} selected");
                    Console.ForegroundColor = ConsoleColor.Black;
                
                switch(choice){
                    case "1":
                        var query = db.Products.OrderBy(p => p.ProductId);

                            Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{query.Count()} records returned - All Products");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.ProductId} - {item.ProductName}");
                        }
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine();
                        break;

                    case "2":
                        var query2 = db.Products.OrderBy(p => p.ProductId).Where(p => p.Discontinued == false);

                            Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{query2.Count()} records returned - Active Products Only");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query2)
                        {
                            Console.WriteLine($"{item.ProductId} - {item.ProductName}");
                        }
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine();
                        break;

                    case "3":
                        var query3 = db.Products.OrderBy(p => p.ProductId).Where(p => p.Discontinued == true);

                            Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{query3.Count()} records returned - Discontinued Products Only");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query3)
                        {
                            Console.WriteLine($"{item.ProductId} - {item.ProductName}");
                        }
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine();
                        break;

                    case "c":
                    case "C":
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Exiting this menu");
                            Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine();
                        break;

                    default:
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                        logger.Error("Invalid selection");
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.WriteLine();
                        break;
                }
            } while (choice.ToLower() != "c");
        }
        else if (choice == "8") //Add Product
        {
            Product newProduct = InputProduct(db, logger);
            if(newProduct != null)
            {
                // obtain user input for Supplier of new product
                Supplier selectedSupplier = SelectSupplier(db, logger);

                // if supplier selection successful, continue obtaining user input and populating fields
                if(selectedSupplier != null) 
                {
                    newProduct.SupplierId = selectedSupplier.SupplierId;

                    // obtain category Id
                    Category selectedCategory = SelectCategory(db,logger);

                    // if category selection successful, continue obtaining user input and populating fields
                    if(selectedCategory != null)
                    {
                        newProduct.CategoryId = selectedCategory.CategoryId;

                        // obtain Quantity Per Unit info (all other fields provide default values)
                        newProduct.QuantityPerUnit = InputQtyPerUnit(db,logger);

                        if(newProduct.QuantityPerUnit != null)
                        {
                                // add the new product record to the Products table
                            db.AddProduct(newProduct);
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                logger.Info("Default values will be applied for remaining fields for {name}",newProduct.ProductName);
                                logger.Info("Product added - {name}",newProduct.ProductName);
                                    Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else  // if Quantity per Unit entry unsuccessful, display info of it and create record (field not required but does not have a default)
                        {
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                logger.Info("Quantity per Unit field is empty (value not required, no default value is provided)");
                                    Console.ForegroundColor = ConsoleColor.Black;

                                // add the new product record to the Products table
                            db.AddProduct(newProduct);
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                logger.Info("Default values will be applied for remaining fields for {name}",newProduct.ProductName);
                                logger.Info("Product added - {name}",newProduct.ProductName);
                                    Console.ForegroundColor = ConsoleColor.Black;
                        }
                    }
                    else  // if category selection unsuccessful, display error and return to main menu
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    logger.Error("Invalid input for CategoryId");
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                }
                else    // if supplier selection unsuccessful, display error and return to main menu
                {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    logger.Error("Invalid input for SupplierId");
                        Console.ForegroundColor = ConsoleColor.Black;
                }
            }
        }
        else if (choice == "9") //Edit a Product
        {
                Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Choose a product to edit:");
                Console.ForegroundColor = ConsoleColor.Black;
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
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                            logger.Info($"Product (id: {editProduct.ProductId}) updated");
                                Console.ForegroundColor = ConsoleColor.Black;
                    }
                }else {
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
        else if (choice == "10") //Delete a Product
        {
                Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Choose a product to delete:");
                Console.ForegroundColor = ConsoleColor.Black;
            DisplayProducts(db);
            
            if (int.TryParse(Console.ReadLine(), out int ProductId))
            {
                Product deleteProduct = db.Products.FirstOrDefault(p => p.ProductId == ProductId);
                if(deleteProduct != null)
                {
                    db.DeleteProduct(deleteProduct);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    logger.Info($"Product (id: {deleteProduct.ProductId}) deleted");
                        Console.ForegroundColor = ConsoleColor.Black;
                }else {
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
      
        Console.WriteLine();

    } while (choice.ToLower() != "q");
}
catch (Exception ex)
{
        Console.ForegroundColor = ConsoleColor.DarkRed;
    logger.Error(ex.Message);
        Console.ForegroundColor = ConsoleColor.Black;
}

    Console.ForegroundColor = ConsoleColor.DarkGray;
logger.Info("Program ended");
    Console.ForegroundColor = ConsoleColor.Black;


static Product InputProduct(NWConsole_23_kjbContext db, Logger logger)
{
    Product product = new Product();
        Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Enter the Product name");
        Console.ForegroundColor = ConsoleColor.Black;
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
        Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Enter the Category name");
        Console.ForegroundColor = ConsoleColor.Black;
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
            Console.ForegroundColor = ConsoleColor.DarkRed;
            logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
        }
    }
    return null;
}

static String InputQtyPerUnit(NWConsole_23_kjbContext db, Logger logger)
{
    Product product = new Product();
        Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Enter the Quantity per Unit: ");
    Console.WriteLine("Example entries:\t24 pieces\n\t\t\t24 - 355 ml bottles\n\t\t\t5 kg pkg\n\t\t\t10 pkgs");
        Console.ForegroundColor = ConsoleColor.Black;
    product.QuantityPerUnit = Console.ReadLine();

    ValidationContext context = new ValidationContext(product, null, null);
    List<ValidationResult> results = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(product, context, results, true);
    if (isValid)
    {
        // prevent exceeding max characters allowed
        if (product.QuantityPerUnit.Length > 20){
            // generate validation error if the name exceeds the maximum number of characters allowed
            isValid = false;
            results.Add(new ValidationResult("Entry exceeds maximum 20 characters", new string[] { "QuantityPerUnit" }));
        } else if (product.QuantityPerUnit == "")
        {
            // generate validation warning that the field will be blank
            isValid = false;
            results.Add(new ValidationResult("Entry is blank", new string[] { "QuantityPerUnit" }));
        }
        else 
        {
            return product.QuantityPerUnit;
        }
    }
     foreach (var result in results)
    {
        logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
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

        // display selected product and all of its fields
    Product product = new Product();
    Console.WriteLine("ID   Product Name");
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

static Supplier SelectSupplier(NWConsole_23_kjbContext db, Logger logger)
{
        // obtain user input
        Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\nEnter the Supplier ID:"); 
    DisplaySuppliers(db);

        //check for actual number entry
    if(Int32.TryParse(Console.ReadLine(),out Int32 supId)) 
    {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        logger.Info($"SupplierId {supId} selected");
            Console.ForegroundColor = ConsoleColor.Black;
        
        Supplier supplier = db.Suppliers.FirstOrDefault(s => s.SupplierId == supId);
        if(supplier != null) // if the entered id matches an existing id, execute these statements
        {
            return supplier;
        }
        else
        {
            return null;
        }
    } 
    else 
    {
        return null;
    }
}

static Category SelectCategory(NWConsole_23_kjbContext db, Logger logger)
{
        // obtain user input
        Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\nEnter the Category ID:"); 
    DisplayCategories(db);

        //check for actual number entry
    if(Int32.TryParse(Console.ReadLine(),out Int32 catId)) 
    {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        logger.Info($"SupplierId {catId} selected");
            Console.ForegroundColor = ConsoleColor.Black;
        
        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == catId);
        if(category != null) // if the entered id matches an existing id, execute these statements
        {
            return category;
        }
        else
        {
            return null;
        }
    } 
    else 
    {
        return null;
    }
}
