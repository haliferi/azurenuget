using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AzureCommon;
using Hyak.Common;
using Microsoft.Azure;
using Microsoft.Azure.Management.Network.Models;
using Microsoft.Azure.Management.Resources.Models;

namespace AzureNugetDemo
{
    partial class AzureDemo
    {
        private static T HandleResult<T>(string region, Func<T> action, Func<T, string> toStringFunc) where T : AzureOperationResponse
        {
            using (new Region(region))
            {
                try
                {
                    T result = action();

                    if (result == null)
                    {
                        throw new Exception(Log.Error(region + " result: NULL"));
                    }

                    if (result.StatusCode == HttpStatusCode.NoContent || result.StatusCode == HttpStatusCode.NotFound)
                    {
                        Log.Info(region + " result: " + result.StatusCode);
                        return null;
                    }

                    if (result.StatusCode != HttpStatusCode.OK && result.StatusCode != HttpStatusCode.Created)
                    {
                        throw new Exception(Log.Error(region + " result: " + result.StatusCode));
                    }

                    Log.Info(region + " result: " + (toStringFunc == null ? result.ToString() : toStringFunc(result)));

                    return result;
                }
                catch (CloudException e)
                {
                    var errMsg = region + " error: " + e.Message;
                    if (e.Error == null || string.IsNullOrEmpty(e.Error.Code))
                    {
                        Log.Error(errMsg);
                    }
                    else
                    {
                        Log.Error(errMsg + " - " + e.Error.Code + ": " + e.Error.Message);
                    }
                    throw;
                }
            }
        }

        private static AzureOperationResponse HandleResult(string region, Func<AzureOperationResponse> action)
        {
            return HandleResult(region, action, t => t.StatusCode.ToString());
        }

        private static ProviderRegistionResult HandleResult(string region, Func<ProviderRegistionResult> action)
        {
            return HandleResult(region, action, t => t.Provider.RegistrationState);
        }



        private string GroupBaseId => "/subscriptions/" + azureLogin.SubscriptionId + "/resourceGroups/" + demo.Group + "/";

        private ResourceId GetBaseResourceId(string item)
        {
            return new ResourceId() { Id = GroupBaseId + item };
        }

        private ResourceId GetSubnetId(string virtualNetworkName)
        {
            return GetBaseResourceId(string.Format("providers/Microsoft.Network/virtualNetworks/{0}/subnets/{0}_sub", virtualNetworkName));
        }

        private ResourceId GetNetwokSecurityGroupId(string networkName)
        {
            return GetBaseResourceId("providers/Microsoft.Network/networkSecurityGroups/" + networkName + "_sec");
        }

        private ResourceId GetPublicIpId(string publicIpName)
        {
            return GetBaseResourceId("providers/Microsoft.Network/publicIPAddresses/" + publicIpName);
        }

        private ResourceId GetVmId(string vmName)
        {
            return GetBaseResourceId("providers/Microsoft.Compute/virtualMachines/" + vmName);
        }

        private ResourceId GetNicId(string publicIpName)
        {
            return GetBaseResourceId("providers/Microsoft.Network/networkInterfaces/" + publicIpName + "_nic");
        }
    }
}
