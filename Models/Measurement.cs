using System.Text.Json.Serialization;

namespace ApiTestBarrels.Models
{
    public class Measurement
    {
        public string? id { get; set; }
        public string? barrelId { get; set; }
        public int dirtLevel { get; set; }
        public int weight { get; set; }
    }
    public class MeasurementCreateRequest
    {
        public string barrelId { get; set; } = string.Empty;
        public int dirtLevel { get; set; }
        public int weight { get; set; }
    }
}
