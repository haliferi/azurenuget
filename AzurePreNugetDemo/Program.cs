using System;
using System.Collections.Generic;
using System.Linq;
using AzureCommon;

namespace AzurePreNugetDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var azureLogin = new AzureLoginUser(new DemoSubscriptionConfig());
            if (azureLogin.Authenticate())
                new AzureDemo(azureLogin).Run();

            Console.WriteLine("EOF");
            Console.ReadKey();
        }
    }
}
