using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using AzureCommon;

namespace AzureNugetDemo
{
    public class AzureLoginUser : AzureLoginUserBase, IAzureLogin<SubscriptionCloudCredentials>
    {
        public SubscriptionCloudCredentials Credentials { get; private set; }

        public AzureLoginUser(string subscriptionId, string clientId, string tenant, string userName, string password)
            : base(subscriptionId, clientId, tenant, userName, password)
        {
        }

        public AzureLoginUser(ISubscriptionConfig subscriptionConfig)
            : base(subscriptionConfig)
        {
        }

        protected override bool AuthenticateBase()
        {
            var authenticationContext = new AuthenticationContext("https://login.windows.net/" + tenant);
            var token = authenticationContext.AcquireToken("https://management.core.windows.net/", clientId, userCredential);
            if (token == null)
                return false;

            expiresOn = token.ExpiresOn.UtcDateTime;
            AccessToken = token.AccessToken;
            Credentials = new TokenCloudCredentials(SubscriptionId, token.AccessToken);
            return true;
        }

    }
}
