namespace MyMoney
{
    class MONEY
    {
        static void Main(string[] args)
        {
            Money GetTheMoney1 = new Money();
            Console.WriteLine(GetTheMoney1.GetwhooleNumber());

            GetTheMoney1.AddingValue('-', 1000, 12);
            Console.WriteLine(GetTheMoney1.GetwhooleNumber());
            GetTheMoney1.SubtractionValue('-', 1000, 12);
            Console.WriteLine(GetTheMoney1.GetwhooleNumber());

            Money GetTheMoney2 = new Money('-', 1000, 12, "USD");
            Money GetTheMoney3 = new Money('-', 1000, 12, "USD");
            GetTheMoney1.AddingValue2(GetTheMoney2);
            Console.WriteLine(GetTheMoney1.GetwhooleNumber());  // Output: -1000.12 USD

            GetTheMoney1.SubtractionValue2(GetTheMoney2);
            Console.WriteLine(GetTheMoney1.GetwhooleNumber());

            bool areEqual = GetTheMoney2.MEq(GetTheMoney3);
            if (areEqual)
            {
                Console.WriteLine("The two monetary values are equal.");
            }
            else
            {
                Console.WriteLine("The two monetary values are not equal.");
            }

            int comparisonResult = GetTheMoney2.MComp(GetTheMoney3);
            if (comparisonResult < 0)
            {
                Console.WriteLine("GetTheMoney2 is less than GetTheMoney3.");
            }
            else if (comparisonResult > 0)
            {
                Console.WriteLine("GetTheMoney2 is greater than GetTheMoney3.");
            }
            else
            {
                Console.WriteLine("GetTheMoney2 is equal to GetTheMoney3.");
            }

            Money GetTheMoney4 = GetTheMoney2.SumMoney(GetTheMoney2, GetTheMoney3);
            Console.WriteLine("GetTheMoney4: " + GetTheMoney4.GetwhooleNumber());

            Money GetTheMoney5 = GetTheMoney2.SubstractMoney(GetTheMoney2, GetTheMoney3);
            Console.WriteLine("GetTheMoney5: " + GetTheMoney5.GetwhooleNumber());
            GetTheMoney2.ConvertToCurrency(GetTheMoney3);
            Console.WriteLine(GetTheMoney2.GetwhooleNumber());

        }
    }
}