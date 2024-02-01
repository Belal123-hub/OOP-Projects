
using MultilevelMenuExample.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultilevelMenuExample
{
    public class Wallet
    {
        public List<Operation> Operations { get; set; } = new List<Operation>();

        public decimal Balance { get; set; }
        public Currency Currency { get; set; }
        public string Name { get; set; }
        public Money StartAmount { get; set; }
       

        public Wallet(string name, Money startAmount, Currency currency)
        {
            Name = name;
            StartAmount = startAmount;
            Currency = currency;
            Balance += Convert.ToDecimal(startAmount?.GetFullNumber());
        }


        public void AddOperation(Operation operation)
        {
            if (operation.value.GetCurrency() != Currency)
            {
                Console.WriteLine("Warning: The operation currency is different from the wallet currency!");

                // Optional: here i can implement currency conversion logic here if needed.
                // For now, i won't process the operation if the currencies are different.
                return;
            }

            Operations.Add(operation);

            if (operation is Expense expense)
            {
                decimal doubleNumber = ConvertExpenseAmount(expense.value);
                Console.WriteLine($"Your expense amount: {doubleNumber}");
                Balance -= doubleNumber;
            }
            else if (operation is Income income)
            {
                decimal doubleNumber = ConvertIncomeAmount(income.value);
                Console.WriteLine($"Your income amount: {doubleNumber}");
                Balance += doubleNumber;
            }

            Console.WriteLine($"Your final balance is: {Balance}");
        }



        private decimal ConvertExpenseAmount(Money expenseValue)
        {
            string number = expenseValue.GetInteger() + "." + expenseValue.GetFRactionalNumber();
            return decimal.Parse(number);
        }


        private decimal ConvertIncomeAmount(Money incomeValue)
        {
            string number = incomeValue.GetInteger() + "." + incomeValue.GetFRactionalNumber();
            return decimal.Parse(number);
        }


        public List<TransactionDetails> CheckStatistics(DateTime fromDate, DateTime toDate)
        {
            var transactions = new List<TransactionDetails>();

            foreach (var operation in Operations)
            {
                if (operation.Date >= fromDate && operation.Date <= toDate)
                {
                    string operationType = (operation is Expense) ? "Expense" : "Income";
                    string category = (operation is Expense expense) ? expense.Type.ToString() : (operation is Income income) ? income.Type.ToString() : "";

                    transactions.Add(new TransactionDetails
                    {
                        Amount = operation.value,
                        Date = operation.Date, // Use DateTimeOffset to include timezone information
                        Type = operationType,
                        Category = category
                    });
                }
            }

            return transactions;
        }

    }


    public class Operation
    {
        public Money value { get; set; }
        public DateTime Date { get; set; }
    }

    public class Expense : Operation
    {
        public ExpenseType Type { get; set; }
    }

    public class Income : Operation
    {
        public IncomeType Type { get; set; }
    }
}