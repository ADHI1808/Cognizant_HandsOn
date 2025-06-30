/*Step1- Create folder in vs code and execute below in terminal
dotnet new classlib -n CalcLibrary
dotnet new nunit -n CalcLibrary.Tests
dotnet new sln -n CalculatorApp
dotnet sln CalculatorApp.sln add CalcLibrary/CalcLibrary.csproj
dotnet sln CalculatorApp.sln add CalcLibrary.Tests/CalcLibrary.Tests.csproj
cd CalcLibrary.Tests
dotnet add reference ../CalcLibrary/CalcLibrary.csproj
*/

//step2- create CalculatorTests inside CacLibrary.tests
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

//step3- create Calculator inside CalcLibrary
namespace CalcLibrary
{
    public class Calculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}

//step 4- type "dotnet test" in terminal for output
