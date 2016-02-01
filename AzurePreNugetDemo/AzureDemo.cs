using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Compute;
using Microsoft.Azure.Management.Compute.Models;
using Microsoft.Rest;
using AzureCommon;

namespace AzurePreNugetDemo
{
    internal partial class AzureDemo
    {
        private DemoAzureEnv demo = new DemoAzureEnv();

        private IAzureLogin<ServiceClientCredentials> azureLogin;

        public AzureDemo(IAzureLogin<ServiceClientCredentials> azureLogin)
        {
            this.azureLogin = azureLogin;
        }

        public void Run()
        {
            ListVMs();
        }

        public void ListVMs()
        {
            using (var computeManagementClient = new ComputeManagementClient(azureLogin.Credentials))
            {
                computeManagementClient.SubscriptionId = azureLogin.SubscriptionId;

                var result = HandleResult("List VMs in " + demo.Group,
                    () => computeManagementClient.VirtualMachines.List(demo.Group),
                    t => "Count: " + t.Count());

                if (result == null)
                    return;

                var azureVirtualMachines = new List<VirtualMachine>(result);
                while (result.NextPageLink != null)
                {
                    result = HandleResult("List Next VMs",
                        () => computeManagementClient.VirtualMachines.ListNext(result.NextPageLink),
                        t => "Count: " + t.Count());

                    if (result == null)
                        break;

                    azureVirtualMachines.AddRange(result);
                }

                var virtualMachines = azureVirtualMachines.Select(v => v.Name).ToList();
                Parallel.For(0, virtualMachines.Count, i =>
                {
                    var vmName = virtualMachines[i];
                    var vmInstance = HandleResult("Get VM instance " + vmName,
                        () => computeManagementClient.VirtualMachines.Get(demo.Group, vmName, "InstanceView"),
                        t => t.ProvisioningState);

                    if (vmInstance == null)
                        return;

                    var stat = vmInstance.InstanceView.Statuses.FirstOrDefault(s => s.Code.StartsWith("PowerState"));
                    Log.Info(string.Format("{0}: Size: {1}, State: {2}, Provisioning: {3}", 
                        vmName,
                        vmInstance.HardwareProfile.VmSize, 
                        stat == null ? "Unknown" : stat.DisplayStatus,
                        vmInstance.ProvisioningState));
                });
            }
        }

    }
}
