using NUnit.Framework;
using CalcLibrary;

namespace CalcLibrary.Tests
{
    [TestFixture]
    public class CalculatorTests
    {
        private Calculator calculatorInstance;

        [SetUp]
        public void Initialize()
        {
            calculatorInstance = new Calculator();
        }

        [TearDown]
        public void Cleanup()
        {
            calculatorInstance = null;
        }

        [Test]
        [TestCase(5, 7, 12)]
        [TestCase(-3, 3, 0)]
        [TestCase(10, -5, 5)]
        public void Add_WhenInvoked_ReturnsCorrectSum(int num1, int num2, int expectedSum)
        {
            int actualResult = calculatorInstance.Add(num1, num2);
            Assert.That(actualResult, Is.EqualTo(expectedSum));
        }
    }
}
