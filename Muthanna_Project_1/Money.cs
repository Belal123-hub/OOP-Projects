using System;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;

public sealed class Money : IEquatable<Money>, IComparable<Money>
{
    private char _sign;
    public char Sign
    {
        get
        {
            return _sign;
        }
        set
        {
            if (value == '+' || value == '-')
            {
                _sign = value;
            }
            else
            {
                throw new ArgumentException("Sign must be either + or -");
            }
        }
    }

    public uint IntegerNumber { get; set; }
    private byte _fractionalNumber;

    public byte FractionalNumber
    {
        get
        {
            return _fractionalNumber;
        }
        set
        {
            if (value >= 0 && value < 100)
            {
                _fractionalNumber = value;
            }
            else
            {
                throw new ArgumentException("FractionalNumber must be >= 0 and < 100. Value not set.");
            }
        }
    }

    private readonly string[] SupportedCurrencies = { "USD", "RUB", "EURO" };
    private string _currency;
    public string Currency
    {
        get
        {
            return _currency;
        }
        set
        {
            if (Array.Exists(SupportedCurrencies, currency => currency == value))
            {
                _currency = value;
            }
            else
            {
                throw new ArgumentException("Invalid currency. Currency must be one of: " + string.Join(", ", SupportedCurrencies));
            }
        }
    }

    public Money()
    {                                             // constructor  c1
        Random rnd = new Random();
        //Sign = rnd.Next(2) == 0 ? '+' : '-';   in case we can start with - or +
        Sign = '+';
        IntegerNumber = (uint)rnd.Next(100000);
        FractionalNumber = (byte)rnd.Next(100);
        string[] Currencies = SupportedCurrencies;
        Currency = Currencies[rnd.Next(Currencies.Length)];        //string[] Currencies = { "USD", "RUB", "EURO" };
        //Currency = Currencies[rnd.Next(Currencies.Length)];
    }
    public Money(char Thesign, uint integernum, byte fractionalnum, string currency)
    {
        if (Thesign == '+' || Thesign == '-')                                              // constructor  c2
        {
            Sign = Thesign;
        }
        else
        {
            throw new ArgumentException("Sign must be either + or -");

        }

        if (integernum >= 0)
        {
            IntegerNumber = integernum;
        }
        else
        {
            throw new ArgumentException("Invalid integer number. IntegerNumber must be >= 0.");
        }

        if (fractionalnum >= 0 && fractionalnum < 100)
        {
            FractionalNumber = fractionalnum;
        }
        else
        {
            throw new ArgumentException("Invalid fractional number. FractionalNumber must be >= 0 and < 100.");
        }

        if (Array.Exists(SupportedCurrencies, c => c == currency))
        {
            Currency = currency;
        }
        else
        {
            throw new ArgumentException("Invalid currency. Currency must be either USD, EURO, or RUB.");
        }
    }

    public Money(Money CopyConstructor)     // constructor  c3
    {                                                  // by  copying of c2
        Sign = CopyConstructor.Sign;
        IntegerNumber = CopyConstructor.IntegerNumber;
        FractionalNumber = CopyConstructor.FractionalNumber;
        Currency = CopyConstructor.Currency;
    }
    public Money(string WholeNumber)
    {
        setWholeNumber(WholeNumber);           // constructor  c4
    }
    public char GetSign()
    {
        return Sign;
    }
    public uint GetInteger()
    {
        return IntegerNumber;
    }
    public byte GetFRactionalNumber()
    {
        return FractionalNumber;
    }
    public string GetwhooleNumber()
    {
        return $"{Sign}{IntegerNumber}.{FractionalNumber}{Currency}";
    }
    public void SetSign(char NewSign)
    {
        if (NewSign == '+' || NewSign == '-')
        {
            Sign = NewSign;
        }
        else
        {
            throw new ArgumentException("Sign must be either + or -");
        }
    }
    public void SetInteger(uint NewInteger)
    {
        if (NewInteger >= 0)
        {
            IntegerNumber = NewInteger;
        }
        else
        {
            throw new ArgumentException("Invalid integer number. IntegerNumber must be >= 0.");
        }
    }

