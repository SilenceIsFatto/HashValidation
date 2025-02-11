using System.Security.Cryptography;
using HashValidationUtility;
using HashValidationJson;
using HashValidationHash;
using System.Text.Json;

namespace HashValidationCore
{
    public class HashValidation
    {
        HashValidationHash.HashValidation HVH;
        HashValidationUtility.HashValidation HVU;
        HashValidationJson.HashValidation HVJ;
        private int debugLevel;
        private string programPath;
        public HashValidation()
        {
            this.HVH = new();
            this.HVU = new();
            this.HVJ = new();
            this.debugLevel = HVU.debugLevel;
            this.programPath = HVU.programPath;
        }

        static void Main()
        {
            HashValidation HV = new();

            HV.HVU.CreateDirectory(HV.HVU.FormatProgramDir("HashValidation"));

            HV.HVU.ClearFile(HV.programPath + HV.HVU.FormatLogFile());
            HV.HVH.HashValidationInit();

            HV.HVU.Log("Finished. Press any key to exit.");

            Console.ReadKey();
        }
    }
}