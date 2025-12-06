namespace SmartParkingSystem.DTOs.Requests
{
    public class CreateReservationRequestDTO
    {
        public int Id { get; set; }
        public int SlotId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
