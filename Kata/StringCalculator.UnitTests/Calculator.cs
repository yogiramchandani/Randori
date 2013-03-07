using System;
using System.Collections.Generic;
using System.Linq;

namespace StringCalculator.UnitTests
{
    public class Calculator
    {
        public int Add(string numbers)
        {
            var delimiters = new List<string> {",", "\n"};

            if (numbers.StartsWith("//"))
            {
                delimiters.AddRange(ExtractDelimiters(numbers));

                numbers = numbers.Substring(numbers.IndexOf("\n"));
            }
            var smallNumbers = numbers.Split(delimiters.ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).Where(x => x <= 1000);
            var negativeNumbers = smallNumbers.Where(x => x < 0).ToArray();
            if (negativeNumbers.Any())
            {
                throw new ArgumentOutOfRangeException(
                    "numbers",
                    string.Format(
                        "Negative numbers not allowed, found: {0}", string.Join(",", negativeNumbers)));
            }
            return smallNumbers.Sum();
        }

        private static List<string> ExtractDelimiters(string numbers)
        {
            var delimiters = new List<string>();
            if (numbers.StartsWith("//["))
            {
                if (numbers.Contains("]["))
                {
                    var envelop = numbers.Substring(2, numbers.IndexOf("]\n") - 1);
                    while (envelop != string.Empty)
                    {
                        var closingBracketPosition = envelop.IndexOf("]");
                        delimiters.Add(envelop.Substring(1, closingBracketPosition - 1));
                        envelop = envelop.Substring(closingBracketPosition + 1);
                    }
                }
                else
                {
                    delimiters.Add(numbers.Substring(3, numbers.IndexOf(']') - 3));
                }
            }
            else
            {
                delimiters.Add(numbers.Substring(2, 1));
            }
            return delimiters;
        }
    }
}