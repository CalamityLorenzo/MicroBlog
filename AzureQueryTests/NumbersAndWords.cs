using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Text;

namespace AzureQueryTests
{
    internal class NumbersAndWords : TableEntity
    {
        public NumbersAndWords() { }
        public NumbersAndWords(int Number, string NumberAsWords) : base("numbers"   , String.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.Now.Ticks))
        {
            this.NumberAsWords = NumberAsWords;
        }
        public int Number => int.TryParse(PartitionKey, out var val) ? val : 0;
        public string NumberAsWords { get; set; }
        public DateTime Published { get; set; }

        public override string ToString()
        {
            return $"{Number} {Published}";
        }
    }

    public static class NumToWor
    {
        public static string NumbersToWords(int num)
        {
            StringBuilder words = new StringBuilder();
            string[] singles = new string[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            string[] teens = new string[] { "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            string[] tens = new string[] { "", "", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninty" };
            string[] powers = new string[] { "", "thousand", "million", "billion", "trillion", "quadrillion", "quintillion" };

            if (num >= 1000)
            {
                for (int i = powers.Length - 1; i >= 0; i--)
                {
                    int power = (int)Math.Pow(1000, i);
                    int count = (num - (num % power)) / power;

                    if (num > power)
                    {
                        words.Append(NumbersToWords(count) + " " + powers[i]);
                        num -= count * power;
                    }
                }
            }

            if (num >= 100)
            {
                int count = (num - (num % 100)) / 100;
                words.Append(NumbersToWords(count) + " hundred");
                num -= count * 100;
            }

            if (num >= 20 && num < 100)
            {
                int count = (num - (num % 10)) / 10;
                words.Append(" " + tens[count]);
                num -= count * 10;
            }

            if (num >= 10 && num < 20)
            {
                words.Append(" " + teens[num - 10]);
                num = 0;
            }

            if (num > 0 && num < 10)
            {
                words.Append(" " + singles[num]);
            }

            return words.ToString();
        }
    }
}
