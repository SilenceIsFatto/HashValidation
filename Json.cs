using System.Text.Json;
using System.Text.Json.Serialization;

namespace HashValidationJson
{
    public static class JsonFileReader
    {
        public static T Read<T>(string filePath)
        {
            string text = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(text);
        }
    }

    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(Dictionary<string, string>))] // Adjust type accordingly
    internal partial class JsonContext : JsonSerializerContext
    {
    }

    public class HashValidation
    {
        HashValidationUtility.HashValidation HVU;
        public HashValidation()
        {
            this.HVU = new();
        }

        public void WriteHashesToJson(string outputName)
        {
            Dictionary<string, string> fileHashes = new Dictionary<string, string> { };

            foreach (string fileDir in HVU.GetFilesInDir(HVU.FormatProgramDir("addons")))
            {
                fileHashes.Add(HVU.GetFileFromDir(fileDir), HVU.FileToMD5(fileDir));
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonSerialized = JsonSerializer.Serialize(fileHashes, JsonContext.Default.DictionaryStringString);

            WriteJson(jsonSerialized, outputName);
        }

        public string WriteJson(string json, string outputName)
        {
            try
            {
                string fullDir = HVU.FormatProgramDir(outputName);
                File.WriteAllText(fullDir, json);

                return "";
            }
            catch (Exception e)
            {
                HVU.LogError(e.ToString());
                return string.Format("Json file could not be created");
            }
        }
    }
}
