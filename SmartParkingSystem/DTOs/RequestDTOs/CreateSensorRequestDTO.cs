namespace SmartParkingSystem.DTOs.Requests
{
    public class CreateSensorRequestDTO
    {
        public int SlotId { get; set; }
        public bool IsOccupied { get; set; }
    }

}
