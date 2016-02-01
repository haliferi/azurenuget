using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using AzureCommon;
using Microsoft.Azure.Management.Compute;
using Microsoft.Azure.Management.Compute.Models;
using Microsoft.Azure.Management.Network;
using Microsoft.Azure.Management.Network.Models;
using Microsoft.Azure.Management.Storage;
using Microsoft.Azure.Management.Storage.Models;

namespace AzureNugetDemo
{
    internal partial class AzureDemo
    {
        private DemoAzureEnv demo = new DemoAzureEnv();

        private IAzureLogin<SubscriptionCloudCredentials> azureLogin;

        public AzureDemo(IAzureLogin<SubscriptionCloudCredentials> azureLogin)
        {
            this.azureLogin = azureLogin;
        }

        public void Run()
        {
            var getMyIpTask = QueueGetMyIpTask();

            CreateResourceGroup();

            Parallel.Invoke(
                CreateStorageAccount,
                () => CreateVirtualNetwork("LocalRdp", new [] { getMyIpTask.Result }, new int[] { 3389 })
            );

            var vms = new[] { "DemoVm1" };

            Parallel.ForEach(vms, vm =>
            {
                CreateVirtualMachine(vm, AzureVmSize.Standarad_A0, AddPublicIp(vm + "Ip"));
            });

            ResizeVirtualMachine(vms[0], AzureVmSize.Basic_A0);

            StopVirtualMachine(vms[0]);
        }

        private static Task<string> QueueGetMyIpTask()
        {
            return Task.Factory.StartNew(() =>
            {
                using (new Region("Get Public IP address"))
                {
                    return new System.Net.WebClient().DownloadString("https://api.ipify.org");
                }
            });
        }

        private void CreateResourceGroup()
        {
            azureLogin.Authenticate();
            using (var resourceManagementClient = new ResourceManagementClient(azureLogin.Credentials))
            using (var resourceManagementClient2 = new ResourceManagementClient(azureLogin.Credentials))
            using (var resourceManagementClient3 = new ResourceManagementClient(azureLogin.Credentials))
            {
                var rg = new ResourceGroup()
                {
                    Location = demo.Location,
                };
                HandleResult($"CreateOrUpdate ResourceGroup {demo.Group} @ {demo.Location}",
                    () => resourceManagementClient.ResourceGroups.CreateOrUpdate(demo.Group, rg));

                Parallel.Invoke(
                    () => HandleResult("Register Microsoft.Storage", () => resourceManagementClient.Providers.Register("Microsoft.Storage")),
                    () => HandleResult("Register Microsoft.Network", () => resourceManagementClient2.Providers.Register("Microsoft.Network")),
                    () => HandleResult("Register Microsoft.Compute", () => resourceManagementClient3.Providers.Register("Microsoft.Compute"))
                );
            }
        }

        public void CreateStorageAccount()
        {
            using (var storageManagementClient = new StorageManagementClient(azureLogin.Credentials))
            {
                var sacp = new StorageAccountCreateParameters()
                {
                    AccountType = AccountType.StandardLRS,
                    Location = demo.Location,
                };
                HandleResult("Create Storage Account", 
                    () => storageManagementClient.StorageAccounts.Create(demo.Group, demo.Storage, sacp),
                    t => t.Status.ToString());
            }
        }

        public void CreateVirtualNetwork(string ruleName, string[] sourceAddresses, int[] destPorts)
        {
            azureLogin.Authenticate();
            using (var networkClient = new NetworkResourceProviderClient(azureLogin.Credentials))
            {
                var subnetName = demo.Network + "_sub";
                var netSecName = demo.Network + "_sec";

                var nsg = new NetworkSecurityGroup()
                {
                    Name = netSecName,
                    Location = demo.Location,
                    SecurityRules = new List<SecurityRule>(),
                };
                
                var prio = 0;
                foreach (var sourceAddress in sourceAddresses)
                {
                    foreach (var destPort in destPorts)
                    {
                        prio += 100;
                        nsg.SecurityRules.Add(new SecurityRule()
                        {
                            Name = ruleName + "_" + destPort + "_" + sourceAddress,
                            Access = "Allow",
                            Direction = "Inbound",
                            Protocol = "Tcp",
                            SourceAddressPrefix = sourceAddress,
                            SourcePortRange = "*",
                            DestinationAddressPrefix = "*",
                            DestinationPortRange = destPort > 0 ? destPort.ToString() : "*",
                            Priority = prio,
                        });
                    }
                }

                HandleResult("Create Network Security Group", 
                    () => networkClient.NetworkSecurityGroups.CreateOrUpdate(demo.Group, netSecName, nsg));

                var vn = new VirtualNetwork()
                {
                    Location = demo.Location,
                    AddressSpace = new AddressSpace()
                    {
                        AddressPrefixes = new List<string> { demo.AddressSpace },
                    },
                    Subnets = new List<Subnet>()
                    {
                        new Subnet()
                        {
                            Name = subnetName,
                            AddressPrefix = demo.AddressSpace,
                            NetworkSecurityGroup = GetNetwokSecurityGroupId(demo.Network),
                        }
                    },
                };
                HandleResult("Create Virtual Network", 
                    () => networkClient.VirtualNetworks.CreateOrUpdate(demo.Group, demo.Network, vn));
            }
        }


