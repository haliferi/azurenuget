using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCommon
{
    public class DemoAzureEnv
    {
        public string Group = "{DemoGroup}";

        public AzureLocation Location = AzureLocation.NorthEurope;

        public string Storage = "{demostorage}";

        public string Network = "demonet";

        public string AddressSpace = "10.0.0.0/16";

        public string VmAdminUserName = "Administrator";

        public string VmAdminPassword = "{YourPassword}";
    }
}
