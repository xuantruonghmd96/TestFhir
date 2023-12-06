using Hl7.Fhir.Model;
using static Hl7.Fhir.Model.Appointment;

namespace WebApplication1.Models
{
    public class AppointmentDTO : EntryBaseDTO
    {
        public List<Appointment.ParticipantComponent> Participant { get; set; } = new();
        public bool CanCancel { get; set; }
        public AppointmentStatus? Status { get; set; }
        public DateTimeOffset? Start { get; set; }
        public DateTimeOffset? End { get; set; }
        public string TypeName { get; set; }
    }
}
