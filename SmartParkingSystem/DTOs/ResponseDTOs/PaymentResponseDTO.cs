namespace SmartParkingSystem.DTOs.Responses
{
    public class PaymentResponseDTO
    {
        public int PaymentId { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
