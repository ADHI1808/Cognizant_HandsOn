using System;
class FinancialForecastingWithInput
{
    static void Main()
    {
        Console.Write("Enter Present Value (₹): ");
        double presentValue = Convert.ToDouble(Console.ReadLine());
        Console.Write("Enter Annual Growth Rate (%): ");
        double growthRatePercent = Convert.ToDouble(Console.ReadLine());
        Console.Write("Enter Number of Years: ");
        int years = Convert.ToInt32(Console.ReadLine());
        double growthRate = growthRatePercent / 100;
        double futureValue = CalculateFutureValue(presentValue, growthRate, years);
        Console.WriteLine("Future Value after " + years + " years: ₹" + futureValue.ToString("F2"));
    }
    static double CalculateFutureValue(double presentValue, double growthRate, int years)
    {
        if (years == 0)
            return presentValue;
        return (1 + growthRate) * CalculateFutureValue(presentValue, growthRate, years - 1);
    }
/*
    static double CalculateFutureValueIterative(double presentValue, double growthRate, int years)
    {
          double result = presentValue;
          for (int i = 0; i < years; i++)
               result *= (1 + growthRate);
          return result;
    }
*/
/* 
    static double CalculateFutureValueOptimal(double presentValue, double growthRate, int years)
    {
        return presentValue * Math.Pow(1 + growthRate, years);
    }
*/
}
