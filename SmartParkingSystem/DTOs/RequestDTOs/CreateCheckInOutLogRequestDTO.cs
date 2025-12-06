namespace SmartParkingSystem.DTOs.Requests
{
    public class CreateCheckInOutLogRequestDTO
    {
        public int ReservationId { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public int DurationSeconds { get; set; }
        public decimal TotalFee { get; set; }
        public string PaymentStatus { get; set; }
    }
}
