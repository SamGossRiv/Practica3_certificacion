using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using PatientManager;

namespace Patients.Controllers
{

    public class Patient
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string CI { get; set; }
        public string BloodGroup { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private const string FilePath = "patients.json";

        [HttpGet]
        public IActionResult GetPatients()
        {
            try
            {
                var patients = ReadPatientsFromFile();
                return Ok(patients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{ci}")]
        public IActionResult GetPatientByCI(string ci)
        {
            try
            {
                var patients = ReadPatientsFromFile();
                var patient = patients.Find(p => p.CI == ci);
                if (patient == null)
                {
                    return NotFound("Patient not found");
                }

                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult CreatePatient([FromBody] Patient patient)
        {
            try
            {
                var bloodGroups = new List<string> { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
                var random = new Random();
                patient.BloodGroup = bloodGroups[random.Next(bloodGroups.Count)];

                var patients = ReadPatientsFromFile();
                patients.Add(patient);
                WritePatientsToFile(patients);

                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{ci}")]
        public IActionResult UpdatePatient(string ci, [FromBody] Patient patient)
        {
            try
            {
                var patients = ReadPatientsFromFile();
                var existingPatient = patients.Find(p => p.CI == ci);
                if (existingPatient == null)
                {
                    return NotFound("Patient not found");
                }

                existingPatient.Name = patient.Name;
                existingPatient.LastName = patient.LastName;

                WritePatientsToFile(patients);

                return Ok(existingPatient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{ci}")]
        public IActionResult DeletePatient(string ci)
        {
            try
            {
                var patients = ReadPatientsFromFile();
                var existingPatient = patients.Find(p => p.CI == ci);
                if (existingPatient == null)
                {
                    return NotFound("Patient not found");
                }

                patients.Remove(existingPatient);
                WritePatientsToFile(patients);

                return Ok(existingPatient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private List<Patient> ReadPatientsFromFile()
        {
            if (!System.IO.File.Exists(FilePath))
            {
                return new List<Patient>();
            }

            var json = System.IO.File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<List<Patient>>(json);
        }

        private void WritePatientsToFile(List<Patient> patients)
        {
            var json = JsonConvert.SerializeObject(patients);
            System.IO.File.WriteAllText(FilePath, json);
        }
    }
}
