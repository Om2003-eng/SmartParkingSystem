using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParking.Core.Entities
{
    public class ParkingSlot
    {
        public int SlotId { get; set; }
        public string Area { get; set; }
        public int SlotNumber { get; set; }
        public string Status { get; set; }

        // Navigation
        public ICollection<Reservation> Reservations { get; set; }
        public Sensor Sensor { get; set; }
    }

}
