namespace SmartParkingSystem.DTOs.Responses
{
    public class ChatbotLogResponseDTO
    {
        public int ChatId { get; set; }
        public int Id { get; set; }
        public string Message { get; set; }
        public string Response { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
