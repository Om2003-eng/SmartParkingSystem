namespace SmartParkingSystem.DTOs.Responses
{
    public class ReservationResponseDTO
    {
        public int ReservationId { get; set; }
        public int Id { get; set; }
        public int SlotId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string QRCode { get; set; }
        public string Status { get; set; }
    }
}
