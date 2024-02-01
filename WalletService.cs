using MultilevelMenuExample.Enums;
using MultilevelMenuExample;
using System;

internal class WalletService
{
    private Wallet _wallet;
    private List<User> _users;

    public WalletService(Wallet wallet, List<User> users)
    {
        _wallet = wallet;
        _users = users;
    }
    public (bool, string) AddOperation(Operation operation, Money money, int categoryNumber, DateTime date)
    {
        Operation newOperation = null;

        if (operation is Expense)
        {
            var expense = new Expense();
            expense.Type = (ExpenseType)categoryNumber;
            newOperation = expense;
        }

        else if (operation is Income)
        {
            var income = new Income();
            income.Type = (IncomeType)categoryNumber;
            newOperation = income;
        }

        newOperation.value = money;
        newOperation.Date = date;

        // Add the operation to the wallet
        _wallet.AddOperation(newOperation);
        return (true, "Operation added successfully");
    }
}