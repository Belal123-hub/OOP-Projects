using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using MultilevelMenuExample;
using MultilevelMenuExample.Enums;

Storage storage = Storage.GetInstance();
List<User> userList = storage.GetUsers();
WalletService walletService = new WalletService(new Wallet("DefaultWallet", new Money(0, 0, Currency.USD), Currency.USD), userList);

while (true)
{
    Console.ForegroundColor = ConsoleColor.White;
    IBusinessLogicService service = new BusinessLogicService();
    var walletList = service.Get_Your_Wallets();
    Console.Clear();
    Console.WriteLine("Here are main menu options:");
    var activeUser = storage.GetActiveUser();

    if (activeUser == null)
    {
        Console.WriteLine("**************************************************************************\n");
        Console.WriteLine("Nobody is logged in yet !" + Environment.NewLine);
        Console.WriteLine("1 > Register");
        Console.WriteLine("2 > Login ");
    }

    else
    {
        Console.WriteLine("**************************************************************************\n");
        Console.WriteLine($"Welcome : {activeUser.Name}");

        if (activeUser.ActiveWallet == null)
        {
            Console.WriteLine("No active wallet found , Please choose 4 if you want to create new wallet.");

        }

        else
        {
            Console.WriteLine($"Your activeWallet is : {activeUser.ActiveWallet.Name} ");
        }

        Console.WriteLine("2 > Show my wallets");
        Console.WriteLine("3 > Test operations");
        Console.WriteLine("4 > Make Wallet");
        Console.WriteLine("5 > LogOut");
        Console.WriteLine("6 > Choose wallet");
        Console.WriteLine("7 > Check statistics");
        Console.WriteLine("8 > Delete Wallet");
    }

    Console.WriteLine("666 > Quit\n");

    Console.WriteLine(Environment.NewLine + "Choose from the menu:");
    var userInput = Console.ReadLine();

    if (int.TryParse(userInput, out var output))
    {
        switch (output)
        {
            case 1:
                if (activeUser == null)
                {
                    Register(service);
                }
                else
                {
                    Console.WriteLine("You are already registered.");
                }
                break;

            case 2:
                if (activeUser == null)
                {
                    Login(service);
                }
                else
                {
                    ShowWallets(service, storage);
                }
                break;

            case 3:
                if (activeUser.ActiveWallet != null)
                    TestOperations(walletService, storage);
                break;

            case 4:
                if (activeUser != null)
                {
                    ConstructWallet(service);
                }
                else
                {
                    Console.WriteLine("Please register or log in first.");
                }
                break;

            case 5:
                if (activeUser != null)
                    storage.LogOut();
                break;

            case 6:
                if (activeUser != null)
                    ChooseWallet(service);
                break;

            case 7:
                if (activeUser.ActiveWallet != null)
                    Statistics(service);
                else
                {
                    Console.WriteLine("No Active Wallet ! ");
                }
                break;

            case 8:
                if (activeUser.ActiveWallet != null)
                    DeleteWallet(service, storage);
                break;

            case 666:
                return;

            default:
                break;
        }
    }
    else
    {
        Console.WriteLine("You must type a number only!");
        Console.ReadLine();
    }
}


static void Register(IBusinessLogicService service)                         // if user enters 1 will call this functon to register  if activeuser =null
{
    Console.Clear();
    Console.WriteLine("Type your name:");
    var name = Console.ReadLine();
    Console.WriteLine("Type your email:");
    var email = Console.ReadLine();
    Console.WriteLine("Type your password:");
    var pass = Console.ReadLine();
    Console.WriteLine("Type your birthdate in format 'dd.mm.yyyy':");
    var birthDate = Console.ReadLine();
    var result = service.Register(name, email, pass, birthDate);                                    // WE CALL INTERFACE AND IT'S  METHOD REGISTER
    Console.ForegroundColor = result.Item1 ? ConsoleColor.Green : ConsoleColor.Red;
    Console.WriteLine(result.Item2);
    Console.ForegroundColor = ConsoleColor.White;
    Console.ReadLine();                                                                             // here continue  reading from user
}


static void Login(IBusinessLogicService service)
{
    Console.WriteLine("Type your email:");
    var email = Console.ReadLine();
    Console.WriteLine("Type your password:");
    var pass = Console.ReadLine();
    var result = service.Login(email, pass);
    Console.ForegroundColor = result.Item1 ? ConsoleColor.Green : ConsoleColor.Red;
    Console.WriteLine(result.Item2);                                                             // items are the parameters in the method 
    Console.ForegroundColor = ConsoleColor.White;
    Console.ReadLine();                                                                          // which i call for ex : in login three items
}


