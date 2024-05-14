using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace PatientManager
{
    public class Patient
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string CI { get; set; }
        public string BloodGroup { get; set; }
    }
    public class PatientManager
    {
        private readonly string _filePath;

        public PatientManager(IConfiguration configuration)
        {
            _filePath = configuration["PatientFileSettings:FilePath"];
        }

        public List<Patient> ReadPatientsFromFile()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Patient>();
            }

            string json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<Patient>>(json);
        }

        public void WritePatientsToFile(List<Patient> patients)
        {
            string json = JsonConvert.SerializeObject(patients);
            File.WriteAllText(_filePath, json);
        }
    }
}
