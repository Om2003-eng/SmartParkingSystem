using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParking.Core.Entities
{
    public class Sensor
    {
        public int SlotId { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdateTime { get; set; }

        public ParkingSlot ParkingSlot { get; set; }
    }

}
