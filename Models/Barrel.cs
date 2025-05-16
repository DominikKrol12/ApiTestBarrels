namespace ApiTestBarrels.Models
{
    public class Barrel
    {
        public string? id { get; set; }
        public string? qr { get; set; }
        public string? rfid { get; set; }
        public string? nfc { get; set; }
    }
    public class BarrelCreateRequest
    {
        public string qr { get; set; } = string.Empty;
        public string rfid { get; set; } = string.Empty;
        public string nfc { get; set; } = string.Empty;
    }
}