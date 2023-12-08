using Hl7.Fhir.Model;
using static Hl7.Fhir.Model.Appointment;

namespace WebApplication1.Models
{
    public class AppointmentDTO : EntryBaseDTO
    {
        public DateTimeOffset? Start { get; set; }
        public DateTimeOffset? End { get; set; }

        public bool CanCancel { get; set; }
        public bool CanReschedule { get; set; }
        public bool IsExceedRescheduleLimit { get; set; }
        public bool IsCheckedIn { get; set; }
        public string? ContactNo { get; set; }
        public string? AppointmentIdForRegistration { get; set; }
        public string? VCPZoomParticipantUrl { get; set; }
        public string? DepartmentCode { get; set; }
        public string? EncounterId { get; set; }
        public string? OrganizationUnit { get; set; }
        public string? OrganizationId { get; set; }
        public string? OrganizationDisplay { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? LocationId { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? RegistrationAvailability { get; set; }
        public DateTimeOffset? PossibleRescheduleStartDate { get; set; }
        public DateTimeOffset? PossibleRescheduleEndDate { get; set; }
        public string? PatientInstruction { get; set; }
        public string? Slot { get; set; }
        public string? VisitType { get; set; }
        public string? VisitTypeId { get; set; }
        public string? PatientId { get; set; }
        public string? ProviderPractitionerId { get; set; }
        public string? ProviderPractitioner { get; set; }
        public string? LocationZoneInfo { get; set; }
        public List<string> ServiceType { get; set; } = new List<string>();
        public string? AppointmentLinkId { get; set; }
        public string? RescheduleToDays { get; set; }
        public DateTimeOffset? OrderExpectedDate { get; set; }
        public string? Comment { get; set; }
    }
}
