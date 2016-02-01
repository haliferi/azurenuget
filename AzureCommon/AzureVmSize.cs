using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCommon
{
    public struct AzureVmSize
    {
        public static readonly AzureVmSize Basic_A0 = "Basic_A0";
        public static readonly AzureVmSize Basic_A1 = "Basic_A1";
        public static readonly AzureVmSize Basic_A2 = "Basic_A2";
        public static readonly AzureVmSize Basic_A3 = "Basic_A3";
        public static readonly AzureVmSize Basic_A4 = "Basic_A4";

        public static readonly AzureVmSize Standarad_A0 = "Standard_A0";
        public static readonly AzureVmSize Standarad_A1 = "Standard_A1";
        public static readonly AzureVmSize Standarad_A2 = "Standard_A2";
        public static readonly AzureVmSize Standarad_A3 = "Standard_A3";
        public static readonly AzureVmSize Standarad_A4 = "Standard_A4";
        public static readonly AzureVmSize Standarad_A5 = "Standard_A5";
        public static readonly AzureVmSize Standarad_A6 = "Standard_A6";
        public static readonly AzureVmSize Standarad_A7 = "Standard_A7";

        public static readonly AzureVmSize Standard_D1 = "Standard_D1";
        public static readonly AzureVmSize Standard_D2 = "Standard_D2";
        public static readonly AzureVmSize Standard_D3 = "Standard_D3";
        public static readonly AzureVmSize Standard_D4 = "Standard_D4";
        public static readonly AzureVmSize Standard_D11 = "Standard_D11";
        public static readonly AzureVmSize Standard_D12 = "Standard_D12";
        public static readonly AzureVmSize Standard_D13 = "Standard_D13";
        public static readonly AzureVmSize Standard_D14 = "Standard_D14";

        public static readonly AzureVmSize Standard_D1_v2 = "Standard_D1_v2";
        public static readonly AzureVmSize Standard_D2_v2 = "Standard_D2_v2";
        public static readonly AzureVmSize Standard_D3_v2 = "Standard_D3_v2";
        public static readonly AzureVmSize Standard_D4_v2 = "Standard_D4_v2";
        public static readonly AzureVmSize Standard_D11_v2 = "Standard_D11_v2";
        public static readonly AzureVmSize Standard_D12_v2 = "Standard_D12_v2";
        public static readonly AzureVmSize Standard_D13_v2 = "Standard_D13_v2";
        public static readonly AzureVmSize Standard_D14_v2 = "Standard_D14_v2";

        public static readonly AzureVmSize Standard_DS1 = "Standard_DS1";
        public static readonly AzureVmSize Standard_DS2 = "Standard_DS2";
        public static readonly AzureVmSize Standard_DS3 = "Standard_DS3";
        public static readonly AzureVmSize Standard_DS4 = "Standard_DS4";
        public static readonly AzureVmSize Standard_DS11 = "Standard_DS11";
        public static readonly AzureVmSize Standard_DS12 = "Standard_DS12";
        public static readonly AzureVmSize Standard_DS13 = "Standard_DS13";
        public static readonly AzureVmSize Standard_DS14 = "Standard_DS14";

        public static readonly AzureVmSize Standard_G1 = "Standard_G1";
        public static readonly AzureVmSize Standard_G2 = "Standard_G2";
        public static readonly AzureVmSize Standard_G3 = "Standard_G3";
        public static readonly AzureVmSize Standard_G4 = "Standard_G4";
        public static readonly AzureVmSize Standard_G5 = "Standard_G5";

        public static readonly AzureVmSize Standard_GS1 = "Standard_GS1";
        public static readonly AzureVmSize Standard_GS2 = "Standard_GS2";
        public static readonly AzureVmSize Standard_GS3 = "Standard_GS3";
        public static readonly AzureVmSize Standard_GS4 = "Standard_GS4";
        public static readonly AzureVmSize Standard_GS5 = "Standard_GS5";

        public static readonly AzureVmSize Standarad_A8 = "Standard_A8";
        public static readonly AzureVmSize Standarad_A9 = "Standard_A9";

        private readonly string value;

        public AzureVmSize(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value;
        }

        public static implicit operator string (AzureVmSize stringEnum) { return stringEnum.value; }

        public static implicit operator AzureVmSize(string value) { return new AzureVmSize(value); }
    }
}