    public void SetFrationalNumber(byte NewFractional)
    {
        if (NewFractional >= 0 && NewFractional < 100)
        {
            FractionalNumber = NewFractional;
        }
        else
        {
            throw new ArgumentException("Invalid fractional number. FractionalNumber must be >= 0 and < 100.");
        }
    }

    public void SetCurrency(string NewCurrency)
    {
        if (Array.Exists(SupportedCurrencies, c => c == NewCurrency))
        {
            Currency = NewCurrency;
        }
        else
        {
            throw new ArgumentException("Invalid currency. Currency must be either USD, EURO, or RUB.");
        }
    }
    public bool setWholeNumber(string num)
    {
        Regex moneyReg = new Regex(@"^[-+]?\d+(\.\d+)?$");
        Regex integerReg = new Regex(@"\d+(?=\.)");
        Regex fractionalReg = new Regex(@"(?<=\.)\d+");
        if (moneyReg.IsMatch(num))
        {
            byte fractionalPart = byte.Parse(fractionalReg.Match(num).Value);
            if (fractionalPart >= 0 && fractionalPart < 100)
            {
                this.FractionalNumber = fractionalPart;
            }
            else
            {
                return false;
            }
            this.IntegerNumber = uint.Parse(integerReg.Match(num).Value);
            if (num[0] == '-')
            {
                this.Sign = '-';
            }
            else
            {
                this.Sign = '+';
            }
            return true;
        }
        return false;
    }
    public void AddingValue(char sign, uint integerPart, byte fractionalPart)
    {
        // Calculate the overall value as a whole number including both integer and fractional parts
        long currentOverallValue = (Sign == '-') ? -(long)IntegerNumber * 100 - FractionalNumber : (long)IntegerNumber * 100 + FractionalNumber;
        long toAddOverallValue = (sign == '-') ? -(long)integerPart * 100 - fractionalPart : (long)integerPart * 100 + fractionalPart;

        // Add the integer and fractional parts
        long newOverallValue = currentOverallValue + toAddOverallValue;

        // Determine the new sign
        Sign = (newOverallValue < 0) ? '-' : '+';

        // Check if the fractional part is greater than 100
        if (fractionalPart >= 100)
        {
            throw new ArgumentException("Invalid fractional number. FractionalNumber must be >= 0 and < 100.");
            return; // Exit the method to avoid updating the IntegerNumber and FractionalNumber
        }

        // Update the integer and fractional parts directly without using Math.Abs
        if (newOverallValue < 0)
        {
            newOverallValue = -newOverallValue;
            Sign = '-';
        }

        IntegerNumber = (uint)newOverallValue / 100;
        FractionalNumber = (byte)(newOverallValue % 100);
    }
    public void SubtractionValue(char sign, uint integerPart, byte fractionalPart)
    {

        long currentOverallValue = (Sign == '-') ? -(long)IntegerNumber * 100 - FractionalNumber : (long)IntegerNumber * 100 + FractionalNumber;
        long toSubtractOverallValue = (sign == '-') ? -(long)integerPart * 100 - fractionalPart : (long)integerPart * 100 + fractionalPart;

        long newOverallValue = currentOverallValue - toSubtractOverallValue;

        Sign = (newOverallValue < 0) ? '-' : '+';

        if (fractionalPart >= 100)
        {
            throw new ArgumentException("Invalid fractional number. FractionalNumber must be >= 0 and < 100.");
            return;
        }
        if (newOverallValue < 0)
        {
            newOverallValue = -newOverallValue;
            Sign = '-';
        }

        IntegerNumber = (uint)newOverallValue / 100;
        FractionalNumber = (byte)(newOverallValue % 100);
    }
    private long CalculateOverallValue(char moneySign, uint moneyInteger, byte moneyFractional)
    {
        return (moneySign == '-') ? -(long)moneyInteger * 100 - moneyFractional : (long)moneyInteger * 100 + moneyFractional;
    }

    private void UpdateMoneyValues(long newOverallValue)
    {
        Sign = (newOverallValue < 0) ? '-' : '+';

        if (newOverallValue < 0)
        {
            newOverallValue = -newOverallValue;
            Sign = '-';
        }

        IntegerNumber = (uint)newOverallValue / 100;
        FractionalNumber = (byte)(newOverallValue % 100);
    }

