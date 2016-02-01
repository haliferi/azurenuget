using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCommon
{
    public static class Log
    {
        public static void Info(string message)
        {
            Console.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), message);
        }

        public static string Error(string message)
        {
            lock (Console.Out)
            {
                var c = Console.ForegroundColor;
                Info(message);
                Console.ForegroundColor = ConsoleColor.Red;
            }
            return message;
        }

    }
}