        public string AddPublicIp(string publicIpName)
        {
            azureLogin.Authenticate();
            using (var networkClient = new NetworkResourceProviderClient(azureLogin.Credentials))
            {
                var nicName = publicIpName + "_nic";

                var pia = new PublicIpAddress()
                {
                    Location = demo.Location,
                    PublicIpAllocationMethod = "Dynamic",
                };
                HandleResult("Create Public IP", () => networkClient.PublicIpAddresses.CreateOrUpdate(demo.Group, publicIpName, pia));

                var ni = new NetworkInterface()
                {
                    Name = nicName,
                    Location = demo.Location,
                    IpConfigurations = new List<NetworkInterfaceIpConfiguration>()
                    {
                        new NetworkInterfaceIpConfiguration()
                        {
                            Name = nicName + "config",
                            PublicIpAddress = GetPublicIpId(publicIpName),
                            Subnet = GetSubnetId(demo.Network),
                        }
                    }
                };
                HandleResult("Create Network Interface", () => networkClient.NetworkInterfaces.CreateOrUpdate(demo.Group, nicName, ni));
            }
            return publicIpName;
        }

        public void CreateVirtualMachine(string virtualMachineName, AzureVmSize size, string ipName, bool startVm = true)
        {
            azureLogin.Authenticate();
            using (var computeManagementClient = new ComputeManagementClient(azureLogin.Credentials))
            {
                var osdiskName = virtualMachineName + "_osdisk";

                var vm = new VirtualMachine()
                {
                    Name = virtualMachineName,
                    Location = demo.Location,
                    OSProfile = new OSProfile()
                    {
                        AdminUsername = demo.VmAdminUserName,
                        AdminPassword = demo.VmAdminPassword,
                        ComputerName = virtualMachineName,
                        WindowsConfiguration = new WindowsConfiguration()
                        {
                            EnableAutomaticUpdates = true,
                            ProvisionVMAgent = true,
                        },
                    },
                    HardwareProfile = new HardwareProfile()
                    {
                        VirtualMachineSize = size
                    },
                    NetworkProfile = new NetworkProfile()
                    {
                        NetworkInterfaces = new List<NetworkInterfaceReference>()
                        {
                            new NetworkInterfaceReference
                            {
                                ReferenceUri = GetNicId(ipName).Id,
                            },
                        },
                    },
                    StorageProfile = new StorageProfile()
                    {
                        ImageReference = new ImageReference()
                        {
                            Publisher = "MicrosoftWindowsServer",
                            Offer = "WindowsServer",
                            Sku = "2012-R2-Datacenter",
                            Version = "latest",
                        },
                        OSDisk = new OSDisk
                        {
                            Name = osdiskName,
                            CreateOption = "FromImage",
                            Caching = "ReadWrite",
                            VirtualHardDisk = new VirtualHardDisk()
                            {
                                Uri = string.Format("http://{0}.blob.core.windows.net/vhds/{1}.vhd", demo.Storage, osdiskName),
                            },
                        },
                    },
                };

                HandleResult("Create VM", () => computeManagementClient.VirtualMachines.CreateOrUpdate(demo.Group, vm));

                var vmInstance = HandleResult("Get VM instance " + virtualMachineName,
                    () => computeManagementClient.VirtualMachines.GetWithInstanceView(demo.Group, virtualMachineName),
                    t => t.VirtualMachine.ProvisioningState);
                var stat = vmInstance.VirtualMachine.InstanceView.Statuses.FirstOrDefault(s => s.Code.StartsWith("PowerState"));
                bool vmStarted = stat != null && stat.DisplayStatus == "VM running";

                if (startVm && !vmStarted)
                {
                    StartVirtualMachine(virtualMachineName);
                }
            }
        }

        public void DeleteVirtualMachine(string virtualMachineName)
        {
            azureLogin.Authenticate();
            using (var computeManagementClient = new ComputeManagementClient(azureLogin.Credentials))
            {
                HandleResult($"Delete VM '{virtualMachineName}'", 
                    () => computeManagementClient.VirtualMachines.Delete(demo.Group, virtualMachineName));
            }
        }

        public void StopVirtualMachine(string virtualMachineName)
        {
            azureLogin.Authenticate();
            using (var computeManagementClient = new ComputeManagementClient(azureLogin.Credentials))
            {
                HandleResult($"Stop VM '{virtualMachineName}'", 
                    () => computeManagementClient.VirtualMachines.Deallocate(demo.Group, virtualMachineName));
            }
        }

        public void StartVirtualMachine(string virtualMachineName)
        {
            azureLogin.Authenticate();
            using (var computeManagementClient = new ComputeManagementClient(azureLogin.Credentials))
            {
                HandleResult($"Start VM '{virtualMachineName}'", 
                    () => computeManagementClient.VirtualMachines.Start(demo.Group, virtualMachineName));
            }
        }

        public void ResizeVirtualMachine(string virtualMachineName, AzureVmSize size)
        {
            azureLogin.Authenticate();
            using (var computeManagementClient = new ComputeManagementClient(azureLogin.Credentials))
            {
                var vm = new VirtualMachine()
                {
                    Name = virtualMachineName,
                    Location = demo.Location,
                    HardwareProfile = new HardwareProfile()
                    {
                        VirtualMachineSize = size,
                    },
                };
                HandleResult($"Resize VM '{virtualMachineName}' to {size}", 
                    () => computeManagementClient.VirtualMachines.CreateOrUpdate(demo.Group, vm));
            }
        }

    }
}