    public void AddingValue2(Money moneyToAdd)
    {
        if (Currency == moneyToAdd.Currency)
        {
            long overallValueToAdd = CalculateOverallValue(moneyToAdd.Sign, moneyToAdd.IntegerNumber, moneyToAdd.FractionalNumber);
            long currentOverallValue = CalculateOverallValue(Sign, IntegerNumber, FractionalNumber);
            long newOverallValue = currentOverallValue + overallValueToAdd;

            UpdateMoneyValues(newOverallValue);
        }
        else
        {
            throw new ArgumentException("Currencies are not the same. Cannot perform the operation.");
        }
    }

    public void SubtractionValue2(Money moneyToSubtract)
    {
        if (Currency == moneyToSubtract.Currency)
        {
            long overallValueToSubtract = CalculateOverallValue(moneyToSubtract.Sign, moneyToSubtract.IntegerNumber, moneyToSubtract.FractionalNumber);
            long currentOverallValue = CalculateOverallValue(Sign, IntegerNumber, FractionalNumber);
            long newOverallValue = currentOverallValue - overallValueToSubtract;

            UpdateMoneyValues(newOverallValue);
        }
        else
        {
            throw new ArgumentException("Currencies are not the same. Cannot perform the operation.");
        }
    }

    public bool MEq(Money otherMoney)
    {
        if (otherMoney is null)
            return false;

        // Check if the currencies and values are the same
        return Sign == otherMoney.Sign &&
         IntegerNumber == otherMoney.IntegerNumber &&
         FractionalNumber == otherMoney.FractionalNumber &&
         Currency == otherMoney.Currency;
    }

    public bool Equals(Money other)
    {
        return MEq(other);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Sign, IntegerNumber, FractionalNumber, Currency);
    }
    public int MComp(Money otherMoney)
    {
        if (otherMoney is null)
            throw new ArgumentNullException(nameof(otherMoney), "Cannot compare to a null value.");

        // Compare the two monetary values based on their overall value
        long thisValue = (Sign == '-') ? -IntegerNumber * 100 - FractionalNumber : IntegerNumber * 100 + FractionalNumber;
        long otherValue = (otherMoney.Sign == '-') ? -otherMoney.IntegerNumber * 100 - otherMoney.FractionalNumber : otherMoney.IntegerNumber * 100 + otherMoney.FractionalNumber;

        return thisValue.CompareTo(otherValue);
    }
    public int CompareTo(Money other)
    {
        return MComp(other);
    }
    public Money SumMoney(Money money1, Money money2)
    {
        Money result = new Money(money1);
        result.AddingValue2(money2);
        return result;
    }
    public Money SubstractMoney(Money money1, Money money2)
    {
        Money result2 = new Money(money1);
        result2.SubtractionValue2(money2);
        return result2;
    }
    public void ConvertToCurrency(Money targetCurrencyMoney)
    {

        if (Currency != targetCurrencyMoney.Currency)
        {
            long overallValue = (Sign == '-') ? -IntegerNumber * 100 - FractionalNumber : IntegerNumber * 100 + FractionalNumber;

            double conversionRate = GetConversionRate(Currency, targetCurrencyMoney.Currency);

            overallValue = (int)(overallValue * conversionRate);

            Sign = (overallValue < 0) ? '-' : '+';

            overallValue = Math.Abs(overallValue);

            IntegerNumber = (uint)overallValue / 100;
            FractionalNumber = (byte)(overallValue % 100);
            Currency = targetCurrencyMoney.Currency;
        }
    }
    private double GetConversionRate(string sourceCurrency, string targetCurrency)
    {
        if (sourceCurrency == "USD" && targetCurrency == "EUR")
        {
            return 0.85; // 1 USD is equivalent to 0.85 EUR
        }
        else if (sourceCurrency == "USD" && targetCurrency == "RUB")
        {
            return 73.5; // 1 USD is equivalent to 73.5 RUB
        }
        else if (sourceCurrency == "EUR" && targetCurrency == "USD")
        {
            return 1.18; // 1 EUR is equivalent to 1.18 USD
        }

        return 1.0;
    }

}

