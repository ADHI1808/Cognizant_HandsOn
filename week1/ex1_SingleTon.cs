using System;
public class Logger
{
    private static Logger instance;
    private Logger()
    {
        Console.WriteLine("Logger initialized.");
    }
    public static Logger GetInstance()
    {
        if (instance == null)
        {
            instance = new Logger();
        }
        return instance;
    }
    public void Log(string message)
    {
        Console.WriteLine("Log: " + message);
    }
    public static void Main(string[] args)
    {
        Logger logger1 = Logger.GetInstance();
        Logger logger2 = Logger.GetInstance();

        logger1.Log("This is the first log message.");
        logger2.Log("This is the second log message.");

        if (object.ReferenceEquals(logger1, logger2))
        {
            Console.WriteLine("Both logger1 and logger2 are the same instance.");
        }
        else
        {
            Console.WriteLine("Different instances exist! Singleton failed.");
        }

        Console.ReadLine();
    }
}
