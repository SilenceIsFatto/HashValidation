using HashValidationUtility;
using HashValidationJson;

namespace HashValidationHash
{
    public class HashValidation
    {
        HashValidationUtility.HashValidation HVU;
        HashValidationJson.HashValidation HVJ;
        private int debugLevel;
        private string programPath;
        public HashValidation()
        {
            this.HVU = new();
            this.HVJ = new();
            this.debugLevel = HVU.debugLevel;
            this.programPath = HVU.programPath;
        }

        private bool CheckHashMatch(string firstHash, string secondHash)
        {
            return (firstHash == secondHash);
        }

        private bool isModEnvironment(string[] filesInDir)
        {
            bool inModEnvironment = false;

            foreach (string fileDir in filesInDir)
            {
                if (fileDir.Contains("mod.cpp"))
                {
                    inModEnvironment = true;
                }
            }

            return inModEnvironment;
        }

        private void ValidateHashes()
        {
            string hashFileLocal = HVU.FormatMiscFolder("hashes_local.json");
            string hashFileRemote = HVU.FormatMiscFolder("hashes_remote.json");

            HVJ.WriteHashesToJson(hashFileLocal);
            HVU.DownloadHashFile(hashFileRemote);

            string[] addonDirectory = HVU.GetFilesInDir(programPath + "addons");

            string fullDir = HVU.FormatProgramDir(hashFileRemote);
            Dictionary<string, string> hashes = JsonFileReader.Read<Dictionary<string, string>>(fullDir);

            List<string> successfulHashes = new();
            List<string> failedHashes = new();

            foreach (string fileDir in addonDirectory)
            {
                string fileHash = HVU.FileToMD5(fileDir);
                string fileName = HVU.GetFileFromDir(fileDir);
                string fileHashCompare = "";

                if (hashes.ContainsKey(fileName))
                {
                    fileHashCompare = hashes[fileName];
                }
                else
                {
                    continue;
                }

                bool doHashesMatch = CheckHashMatch(fileHash, fileHashCompare);

                HVU.Log($"Checking if {fileName} matches the json hash...");
                HVU.Log($"{fileName} hash (File): {fileHash}");
                HVU.Log($"{fileName} hash (JSON): {fileHashCompare}");
                if (doHashesMatch)
                {
                    HVU.Log("Hashes match!");
                    successfulHashes.Add(fileName);
                }
                else
                {
                    HVU.Log("Hashes do not match.");
                    failedHashes.Add(fileName);
                };

                Console.WriteLine("");
                Console.WriteLine("");
            }

            HVU.Log($"Successful hashes: {successfulHashes.Count}. Failed Hashes: {failedHashes.Count}");
            HVU.Log($"The following files were validated:");
            Console.WriteLine("");
            HVU.LogLines(successfulHashes.ToArray());
            Console.WriteLine("");
            HVU.Log($"The following files failed to validate:");
            Console.WriteLine("");
            HVU.LogLines(failedHashes.ToArray());
        }

        public void HashValidationInit()
        {
            Console.WriteLine("Current debug level is {0}.\n\nCurrent working directory is '{1}'.\n", debugLevel, programPath);

            string[] filesInDir = HVU.GetFilesInDir(programPath);

            bool inModEnv = isModEnvironment(filesInDir);

            if (inModEnv)
            {
                HVU.Log("Mod Environment Detected");
                ValidateHashes();
            }
            else
            {
                HVU.LogLines(["Mod environment was not detected!", "The program should be placed in the same directory as the mod.cpp."]);
            }

            //if (debugLevel >= 1) { };
            if (debugLevel >= 2) { Console.WriteLine(""); }
        }
    }
}