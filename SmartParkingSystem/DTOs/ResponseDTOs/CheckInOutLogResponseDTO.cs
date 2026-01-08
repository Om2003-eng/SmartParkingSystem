namespace SmartParkingSystem.DTOs.Responses
{
    public class CheckInOutLogResponseDTO
    {
        public int ReservationId { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public int DurationSeconds { get; set; }
        public decimal TotalFee { get; set; }
        public string PaymentStatus { get; set; }
        public string FullName { get; set; }

    }
}
