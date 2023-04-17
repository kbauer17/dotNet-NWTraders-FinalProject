using NLog;
using System.Linq;
using Northwind_console.Model;

// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory() + "\\nlog.config";

// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
logger.Info("Program started");

try
{
    string choice;
    do
    {
        Console.WriteLine("1) Display Categories");
        Console.WriteLine("2) Add Category");
        Console.WriteLine("\"q\" to quit");
        choice = Console.ReadLine();
        Console.Clear();

    } while (choice.ToLower() != "q");
}
catch (Exception ex)
{
    logger.Error(ex.Message);
}

logger.Info("Program ended");