namespace SmartParkingSystem.DTOs.Requests
{
    public class CreatePaymentRequestDTO
    {
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
    }
}
