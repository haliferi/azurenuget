using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCommon
{
    public struct AzureLocation
    {
        public static readonly AzureLocation EastAsia = "eastasia";
        public static readonly AzureLocation SoutheastAsia = "southeastasia";
        public static readonly AzureLocation CentralUS = "centralus";
        public static readonly AzureLocation EastUS = "eastus";
        public static readonly AzureLocation EastUS2 = "eastus2";
        public static readonly AzureLocation WestUS = "westus";
        public static readonly AzureLocation NorthCentralUS = "northcentralus";
        public static readonly AzureLocation SouthCentralUS = "southcentralus";
        public static readonly AzureLocation NorthEurope = "northeurope";
        public static readonly AzureLocation WestEurope = "westeurope";
        public static readonly AzureLocation JapanWest = "japanwest";
        public static readonly AzureLocation JapanEast = "japaneast";

        public static readonly AzureLocation BrazilSouth = "brazilsouth";
        public static readonly AzureLocation AustraliaEast = "australiaeast";
        public static readonly AzureLocation AustraliaSoutheast = "australiasoutheast";
        public static readonly AzureLocation SouthIndia = "southindia";
        public static readonly AzureLocation CentralIndia = "centralindia";
        public static readonly AzureLocation WestIndia = "westindia";

        private readonly string value;

        public AzureLocation(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value;
        }

        public static implicit operator string (AzureLocation stringEnum) { return stringEnum.value; }

        public static implicit operator AzureLocation(string value) { return new AzureLocation(value); }
    }
}
