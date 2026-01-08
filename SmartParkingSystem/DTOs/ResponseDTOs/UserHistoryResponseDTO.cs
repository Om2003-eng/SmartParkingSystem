namespace SmartParkingSystem.DTOs.ResponseDTOs
{
    public class UserHistoryResponseDTO
    {
        public int ReservationId { get; set; }
        public int SlotNumber { get; set; }
        public string Area { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public int DurationSeconds { get; set; }
        public decimal TotalFee { get; set; }
        public string PaymentStatus { get; set; }
    }

}
