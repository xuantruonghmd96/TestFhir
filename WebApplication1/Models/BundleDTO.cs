
using Hl7.Fhir.Model;
using System.Linq.Expressions;

namespace WebApplication1.Models
{
    public class BundleDTO<T>
    {
        public int Total { get; set; }
        public int Count { get; set; }
        public List<T> Items { get; set; }
    }

    public class EntryBaseDTO
    {
        public string FullUrl { get; set; }
        public string Id { get; set; }

        public static string? GetReferenceId(ResourceReference? resourceReference)
        {
            if (resourceReference == null)
                return null;
            return resourceReference.Reference.Contains('/') ? resourceReference.Reference.Split('/').Last() : resourceReference.Reference;
        }

        public static string? GetReferenceDisplay(ResourceReference? resourceReference)
        {
            return resourceReference?.Display;
        }

        public static string? GetReferenceReference(ResourceReference? resourceReference)
        {
            return resourceReference?.Reference;
        }

        internal static string? GetIdentifierValue(Identifier? identifier)
        {
            return identifier?.Value;
        }

        public static ResourceReference CreateResouceReference(string type, string elementId, string? display = null)
        {
            return new ResourceReference
            {
                Type = type,
                ElementId = elementId,
                Display = display,
                Reference = $"{type}/{elementId}",
            };
        }

        public static ResourceReference? GetParticipantActor(List<Appointment.ParticipantComponent> participants, string actorType)
        {
            return participants.FirstOrDefault(x => x.Actor?.Type == actorType)?.Actor;
        }
    }
}