static void ConstructWallet(IBusinessLogicService service)
{

    var wallets = service.Get_Your_Wallets();
    Console.Clear();
    Console.WriteLine("Here you will create wallet");
    Console.WriteLine("Type your wallet name:");
    var name = Console.ReadLine();
    Console.WriteLine("Type your integerpart:");
    var integerpart = Console.ReadLine();
    Console.WriteLine("Type your fractionalpart:");
    var fractionalpart = Console.ReadLine();
    Console.WriteLine("Choose your currency:");
    foreach (Currency currencyOption in Enum.GetValues(typeof(Currency)))
    {
        Console.WriteLine($"{(int)currencyOption} > {currencyOption}");
    }
    var currencyIndex = Console.ReadLine();
    Enum.TryParse<Currency>(currencyIndex, out var currencyCode);
    Money money = new Money(Int32.Parse(integerpart), Int32.Parse(fractionalpart), currencyCode);
    var result = service.ConstructWallet(name, money);
    Console.ForegroundColor = result.Item1 ? ConsoleColor.Green : ConsoleColor.Red;
    Console.WriteLine(result.Item2);
    Console.ForegroundColor = ConsoleColor.White;
    Console.ReadLine();
}


void ChooseWallet(IBusinessLogicService services)
{
    var walletList = services.Get_Your_Wallets();
    Console.Clear();

    if (walletList.Count == 0)
    {
        throw new InvalidOperationException("No wallets available. Please create a wallet first.");
    }

    Console.WriteLine("Choose your Wallet to perform operation\n");

    int walletIndex = 0;
    foreach (var wallet in walletList)
    {
        Console.WriteLine(walletIndex + " " + wallet.Name);
        walletIndex++;
    }
    Console.WriteLine("Input wallet index: ");
    var indexInput = Console.ReadLine();

    if (int.TryParse(indexInput, out int conIndex) && conIndex >= 0 && conIndex < walletList.Count)
    {
        var walletActive = services.ChooseWallet(conIndex);
        walletService = new WalletService(walletActive, userList);
    }

    else
    {
        throw new ArgumentException("Invalid input. Please enter a valid wallet index.");
    }
}


static Money GetMoneyDetails()
{
    Console.WriteLine("Type IntegerValue:");
    var Integerpart = Console.ReadLine();
    Console.WriteLine("Type fractionpart:");
    var fractionpart = Console.ReadLine();
    Console.WriteLine("Choose your currency:");
    foreach (Currency currencyOption in Enum.GetValues(typeof(Currency)))
    {
        Console.WriteLine($"{(int)currencyOption} > {currencyOption}");
    }
    var currencyIndex = Console.ReadLine();
    Enum.TryParse<Currency>(currencyIndex, out var currencyCode);
    Money money = new Money(Int32.Parse(Integerpart), Int32.Parse(fractionpart), currencyCode);
    return money;
}


static void ShowWallets(IBusinessLogicService service, Storage storage)
{
    var activeUser = storage.GetActiveUser();
    var wallets = service.Get_Your_Wallets();
    if (wallets.Count == 0)
    {
        throw new InvalidOperationException("No wallets available. Please create a wallet first.");
    }

    Console.WriteLine($"******************************** Welcome {activeUser.Name} here you can see all your wallets : ******************************** \n");


    int WalletIndex = 1;
    foreach (var wallet in wallets)
    {
        Console.WriteLine($"$$$$$$$$ Wallet {WalletIndex} $$$$$$$$\n");
        Console.WriteLine($"Currency: {wallet.Currency}");
        Console.WriteLine($"Wallet Name: {wallet.Name}");
        string getMoney = wallet.Balance != null
            ? $"Your Current Balance : {wallet.Balance}\n"
            : "0.0\n";

        WalletIndex++;
        Console.WriteLine(getMoney);
    }

    Console.WriteLine("**************************************************************************************************************");
    Console.ReadLine();
}



static void Statistics(IBusinessLogicService services)
{
    Console.Write("Please enter the start date in this format 'dd.MM.yyyy': ");
    var start = Console.ReadLine();

    Console.Write("Please enter the end date in this format 'dd.MM.yyyy': ");
    var end = Console.ReadLine();

    if (DateTime.TryParseExact(start, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateFromParsed)
       && DateTime.TryParseExact(end, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateToParsed))
    {
        CollectAndDisplayStatistics(services, dateFromParsed, dateToParsed);
    }

}


