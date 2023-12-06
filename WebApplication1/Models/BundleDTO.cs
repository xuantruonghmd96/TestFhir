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
        public List<Hl7.Fhir.Model.Extension> Extension { get; set; } = new();
        public string FullUrl { get; set; }
        public string Id { get; set; }
    }
}
