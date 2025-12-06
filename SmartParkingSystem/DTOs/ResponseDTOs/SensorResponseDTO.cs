namespace SmartParkingSystem.DTOs.Responses
{
    public class SensorResponseDTO
    {
        public int SlotId { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }
}
