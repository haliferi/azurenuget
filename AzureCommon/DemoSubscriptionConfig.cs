using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCommon
{
    public class DemoSubscriptionConfig : ISubscriptionConfig
    {
        public string SubscriptionId => "{subscription guid}";

        public string Tenant => "{youraccount}.onmicrosoft.com";

        public string ClientId => "{azure ad application client id}";

        public string UserName => "{youradmin}@{youraccount}.onmicrosoft.com";

        public string Password => "{YourPassword}";
        
    }
}
