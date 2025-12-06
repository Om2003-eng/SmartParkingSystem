namespace SmartParkingSystem.DTOs.Requests
{
    public class CreateParkingSlotRequestDTO
    {
        public string Area { get; set; }
        public int SlotNumber { get; set; }
        public string Status { get; set; }
    }
}