static void CollectAndDisplayStatistics(IBusinessLogicService service, DateTime dateFrom, DateTime dateTo)
{
    while (true)
    {
        Console.WriteLine("Please enter 1 to check stats or another number to exit :");

        var userInput = Console.ReadLine();

        if (int.TryParse(userInput, out var output))
        {
            switch (output)
            {
                case 1:

                    var statistics = service.CollectStatistics(dateFrom, dateTo);

                    if (statistics != null && statistics.Count > 0)
                    {
                        Console.WriteLine("Statistics:");

                        foreach (var stat in statistics)
                        {
                            Console.WriteLine($"Amount: {stat.Amount.GetFullNumber()}, Date: {stat.Date.Date.ToString("dd.MM.yyyy")}, Type: {stat.Type}, Category: {stat.Category}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Debug: No transactions found for the specified period.");
                    }
                    break;

                default:
                    return;  // Exit the method when the user chooses something other than '1'
            }
        }
    }
}



static void TestOperations(WalletService service, Storage storage)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("Choose operation type:");
        Console.WriteLine("1 > Income");
        Console.WriteLine("2 > Expense");
        Console.WriteLine("3 > Back to main menu");
        var userInput = Console.ReadLine();

        if (int.TryParse(userInput, out var operationType))
        {
            Operation operation;
            Money money;

            switch (operationType)
            {
                case 1:
                    operation = new Income();
                    money = GetMoneyDetails();
                    Console.WriteLine("Choose Income category:");

                    foreach (IncomeType incomeCategoryOption in Enum.GetValues(typeof(IncomeType)))
                    {
                        Console.WriteLine($"{(int)incomeCategoryOption} > {incomeCategoryOption}");
                    }

                    var incomeCategoryIndex = Console.ReadLine();
                    if (Enum.TryParse<IncomeType>(incomeCategoryIndex, out var selectedIncomeCategory))
                    {
                        Console.Write("Enter the operation date (dd.MM.yyyy): ");

                        if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime operationDate))
                        {
                            service.AddOperation(operation, money, (int)selectedIncomeCategory, operationDate);
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid category number!");
                    }
                    break;

                case 2:
                    operation = new Expense();
                    money = GetMoneyDetails();
                    Console.WriteLine("Choose Expense category:");

                    foreach (ExpenseType expenseCategoryOption in Enum.GetValues(typeof(ExpenseType)))
                    {
                        Console.WriteLine($"{(int)expenseCategoryOption} > {expenseCategoryOption}");
                    }

                    var expenseCategoryIndex = Console.ReadLine();
                    if (Enum.TryParse<ExpenseType>(expenseCategoryIndex, out var selectedExpenseCategory))
                    {
                        Console.Write("Enter the operation date (dd.MM.yyyy): ");
                        if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime expenseOperationDate))
                        {
                            service.AddOperation(operation, money, (int)selectedExpenseCategory, expenseOperationDate);
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid category number!");
                    }
                    break;

                case 3:
                    // Return back to the main menu
                    return;

                default:
                    Console.WriteLine("Invalid operation type!");
                    break;
            }
        }
        else
        {
            Console.WriteLine("You must type a number only!");
        }

        Console.ReadLine();
    }
}




static void DeleteWallet(IBusinessLogicService service, Storage storage)
{
    var activeUser = storage.GetActiveUser();
    Console.WriteLine("Here are your wallets:");
    var walletList = service.Get_Your_Wallets();
    int walletIndex = 0;
    foreach (var wallet in walletList)
    {
        Console.WriteLine(walletIndex + " " + wallet.Name);
        walletIndex++;
    }
    Console.Write("Enter the index of the wallet to delete: ");
    if (int.TryParse(Console.ReadLine(), out walletIndex) && walletIndex >= 0 && walletIndex < walletList.Count)
    {
        Console.Write("Are you sure you want to delete this wallet? (Yes/No): ");
        string confirmation = Console.ReadLine();
        if (confirmation.Equals("Yes", StringComparison.OrdinalIgnoreCase))
        {
            var deletedWallet = walletList[walletIndex];
            var result = service.DeleteWallet(walletIndex);
            Console.WriteLine(result.Item2);

            // Check if the deleted wallet is the active wallet, and set it to null if true
            if (activeUser.ActiveWallet != null && activeUser.ActiveWallet.Equals(deletedWallet))
            {
                activeUser.ActiveWallet = null;
            }
        }
        else
        {
            Console.WriteLine("Wallet deletion canceled.");
        }
    }
    else
    {
        Console.WriteLine("Invalid input for wallet index.");
    }
}
