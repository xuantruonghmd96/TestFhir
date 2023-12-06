using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private const string _fhirServer = "http://server.fire.ly";

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Patient", Name = "GetPatient")]
        public async Task<BundleDTO<PatientDTO>> Get()
        {
            FhirClient fhirClient = new FhirClient(_fhirServer)
            {
                //Settings =
                //{
                //    PreferredFormat = ResourceFormat.Json,
                //    PreferredReturn = Prefer.ReturnRepresentation,
                //    VerifyFhirVersion = true,
                //    ExplicitFhirVersion = "4.0.1",
                //}
            };

            List<PatientDTO> patients = new();

            var patientBundle = await fhirClient.SearchAsync<Patient>(new string[] { "name=P" }, pageSize: 20);
            foreach (var entry in patientBundle.Entry)
            {
                patients.Add(new PatientDTO
                {
                    Extension = ((Patient)entry.Resource).Extension,
                    FullUrl = entry.FullUrl,
                    Id = ((Patient)entry.Resource).Id,
                    Name = ((Patient)entry.Resource).Name.FirstOrDefault()?.ToString(),
                    BirthDate = ((Patient)entry.Resource).BirthDate,
                });
            }

            return new BundleDTO<PatientDTO>
            {
                Total = patientBundle.Total!.Value,
                Count = patientBundle.Entry.Count,
                Items = patients,
            };
        }

        [HttpGet("Appointment", Name = "GetAppointment")]
        public async Task<BundleDTO<AppointmentDTO>> GetAppointment()
        {
            FhirClient fhirClient = new FhirClient(_fhirServer);

            List<AppointmentDTO> result = new();

            var bundle = await fhirClient.SearchAsync<Appointment>();
            foreach (var entry in bundle.Entry)
            {
                result.Add(new AppointmentDTO
                {
                    FullUrl = entry.FullUrl,
                    Id = entry.Resource.Id,
                    Extension = ((Appointment)entry.Resource).Extension,

                    Status = ((Appointment)entry.Resource).Status,
                    Start = ((Appointment)entry.Resource).Start,
                    End = ((Appointment)entry.Resource).End,
                    TypeName = ((Appointment)entry.Resource).TypeName,
                });
            }

            return new BundleDTO<AppointmentDTO>
            {
                Total = bundle.Total!.Value,
                Count = bundle.Entry.Count,
                Items = result,
            };
        }

        [HttpPost("Appointment", Name = "CreateAppointment")]
        public async Task<AppointmentDTO> CreateAppointment([FromBody] AppointmentDTO request)
        {
            FhirClient fhirClient = new FhirClient(_fhirServer);

            Appointment model = new Appointment
            {
                Status = request.Status,
                Start = request.Start,
                End = request.End,
                // Must have Participant 1..*
                Participant = new List<Appointment.ParticipantComponent>
                {
                    new Appointment.ParticipantComponent
                    {
                        Actor = new ResourceReference
                        {
                            Type = "Patient",
                            Reference = "Patient",
                        },
                        Status = ParticipationStatus.Tentative,
                    }
                },
                Extension = request.Extension.Select(x =>
                    new Extension
                    {
                        Url = x.Url,
                        Value = x.Value,
                    }
                ).ToList(),
            };
            model = await fhirClient.CreateAsync(model);

            return new AppointmentDTO
            {
                Id = model.Id,
                Status = model.Status,
                Start = model.Start,
                End = model.End,
                TypeName = model.TypeName,
                Extension = ((Appointment)model).Extension,
            };
        }

        [HttpGet("Appointment/{id:string}", Name = "GetAppointmentById")]
        public async Task<Appointment?> GetAppointmentById([FromRoute] string id)
        {
            FhirClient fhirClient = new FhirClient(_fhirServer);

            var model = await fhirClient.GetAsync($"Appointment/{id}");

            return (Appointment?)model;
        }

        [HttpDelete("Appointment/{id:string}", Name = "DeleteAppointment")]
        public async Task<AppointmentDTO?> DeleteAppointment([FromRoute] string id)
        {
            FhirClient fhirClient = new FhirClient(_fhirServer);

            var model = await fhirClient.GetAsync($"Appointment/{id}");
            if (model != null)
            {
                await fhirClient.DeleteAsync(model);
                return new AppointmentDTO
                {
                    Id = model.Id,
                    Status = ((Appointment)model).Status,
                    Start = ((Appointment)model).Start,
                    End = ((Appointment)model).End,
                    TypeName = ((Appointment)model).TypeName,
                };
            }

            return null;
        }
    }
}
