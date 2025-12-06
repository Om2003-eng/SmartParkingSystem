namespace SmartParkingSystem.DTOs.Responses
{
    public class AdminReportResponseDTO
    {
        public int ReportId { get; set; }
        public int AdminId { get; set; }
        public string ReportType { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
