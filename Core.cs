namespace HashValidationCore
{
    public class HashValidation
    {
        HashValidationHash.HashValidation HVH;
        HashValidationUtility.HashValidation HVU;
        HashValidationJson.HashValidation HVJ;
        public HashValidation()
        {
            this.HVH = new();
            this.HVU = new();
            this.HVJ = new();
        }

        static void Main()
        {
            HashValidation HV = new();

            HV.HVU.CreateDirectory(HV.HVU.FormatProgramDir("HashValidation"));

            HV.HVU.ClearFile(HV.HVU.programPath + HV.HVU.FormatLogFile());
            HV.HVH.HashValidationInit();

            Console.WriteLine("");
            HV.HVU.Log("Finished. Press any key to exit.");

            Console.ReadKey();
        }
    }
}