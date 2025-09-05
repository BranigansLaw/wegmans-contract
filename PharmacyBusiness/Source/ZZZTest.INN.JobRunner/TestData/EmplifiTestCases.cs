using CaseServiceWrapper;
using Newtonsoft.Json;

namespace ZZZTest.INN.JobRunner.TestData
{
    public class EmplifiTestCases
    {
        public List<Case> Cases20240613143000 { get; private set; }
        public List<Case> Cases20240614143000 { get; private set; }
        public List<Case> Cases20240617143000 { get; private set; }
        
        public List<Case> Cases20250113143000 { get; private set; }
        public List<Case> Cases20250114143000 { get; private set; }
        public List<Case> Cases20250117143000 { get; private set; }

        public EmplifiTestCases()
        {
            Cases20240613143000 = new List<Case>();
            Cases20240614143000 = new List<Case>();
            Cases20240617143000 = new List<Case>();

            Cases20250113143000 = new List<Case>();
            Cases20250114143000 = new List<Case>();
            Cases20250117143000 = new List<Case>();

            //NOTE: All this test data is NOT Production data. It is test data that was created for testing purposes by Meslissa Hatch.

            //Run Date 20240613143000
            string jsonFilePath = "../../../../ZZZTest.INN.JobRunner/TestData/DataFiles/"; //"./DataFiles/";
            DirectoryInfo jsonDirectory = new DirectoryInfo(jsonFilePath);
            FileInfo[] jsonFiles = jsonDirectory.GetFiles("Cases_20240613143000.json");
            foreach (FileInfo jsonFile in jsonFiles)
            {
                string fileContents = File.ReadAllText(Path.Combine(jsonFilePath + jsonFile.Name));
                List<Case> cases = JsonConvert.DeserializeObject<List<Case>>(fileContents)!;
                Cases20240613143000.AddRange(cases);
            }

            //Run Date 20240614143000
            jsonFiles = jsonDirectory.GetFiles("Cases_20240614143000.json");
            foreach (FileInfo jsonFile in jsonFiles)
            {
                string fileContents = File.ReadAllText(Path.Combine(jsonFilePath + jsonFile.Name));
                List<Case> cases = JsonConvert.DeserializeObject<List<Case>>(fileContents)!;
                Cases20240614143000.AddRange(cases);
            }

            //Run Date 20240617143000
            jsonFiles = jsonDirectory.GetFiles("Cases_20240617143000.json");
            foreach (FileInfo jsonFile in jsonFiles)
            {
                string fileContents = File.ReadAllText(Path.Combine(jsonFilePath + jsonFile.Name));
                List<Case> cases = JsonConvert.DeserializeObject<List<Case>>(fileContents)!;
                Cases20240617143000.AddRange(cases);
            }

            //Run Date 20250113143000
            jsonFiles = jsonDirectory.GetFiles("Cases_20250113143000.json");
            foreach (FileInfo jsonFile in jsonFiles)
            {
                string fileContents = File.ReadAllText(Path.Combine(jsonFilePath + jsonFile.Name));
                List<Case> cases = JsonConvert.DeserializeObject<List<Case>>(fileContents)!;
                Cases20250113143000.AddRange(cases);
            }

            //Run Date 20250114143000
            jsonFiles = jsonDirectory.GetFiles("Cases_20250114143000.json");
            foreach (FileInfo jsonFile in jsonFiles)
            {
                string fileContents = File.ReadAllText(Path.Combine(jsonFilePath + jsonFile.Name));
                List<Case> cases = JsonConvert.DeserializeObject<List<Case>>(fileContents)!;
                Cases20250114143000.AddRange(cases);
            }

            //Run Date 20250117143000
            jsonFiles = jsonDirectory.GetFiles("Cases_20250117143000.json");
            foreach (FileInfo jsonFile in jsonFiles)
            {
                string fileContents = File.ReadAllText(Path.Combine(jsonFilePath + jsonFile.Name));
                List<Case> cases = JsonConvert.DeserializeObject<List<Case>>(fileContents)!;
                Cases20250117143000.AddRange(cases);
            }
        }
    }
}
