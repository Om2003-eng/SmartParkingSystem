using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParking.Core.Entities
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public int SlotId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string QRCode { get; set; }
        public string Status { get; set; }

        // Navigation
        public User User { get; set; }
        public ParkingSlot Slot { get; set; }
        public CheckInOutLog CheckInOutLog { get; set; }
    }

}
