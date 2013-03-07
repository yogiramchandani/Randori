using System;
using System.Linq;
using System.Text.RegularExpressions;
using Ploeh.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace StringCalculator.UnitTests
{
    public class CalculatorTests
    {
        [Theory, CalculatorTestConventions]
        public void AddEmpty_ReturnsCorrectResult(Calculator sut)
        {
            var numbers = "";
            int actual = sut.Add(numbers);
            Assert.Equal(0, actual);
        }
        
        [Theory, CalculatorTestConventions]
        public void AddSingleNumber_ReturnsCorrectResult(Calculator sut, int expected)
        {
            var numbers = expected.ToString();
            int actual = sut.Add(numbers);
            Assert.Equal(expected, actual);
        }
        
        [Theory, CalculatorTestConventions]
        public void AddTwoNumbers_ReturnsCorrectResult(Calculator sut, int x, int y)
        {
            var numbers = string.Format("{0},{1}", x, y);
            int actual = sut.Add(numbers);
            Assert.Equal(x + y, actual);
        }
        
        [Theory, CalculatorTestConventions]
        public void AddAnyAmountOfNumbers_ReturnsCorrectResult(Calculator sut, int count, Generator<int> generator)
        {
            var listOfNumbers = generator.Take(count + 2).ToArray();
            string numbers = string.Join(",", listOfNumbers);
            int actual = sut.Add(numbers);
            int expected = listOfNumbers.Sum();
            Assert.Equal(expected, actual);
        }
        
        [Theory, CalculatorTestConventions]
        public void AddWithLineBreakAndCommaDelimiter_ReturnsCorrectResult(Calculator sut, int x, int y, int z)
        {
            string numbers = string.Format("{0}\n{1},{2}", x, y, z);
            int actual = sut.Add(numbers);
            
            Assert.Equal(x + y + z, actual);
        }
        
        [Theory, CalculatorTestConventions]
        public void AddLineWithCustomDelimiter_ReturnsCorrectResult(
            Calculator sut, 
            Generator<char> charGenerator, 
            int count, 
            Generator<int> numberGenerator)
        {
            int dummy;
            var delimiter = charGenerator
                .Where(x => !int.TryParse(x.ToString(), out dummy)).First(x => x != '-');
            var listOfNumbers = numberGenerator.Take(count + 2).ToArray();
            string numbers = string.Format("//{0}\n{1}", delimiter, string.Join(delimiter.ToString(), listOfNumbers));
            int actual = sut.Add(numbers);
            
            Assert.Equal(listOfNumbers.Sum(), actual);
        }
        
        [Theory, CalculatorTestConventions]
        public void AddNegativeNumber_ExpectAnExceptionWithTheNegativeNumber(Calculator sut, int expected)
        {
            expected = -expected;
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => sut.Add(expected.ToString()));
            Assert.True(ex.Message.StartsWith("Negative numbers not allowed"));
            Assert.True(ex.Message.Contains(expected.ToString()));
        }
        
        [Theory, CalculatorTestConventions]
        public void AddNegativeNumbers_ExpectAnExceptionWithTheNegativeNumbers(Calculator sut, int x, int y, int z)
        {
            x = -x;
            z = -z;
            var numbers = string.Join(",", x, y, z);
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => sut.Add(numbers));
            Assert.True(ex.Message.StartsWith("Negative numbers not allowed"));
            Assert.True(ex.Message.Contains(x.ToString()));
            Assert.True(ex.Message.Contains(z.ToString()));
        }

        
        [Theory, CalculatorTestConventions]
        public void AddNumbersWithOver1000_ReturnsResultIgnoringNumbersOver1000(Calculator sut, int expected)
        {
            var numbers = string.Format("{0}, 1001", expected);
            var actual = sut.Add(numbers);
            Assert.Equal(expected, actual);
        }

        [Theory, CalculatorTestConventions]
        public void AddIgnoresBigNumber_ReturnsCorrectResult(Calculator sut, int smallSeed, int bigSeed)
        {
            int x = Math.Min(smallSeed, 1000);
            int y = Math.Max(bigSeed, 1001);
            var actual = sut.Add(string.Join(",", x, y));
            Assert.Equal(x, actual);
        }

        [Theory, CalculatorTestConventions]
        public void AddLineWithCustomDelimiterOfAnyLength_ReturnsCorrectResult(
            Calculator sut,
            string delimiter,
            int count,
            Generator<int> numberGenerator)
        {
            var listOfNumbers = numberGenerator.Take(count + 2).ToArray();
            string numbers = string.Format("//[{0}]\n{1}", delimiter, string.Join(delimiter, listOfNumbers));
            int actual = sut.Add(numbers);

            Assert.Equal(listOfNumbers.Sum(), actual);
        }

        [Theory, CalculatorTestConventions]
        public void AddLineWithMultipleCustomDelimiterOfAnyLength_ReturnsCorrectResult(
            Calculator sut,
            string delimiter1,
            string delimiter2,
            int x,
            int y,
            int z)
        {
            string numbers = string.Format("//[{0}][{1}]\n{1}{2}{0}{3}{1}{4}", delimiter1, delimiter2, x, y, z);
            int actual = sut.Add(numbers);

            Assert.Equal(x + y + z, actual);
        }
    }
}
