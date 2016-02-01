using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AzureCommon
{

    public abstract class AzureLoginUserBase
    {
        public static TimeSpan ReloginSafetyTime = TimeSpan.FromMinutes(5);

        public string SubscriptionId { get; }

        public string AccessToken { get; protected set; }

        protected DateTime expiresOn;

        protected readonly string clientId;
        protected readonly string tenant;
        protected readonly UserCredential userCredential;

        private readonly object lockObject = new object();

        protected AzureLoginUserBase(string subscriptionId, string clientId, string tenant, string userName, string password)
        {
            SubscriptionId = subscriptionId;
            this.clientId = clientId;
            this.tenant = tenant;
            userCredential = new UserCredential(userName, password);
        }

        protected AzureLoginUserBase(ISubscriptionConfig subscriptionConfig)
            : this(subscriptionConfig.SubscriptionId, subscriptionConfig.ClientId, subscriptionConfig.Tenant,
                  subscriptionConfig.UserName, subscriptionConfig.Password)
        {
        }

        public bool Authenticate()
        {
            lock (lockObject)
            {
                if (expiresOn > DateTime.UtcNow + ReloginSafetyTime)
                    return true;

                using (new Region($"Authenticating with User {userCredential.UserName}"))
                {
                    var result = AuthenticateBase();
                    if (!result)
                    {
                        Log.Error("Cannot authenticate!");
                    }
                    else
                    {
                        Log.Info($"Authenticated successfully! Expires in {expiresOn - DateTime.UtcNow}");
                    }
                    return result;
                }
            }
        }

        protected abstract bool AuthenticateBase();
    }
}
