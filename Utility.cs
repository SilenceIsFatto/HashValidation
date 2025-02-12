using System.Security.Cryptography;

namespace HashValidationUtility
{
    public class HashValidation
    {
        private string programName;
        private string hashFileURL;
        public string programPath;
        public int debugLevel;
        public HashValidation()
        {
            this.programName = "HashValidation";
            this.hashFileURL = GetHashURL();
            this.programPath = GetExecutionDir();
            this.debugLevel = SetDebugLevel(0);
        }

        private string GetHashURL()
        {
            // Can add UI related stuff here maybe
            string hashFileURL = "https://raw.githubusercontent.com/SilenceIsFatto/HashValidation/refs/heads/master/hashes_remote.json";

            return hashFileURL;
        }

        public void CreateDirectory(string dir)
        {
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch (Exception e)
            {
                //LogError("Directory already exists, continuing");
            }
        }

        public string FormatMiscFolder(string dir)
        {
            return "HashValidation\\" + dir;
        }

        public string FormatLogFile()
        {
            string logFileFormatted = FormatMiscFolder("Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + ".txt");
            return logFileFormatted;
        }

        public void WriteToFile(string filePath, string log)
        {
            CreateDirectory(filePath);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Append))
            {
                using (StreamWriter logStream = new StreamWriter(fileStream))
                {
                    logStream.WriteLine(log);
                }
                fileStream.Close();
            }
        }

        public void ClearFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception e)
            {
                LogError("File not found, continuing");
            }

            using (FileStream fileStream = new FileStream(filePath, FileMode.CreateNew))
            {
                fileStream.Close();
            }
        }

        public void DownloadHashFile(string fileName)
        {
            string path = FormatProgramDir(fileName);
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
                    {
                        NoCache = true,
                        NoStore = true
                    };
                    client.DefaultRequestHeaders.Add("User-Agent", "CSharpApp");
                    client.DefaultRequestHeaders.Add("Connection", "close"); // Force a new connection

                    var response = client.GetAsync(hashFileURL).Result;
                    Log($"Status Code (GitHub): {response.StatusCode}");

                    var contents = client.GetByteArrayAsync(hashFileURL).Result;
                    System.IO.File.WriteAllBytes(path, contents);
                    client.Dispose();
                }
            }
            catch (Exception e)
            {
                Log("Something went wrong whilst trying to download the remote json file.");
                LogError(e.ToString());
            }
        }

        public string FormatProgramDir(string fileDir)
        {
            return string.Format(@"{0}\{1}", programPath, fileDir);
        }

        public int SetDebugLevel(int level)
        {
            debugLevel = level;

            return debugLevel;
        }

        public int GetDebugLevel()
        {
            return debugLevel;
        }

        public string GetExecutionDir()
        {
            string executionPath = AppDomain.CurrentDomain.BaseDirectory;

            return executionPath;
        }

        public string GetFileFromDir(string dir)
        {
            try
            {
                string filename = Path.GetFileName(dir);
                return filename;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public string[] GetFilesInDir(string dir)
        {
            string[] filesInDir = [];

            try
            {
                filesInDir = Directory.GetFiles(dir);
            }
            catch (Exception e)
            {
                Log("The addons folder could not be found.");
                LogError(e.ToString());
            };

            return filesInDir;
        }

        public void Log(string message)
        {
            if (GetDebugLevel() >= 0)
            {
                string messageFormatted = string.Format("| {0} | {1} |", programName, message);
                WriteToFile(programPath + FormatLogFile(), messageFormatted);
                Console.WriteLine(messageFormatted);
            }
        }

        public void LogError(string error)
        {
            if (GetDebugLevel() >= 2)
            {
                string errorFormatted = string.Format("| {0} | Error | {1} |", programName, error);
                WriteToFile(programPath + FormatLogFile(), errorFormatted);
                Console.WriteLine(errorFormatted);
            }
        }

        public void LogLines(string[] lines)
        {
            foreach (string line in lines)
            {
                Log(line);
            }
        }

        public bool DoesFileExist(string filePath)
        {
            bool fileExists = File.Exists(filePath);

            return fileExists;
        }

        public string FileToMD5(string filePath)
        {
            bool fileExists = DoesFileExist(filePath);

            if (!fileExists)
            {
                string errorMessage = string.Format("[ERROR] '{0}' could not be found.", filePath);
                return errorMessage;
            }

            try
            {
                using (var md5 = MD5.Create())
                {
                    using (FileStream stream = File.OpenRead(filePath))
                    {
                        var hash = md5.ComputeHash(stream);
                        return Convert.ToHexStringLower(hash);
                    }
                }
            }
            catch (Exception e)
            {
                Log("File could not be accessed");
                LogError(e.ToString());
                return string.Format("File could not be accessed");
            }
        }
    }
}