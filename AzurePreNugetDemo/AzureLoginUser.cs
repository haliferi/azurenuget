﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using AzureCommon;

namespace AzurePreNugetDemo
{
    public class AzureLoginUser : AzureLoginUserBase, IAzureLogin<ServiceClientCredentials>
    {
        public ServiceClientCredentials Credentials { get; private set; }

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
            var token = authenticationContext.AcquireTokenAsync("https://management.core.windows.net/", clientId, userCredential).Result;
            if (token == null)
                return false;

            expiresOn = token.ExpiresOn.UtcDateTime;
            AccessToken = token.AccessToken;
            Credentials = new TokenCredentials(token.AccessToken);
            return true;
        }
    }
}
