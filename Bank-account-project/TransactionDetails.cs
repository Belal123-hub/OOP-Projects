using System;

namespace MultilevelMenuExample
{
    public class TransactionDetails
    {
        public Money Amount { get; set; }
        public DateTimeOffset Date { get; set; } 
        public string Type { get; set; }
        public string Category { get; set; }
    }
}
