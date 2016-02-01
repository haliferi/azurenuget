using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCommon
{
    public interface IAzureLogin<out T>
    {
        T Credentials { get; }

        string SubscriptionId { get; }

        string AccessToken { get; }

        bool Authenticate();
    }

}
