using AutoMapper;
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
        private readonly Mapper _mapper;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Bundle.EntryComponent, AppointmentDTO>()
                    .ForMember(dest => dest.FullUrl, opt => opt.MapFrom(src => src.FullUrl))
                    .ConstructUsing((src, ctx) => ctx.Mapper.Map<AppointmentDTO>(src.Resource))
                    .IncludeAllDerived();
                cfg.CreateMap<Resource, AppointmentDTO>()
                    .ConstructUsing((src, ctx) => ctx.Mapper.Map<AppointmentDTO>((Appointment)src))
                    .IncludeAllDerived();
                cfg.CreateMap<Appointment, AppointmentDTO>()
                    .ForMember(dest => dest.CanCancel, opt => opt.MapFrom(src => src.GetBoolExtension(Models.Contstant.AppointmentExtensionCanCancel) ?? false))
                    .ForMember(dest => dest.CanReschedule, opt => opt.MapFrom(src => src.GetBoolExtension(Models.Contstant.AppointmentExtensionCanReschedule) ?? false))
                    .ForMember(dest => dest.IsExceedRescheduleLimit, opt => opt.MapFrom(src => src.GetBoolExtension(Models.Contstant.AppointmentExtensionExceedRescheduleLimit) ?? false))
                    .ForMember(dest => dest.IsCheckedIn, opt => opt.MapFrom(src => src.GetBoolExtension(Models.Contstant.AppointmentExtensionIsCheckedIn) ?? false))

                    .ForMember(dest => dest.ContactNo, opt => opt.MapFrom(src => src.GetStringExtension(Models.Contstant.AppointmentExtensionContactNo)))
                    .ForMember(dest => dest.AppointmentIdForRegistration, opt => opt.MapFrom(src => src.GetStringExtension(Models.Contstant.AppointmentExtensionRegistrationAppointmentId)))
                    .ForMember(dest => dest.VCPZoomParticipantUrl, opt => opt.MapFrom(src => src.GetStringExtension(Models.Contstant.AppointmentExtensionParticipantURL)))
                    .ForMember(dest => dest.DepartmentCode, opt => opt.MapFrom(src => src.GetStringExtension(Models.Contstant.AppointmentExtensionDepartmentCode)))
                    .ForMember(dest => dest.EncounterId, opt => opt.MapFrom(src => EntryBaseDTO.GetReferenceId(src.GetExtensionValue<ResourceReference>(Models.Contstant.AppointmentExtensionEncounterId))))
                    .ForMember(dest => dest.OrganizationUnit, opt => opt.MapFrom(src => src.GetStringExtension(Models.Contstant.AppointmentExtensionOrganizationUnit)))
                    .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom(src => EntryBaseDTO.GetReferenceId(src.SupportingInformation.FirstOrDefault(x => x.Type == Models.Contstant.Organization))))
                    .ForMember(dest => dest.OrganizationDisplay, opt => opt.MapFrom(src => EntryBaseDTO.GetReferenceDisplay(src.SupportingInformation.FirstOrDefault(x => x.Type == Models.Contstant.Organization))))
                    .ForMember(dest => dest.Location, opt => opt.MapFrom(src => EntryBaseDTO.GetReferenceDisplay(EntryBaseDTO.GetParticipantActor(src.Participant, Models.Contstant.Location)) + " " + EntryBaseDTO.GetReferenceDisplay(src.SupportingInformation.FirstOrDefault(x => x.Type == Models.Contstant.Organization)) + " " + EntryBaseDTO.GetReferenceId(EntryBaseDTO.GetParticipantActor(src.Participant, Models.Contstant.Location))))
                    .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => EntryBaseDTO.GetReferenceId(EntryBaseDTO.GetParticipantActor(src.Participant, Models.Contstant.Location))))
                    .ForMember(dest => dest.RegistrationAvailability, opt => opt.MapFrom(src => src.GetStringExtension(Models.Contstant.AppointmentExtensionRegistrationAvailability)))
                    .ForMember(dest => dest.PossibleRescheduleStartDate, opt => opt.MapFrom(src => src.GetExtensionValue<FhirDateTime>(Models.Contstant.AppointmentExtensionPossibleRescheduleStartDate).ToDateTimeOffset(new TimeSpan())))
                    .ForMember(dest => dest.PossibleRescheduleEndDate, opt => opt.MapFrom(src => src.GetExtensionValue<FhirDateTime>(Models.Contstant.AppointmentExtensionPossibleRescheduleEndDate).ToDateTimeOffset(new TimeSpan())))
                    .ForMember(dest => dest.Slot, opt => opt.MapFrom(src => EntryBaseDTO.GetReferenceReference(src.Slot.FirstOrDefault())))
                    .ForMember(dest => dest.VisitType, opt => opt.MapFrom(src => src.GetStringExtension(Models.Contstant.AppointmentExtensionVisitType)))
                    .ForMember(dest => dest.VisitTypeId, opt => opt.MapFrom(src => src.GetStringExtension(Models.Contstant.AppointmentExtensionVisitTypeId)))
                    .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => EntryBaseDTO.GetReferenceId(EntryBaseDTO.GetParticipantActor(src.Participant, Models.Contstant.Patient))))
                    .ForMember(dest => dest.ProviderPractitionerId, opt => opt.MapFrom(src => EntryBaseDTO.GetReferenceId(EntryBaseDTO.GetParticipantActor(src.Participant, Models.Contstant.Practitioner))))
                    .ForMember(dest => dest.ProviderPractitioner, opt => opt.MapFrom(src => EntryBaseDTO.GetReferenceDisplay(EntryBaseDTO.GetParticipantActor(src.Participant, Models.Contstant.Practitioner))))
                    .ForMember(dest => dest.LocationZoneInfo, opt => opt.MapFrom(src => src.GetStringExtension(Models.Contstant.AppointmentExtensionZoneInfo)))
                    .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType.Select(x => x.Text)))
                    .ForMember(dest => dest.AppointmentLinkId, opt => opt.MapFrom(src => EntryBaseDTO.GetIdentifierValue(src.GetIdentifier(Models.Contstant.AppointmentLinkIdIdentifierSystemType))))
                    .ForMember(dest => dest.RescheduleToDays, opt => opt.MapFrom(src => src.GetStringExtension(Models.Contstant.AppointmentExtensionRescheduleToDays)))
                    .ForMember(dest => dest.OrderExpectedDate, opt => opt.MapFrom(src => src.GetExtensionValue<FhirDateTime>(Models.Contstant.AppointmentExtensionOrderExpectedDate).ToDateTimeOffset(new TimeSpan())))

                    .IncludeAllDerived();

                cfg.CreateMap<AppointmentDTO, Appointment>()
                    .AfterMap((x, y) =>
                    {
                        y.Status = x.Status;
                        y.Start = x.Start;
                        y.End = x.End;
                        y.Description = x.Description;
                        y.PatientInstruction = x.PatientInstruction;
                        y.Comment = x.Comment;
                        y.Extension = new();
                        y.SupportingInformation = new();
                        y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionCanCancel, Value = new FhirBoolean(x.CanCancel) });
                        y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionCanReschedule, Value = new FhirBoolean(x.CanReschedule) });
                        y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionExceedRescheduleLimit, Value = new FhirBoolean(x.IsExceedRescheduleLimit) });
                        y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionIsCheckedIn, Value = new FhirBoolean(x.IsCheckedIn) });
                        if (x.ContactNo != null)
                            y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionContactNo, Value = new FhirString(x.ContactNo) });
                        if (x.AppointmentIdForRegistration != null)
                            y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionRegistrationAppointmentId, Value = new FhirString(x.AppointmentIdForRegistration) });
                        if (x.VCPZoomParticipantUrl != null)
                            y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionParticipantURL, Value = new FhirString(x.VCPZoomParticipantUrl) });
                        if (x.DepartmentCode != null)
                            y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionDepartmentCode, Value = new FhirString(x.DepartmentCode) });
                        if (!string.IsNullOrEmpty(x.EncounterId))
                            y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionEncounterId, Value = EntryBaseDTO.CreateResouceReference("Encounter", x.EncounterId) });
                        if (x.OrganizationUnit != null)
                            y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionOrganizationUnit, Value = new FhirString(x.OrganizationUnit) });
                        if (!string.IsNullOrEmpty(x.OrganizationId))
                            y.SupportingInformation.Add(EntryBaseDTO.CreateResouceReference(Models.Contstant.Organization, x.OrganizationId, x.OrganizationDisplay));
                        if (!string.IsNullOrEmpty(x.LocationId))
                            y.Participant.Add(new Appointment.ParticipantComponent
                            {
                                Actor = EntryBaseDTO.CreateResouceReference(Models.Contstant.Location, x.LocationId, x.Location),
                                Status = ParticipationStatus.Accepted,
                            });
                        if (x.RegistrationAvailability != null)
                            y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionRegistrationAvailability, Value = new FhirString(x.RegistrationAvailability) });
                        if (x.PossibleRescheduleStartDate.HasValue)
                            y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionPossibleRescheduleStartDate, Value = new FhirDateTime(x.PossibleRescheduleStartDate.Value) });
                        if (x.PossibleRescheduleEndDate.HasValue)
                            y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionPossibleRescheduleEndDate, Value = new FhirDateTime(x.PossibleRescheduleEndDate.Value) });
                        if (!string.IsNullOrEmpty(x.Slot))
                            y.Slot.Add(EntryBaseDTO.CreateResouceReference("Slot", x.Slot));
                        if (x.VisitType != null)
                            y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionVisitType, Value = new FhirString(x.VisitType) });
                        if (x.VisitTypeId != null)
                            y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionVisitTypeId, Value = new FhirString(x.VisitTypeId) });
                        if (!string.IsNullOrEmpty(x.PatientId))
                            y.Participant.Add(new Appointment.ParticipantComponent
                            {
                                Actor = EntryBaseDTO.CreateResouceReference(Models.Contstant.Patient, x.PatientId),
                                Status = ParticipationStatus.Accepted,
                            });
                        if (!string.IsNullOrEmpty(x.ProviderPractitionerId))
                            y.Participant.Add(new Appointment.ParticipantComponent
                            {
                                Actor = EntryBaseDTO.CreateResouceReference(Models.Contstant.Practitioner, x.ProviderPractitionerId, x.ProviderPractitioner),
                                Status = ParticipationStatus.Accepted,
                            });
                        if (x.LocationZoneInfo != null)
                            y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionZoneInfo, Value = new FhirString(x.LocationZoneInfo) });
                        y.ServiceType = x.ServiceType.Select(x => new CodeableConcept("", "", x)).ToList();
                        if (x.AppointmentLinkId != null)
                            y.Identifier.Add(new Identifier(Models.Contstant.AppointmentLinkIdIdentifierSystemType, x.AppointmentLinkId));
                        if (x.RescheduleToDays != null)
                            y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionRescheduleToDays, Value = new FhirString(x.RescheduleToDays) });
                        if (x.OrderExpectedDate.HasValue)
                            y.Extension.Add(new Extension { Url = Models.Contstant.AppointmentExtensionOrderExpectedDate, Value = new FhirDateTime(x.OrderExpectedDate.Value) });
                    })
                    .IncludeAllDerived()
                    .ForAllMembers(opt => opt.Ignore());
            });
            _mapper = new Mapper(config);

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

            var bundle = await fhirClient.SearchAsync<Appointment>(pageSize: 20);
            var result = bundle.Entry.Select(x => _mapper.Map<AppointmentDTO>(x)).ToList();

            return new BundleDTO<AppointmentDTO>
            {
                Total = bundle.Total!.Value,
                Count = bundle.Entry.Count,
                Items = result,
            };
        }

        [HttpGet("Appointment/{id:string}", Name = "GetAppointmentById")]
        public async Task<AppointmentDTO?> GetAppointmentById([FromRoute] string id)
        {
            FhirClient fhirClient = new FhirClient(_fhirServer);
            var model = await fhirClient.GetAsync($"Appointment/{id}");
            return _mapper.Map<AppointmentDTO>(model);
        }

        [HttpPost("Appointment", Name = "CreateAppointment")]
        public async Task<AppointmentDTO> CreateAppointment([FromBody] AppointmentDTO request)
        {
            FhirClient fhirClient = new FhirClient(_fhirServer);

            Appointment model = _mapper.Map<Appointment>(request);
            model = await fhirClient.CreateAsync(model);

            return _mapper.Map<AppointmentDTO>(model);
        }

        [HttpDelete("Appointment/{id:string}", Name = "DeleteAppointment")]
        public async Task<AppointmentDTO?> DeleteAppointment([FromRoute] string id)
        {
            FhirClient fhirClient = new FhirClient(_fhirServer);

            var model = await fhirClient.GetAsync($"Appointment/{id}");
            if (model != null)
            {
                await fhirClient.DeleteAsync(model);
                return _mapper.Map<AppointmentDTO>(model);
            }

            return null;
        }
    }
}
