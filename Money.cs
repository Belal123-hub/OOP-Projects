
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MultilevelMenuExample.Enums;                                      // Money class 
using Newtonsoft.Json.Linq;

namespace MultilevelMenuExample
{
    public class Money
    {
        private byte _fractionalNumber;
        public Currency _currency;
        public uint IntegerNumber { get; set; }
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

        public static Money Zero => new Money(0, 0,0);
        public Money(int intPart, int fracPart, Currency Currency)
        {                                                                                            // the Main constructor
            setWholeNumber(intPart, fracPart,(Currency)Currency);
        }

        public uint GetInteger()
        {
          return IntegerNumber;
        }

        public string GetFullNumber()
        {
         
          return (IntegerNumber.ToString() + "." + _fractionalNumber.ToString());

        }

        public byte GetFRactionalNumber()
        {
          return FractionalNumber;
        }

        public Currency GetCurrency()
        {
          return _currency;
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

      /*  public Money()
        {

        }*/
        public void SetCurrency(Currency NewCurrency)
        {
            this._currency = NewCurrency;
        }

        public void setWholeNumber(int intPart, int fracPart, Currency Currency)
        {
            this._currency = Currency;
            SetCurrency(_currency);
            SetInteger(Convert.ToUInt32(intPart));
            SetFrationalNumber((byte)fracPart);
        }
    }
}


