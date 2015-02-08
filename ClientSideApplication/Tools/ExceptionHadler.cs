using System;

namespace ClientSideApplication.Tools
{
    class ExceptionHadler
    {
        public static void ErrorMessage()
        {
            Console.WriteLine("Something went wrong...");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
