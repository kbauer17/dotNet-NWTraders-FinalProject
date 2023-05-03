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
        else if (choice == "6") //Delete a Category
        {
            DeleteCategory(db,logger);
        }
        else if (choice == "7") //Display a Product and all of its fields
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
        else if (choice == "8") //Display All Products; user select all, active, or discontinued
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
        else if (choice == "9") //Add Product
        {
            AddProduct(db,logger);
        }
        else if (choice == "10") //Edit a Product
        {
            EditProduct(db,logger);
        }
        else if (choice == "11") //Delete a Product
        {
            DeleteProduct(db,logger);
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


static Product InputProductName(NWConsole_23_kjbContext db, Logger logger)
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
        } else if(product.ProductName == ""){
            // generate validation error if the name is blank
            isValid = false;
            results.Add(new ValidationResult("Product name should not be blank", new string[] { "Name" }));
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

static decimal InputUnitPrice(NWConsole_23_kjbContext db, Logger logger)
{
    Product product = new Product();
        // obtain user input
        Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\nEnter the unit price:"); 
    
        //check for actual number entry
    if(decimal.TryParse(Console.ReadLine(),out decimal price)) 
    {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        logger.Info($"Unit price of {price} entered");
            Console.ForegroundColor = ConsoleColor.Black;
        product.UnitPrice = price;
        return (decimal)product.UnitPrice;
    }
    else
    {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        logger.Info($"Invalid entry for Unit price;  default value will be used");
            Console.ForegroundColor = ConsoleColor.Black;
        return 0;
    } 

}

static short InputUnitsInStock(NWConsole_23_kjbContext db, Logger logger)
{
    Product product = new Product();
        // obtain user input
        Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\nEnter the number of units in stock:"); 
    
        //check for actual number entry
    if(short.TryParse(Console.ReadLine(),out short unitsInStock)) 
    {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        logger.Info($"Unit price of {unitsInStock} entered");
            Console.ForegroundColor = ConsoleColor.Black;
        product.UnitsInStock = unitsInStock;
        return (short)product.UnitsInStock;
    }
    else
    {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        logger.Info($"Invalid entry for units in stock;  default value will be used");
            Console.ForegroundColor = ConsoleColor.Black;
        return 0;
    } 

}

static short InputUnitsOnOrder(NWConsole_23_kjbContext db, Logger logger)
{
    Product product = new Product();
        // obtain user input
        Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\nEnter the number of units on order:"); 
    
        //check for actual number entry
    if(short.TryParse(Console.ReadLine(),out short unitsOnOrder)) 
    {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        logger.Info($"Unit price of {unitsOnOrder} entered");
            Console.ForegroundColor = ConsoleColor.Black;
        product.UnitsOnOrder = unitsOnOrder;
        return (short)product.UnitsOnOrder;
    }
    else
    {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        logger.Info($"Invalid entry for units on order;  default value will be used");
            Console.ForegroundColor = ConsoleColor.Black;
        return 0;
    } 

}

static short InputReorderLevel(NWConsole_23_kjbContext db, Logger logger)
{
    Product product = new Product();
        // obtain user input
        Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\nEnter the reorder level:"); 
    
        //check for actual number entry
    if(short.TryParse(Console.ReadLine(),out short reorder)) 
    {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        logger.Info($"Unit price of {reorder} entered");
            Console.ForegroundColor = ConsoleColor.Black;
        product.ReorderLevel = reorder;
        return (short)product.ReorderLevel;
    }
    else
    {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        logger.Info($"Invalid entry for reorder level;  default value will be used");
            Console.ForegroundColor = ConsoleColor.Black;
        return 0;
    } 

}

static void UserMenu(){
        Console.ForegroundColor = ConsoleColor.Black;
    Console.WriteLine("1) Display Categories");
    Console.WriteLine("2) Add a Category");
    Console.WriteLine("3) Display a Category with its active products");
    Console.WriteLine("4) Display all Categories with their active products");
    Console.WriteLine("5) Edit a Category Name");
    Console.WriteLine("6) Delete a Category");
    Console.WriteLine("7) Display a Product");
    Console.WriteLine("8) Display all Products");
    Console.WriteLine("9) Add a Product");
    Console.WriteLine("10) Edit a Product");
    Console.WriteLine("11) Delete a Product");
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

static void AddProduct(NWConsole_23_kjbContext db, Logger logger)
{
    Product newProduct = InputProductName(db, logger);
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

static void EditProduct(NWConsole_23_kjbContext db, Logger logger)
{
            Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Choose a product to edit:");
        Console.ForegroundColor = ConsoleColor.Black;
    DisplayProducts(db);
        // verify selection of a product Id
    if (int.TryParse(Console.ReadLine(), out int ProductId))
    {
        Product editProduct = db.Products.FirstOrDefault(p => p.ProductId == ProductId);
        if(editProduct != null)
        {
            string userInput;
            do
            {
                // display selected product current data
                    Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(editProduct.DisplayHeader());
                    Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(editProduct);
                Console.WriteLine();
                
                // provide menu of fields to edit
                    Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Select a field to edit:\n1: Product Name\n2: Supplier Id\n3: Quantity per Unit\n4: Unit Price\n5: Units in Stock\n6: Units on Order\n7: Reorder Level\n8: Discontinued status\n\"x\" to exit this menu");
                userInput = Console.ReadLine();

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                logger.Info($"Option {userInput} selected");
                    Console.ForegroundColor = ConsoleColor.Black;
                
                switch(userInput){
                    case "1":       // update Product Name
                        Product UpdatedProduct = InputProductName(db, logger);
                        if(UpdatedProduct != null)
                        {
                            UpdatedProduct.ProductId = editProduct.ProductId;
                            db.EditProduct(UpdatedProduct);
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                logger.Info($"Product (id: {editProduct.ProductId}) updated");
                                    Console.ForegroundColor = ConsoleColor.Black;
                        }
                        break;

                    case "2":       // update Supplier Id
                        Supplier selectedSupplier = SelectSupplier(db, logger);

                        // if supplier selection successful, continue obtaining user input and populating fields
                        if(selectedSupplier != null) 
                        {
                            editProduct.SupplierId = selectedSupplier.SupplierId;
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                logger.Info($"Product (id: {editProduct.ProductId}) updated");
                                    Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else    // if supplier selection unsuccessful, display error and return to main menu
                        {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                            logger.Error("Invalid input for SupplierId");
                                Console.ForegroundColor = ConsoleColor.Black;
                        }
                        break;

                    /*
                    case "3":       // update Category Id - removed as CategoryID cannot be editted
                        Category selectedCategory = SelectCategory(db, logger);

                        // if category selection successful, continue obtaining user input and populating fields
                        if(selectedCategory != null) 
                        {
                                editProduct.CategoryId = selectedCategory.CategoryId;
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                logger.Info($"Product id: {editProduct.ProductId} updated");
                                    Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else    // if category selection unsuccessful, display error and return to main menu
                        {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                            logger.Error("Invalid input for Category Id");
                                Console.ForegroundColor = ConsoleColor.Black;
                        }
                        break;                                                              */

                    case "3":       // update Quantity Per Unit
                        editProduct.QuantityPerUnit = InputQtyPerUnit(db,logger);

                        if(editProduct.QuantityPerUnit != null) // if category selection successful, continue obtaining user input and populating fields
                        {
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                            logger.Info($"Product (id: {editProduct.ProductId}) updated");
                                Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else  // if Quantity per Unit entry unsuccessful, display error message and loop to try again
                        {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                            logger.Error("Invalid input for Quantity per Unit");
                                Console.ForegroundColor = ConsoleColor.Black;
                        }
                        break;

                    case "4":       // update Unit Price
                        editProduct.UnitPrice = InputUnitPrice(db,logger);
                        break;

                    case "5":       // update Units in Stock
                        editProduct.UnitsInStock = InputUnitsInStock(db,logger);
                        break;

                    case "6":       // update Units on Order
                        editProduct.UnitsOnOrder = InputUnitsOnOrder(db,logger);
                        break;

                    case "7":       // update Reorder Level
                        editProduct.ReorderLevel = InputReorderLevel(db,logger);
                        break;

                    case "8":       // update Discontinued status
                            Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Is the product active? (Y/N)");
                        string status = Console.ReadLine();
                        if(status.ToLower() == "y"){
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                logger.Info("Product status:  Active");
                                    Console.WriteLine();
                                    Console.ForegroundColor = ConsoleColor.Black;
                            editProduct.Discontinued = false;
                        } 
                        else
                        {
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                logger.Info("Product status:  Discontinued");
                                    Console.WriteLine();
                                    Console.ForegroundColor = ConsoleColor.Black;
                            editProduct.Discontinued = true;
                        }
                        break;

                    case "x":
                    case "X":
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
            } while (userInput.ToLower() != "x");

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

static void DeleteProduct(NWConsole_23_kjbContext db, Logger logger)
{
            Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Choose a product to delete:");
        Console.ForegroundColor = ConsoleColor.Black;
    DisplayProducts(db);
    
    if (int.TryParse(Console.ReadLine(), out int ProductId)) // confirm an actual number was entered
    {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            logger.Info($"ProductId {ProductId} selected");
                Console.ForegroundColor = ConsoleColor.Black;
        Product deleteProduct = db.Products.FirstOrDefault(p => p.ProductId == ProductId);
        if(deleteProduct != null) // confirm the selection matches a product Id
        {
            // determine if this product id has ever been ordered
            var query4 = db.OrderDetails.OrderBy(od => od.OrderDetailsId).Where(p => p.ProductId == ProductId);

            if(query4 != null) 
            {
                // check for orders of this product
                if(query4.Count() != 0)
                {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"\nThis product has history on {query4.Count()} orders and should not be deleted.\n");
                        Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                        Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"This product has history on {query4.Count()} orders and will be deleted.");
                        Console.ForegroundColor = ConsoleColor.Black;

                    db.DeleteProduct(deleteProduct);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    logger.Info($"Product (id: {deleteProduct.ProductId}) deleted");
                        //Console.WriteLine("Currently commented out the delete method within Option: Delete Product");
                        Console.ForegroundColor = ConsoleColor.Black;
                }
            }
            else
            {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                logger.Error("Invalid ID number entered");
                    Console.ForegroundColor = ConsoleColor.Black;
            }
        }
        else 
        {
                Console.ForegroundColor = ConsoleColor.DarkRed;
            logger.Error("Invalid input for product id");
                Console.ForegroundColor = ConsoleColor.Black;
        }
    }
    else
    {
            Console.ForegroundColor = ConsoleColor.DarkRed;
        logger.Error("Invalid input"); // was not a number
            Console.ForegroundColor = ConsoleColor.Black;
    }
} 

static void DeleteCategory(NWConsole_23_kjbContext db,Logger logger)
{
        Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Choose a category to delete:");
        Console.ForegroundColor = ConsoleColor.Black;
    DisplayCategories(db);

    if (int.TryParse(Console.ReadLine(), out int CategoryId)) // confirm an actual number was entered
    {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            logger.Info($"Category Id {CategoryId} selected");
                Console.ForegroundColor = ConsoleColor.Black;
        Category deleteCategory = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == CategoryId);
        if(deleteCategory != null)  // confirm the selection matches a category id
        {
            if(deleteCategory.Products.Count() == 0)    // category contains 0 products and may be deleted
            {
                    Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"The category {deleteCategory.CategoryName} contains {deleteCategory.Products.Count()} products and will be deleted.");
                    Console.ForegroundColor = ConsoleColor.Black;

                db.DeleteCategory(deleteCategory);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                logger.Info($"Category id: {deleteCategory.CategoryId} deleted");
                        //Console.WriteLine("Currently commented out the delete method for Categories with 0 products within the Delete Category method");
                    Console.ForegroundColor = ConsoleColor.Black;
            }
            else if(deleteCategory.Products.Count() != 0)    // category contains products and needs additional investigation before deleting
            {
                    Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"The category {deleteCategory.CategoryName} contains {deleteCategory.Products.Count()} these products:");
                foreach(Product p in deleteCategory.Products) //list the products found associated with this category
                {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(p.ProductName);
                }
                        Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Black;
                foreach (Product p in deleteCategory.Products)  // loop through associated products, deleting those which have no order history
                {
                    // determine if this product id has ever been ordered
                    var query4 = db.OrderDetails.OrderBy(od => od.OrderDetailsId).Where(o => o.ProductId == p.ProductId);

                        if(query4.Count() != 0) // this product DOES have associated orders
                        {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine($"\n\tThe product:\t {p.ProductName}, id: {p.ProductId} has history on {query4.Count()} orders and should not be deleted.");
                                Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else    //this product Does NOT have associated orders and will be deleted
                        {
                                Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\tThe product:\t {p.ProductName}, id: {p.ProductId} \thas history on {query4.Count()} orders and will be deleted.");
                                Console.ForegroundColor = ConsoleColor.Black;

                            db.DeleteProduct(p);
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                            logger.Info($"Product id: {p.ProductId} deleted");
                                //Console.WriteLine("Currently commented out the delete method to delete Product without order history within the Delete Category method");
                                Console.ForegroundColor = ConsoleColor.Black;
                        }
                }

                // recheck category to see if it has any associated products and delete if there aren't any
                if(deleteCategory.Products.Count() == 0)
                {
                    Console.WriteLine($"Category {deleteCategory.CategoryName} is now empty and will be deleted.");
                    db.DeleteCategory(deleteCategory);
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                        logger.Info($"Category id: {deleteCategory.CategoryId} deleted");
                            //Console.WriteLine("Currently commented out the delete method to delete empty category after removing all products who did not have order history");
                        Console.ForegroundColor = ConsoleColor.Black;
                }
                else if(deleteCategory.Products.Count() != 0)
                {
                    Console.WriteLine($"Category {deleteCategory.CategoryName} contains products with order history and cannot be deleted.\nWould you like to rename the category to 'Inactive-{deleteCategory.CategoryName}'? (Y/N)");
                    string uInput = Console.ReadLine();
                        if(uInput.ToLower() == "Y")
                        {
                            deleteCategory.CategoryName = "Inactive-"+deleteCategory.CategoryName;
                        }
                        else
                        {
                            logger.Info("Category name unchanged.");
                        }
                }

            }


        }
        else 
        {
                Console.ForegroundColor = ConsoleColor.DarkRed;
            logger.Error("Invalid input for category id");
                Console.ForegroundColor = ConsoleColor.Black;
        }
    }
    else
    {
            Console.ForegroundColor = ConsoleColor.DarkRed;
        logger.Error("Invalid input"); // was not a number
            Console.ForegroundColor = ConsoleColor.Black;
    }
}
