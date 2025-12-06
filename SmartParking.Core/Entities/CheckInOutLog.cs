using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParking.Core.Entities
{
    public class CheckInOutLog
    {
        public int ReservationId { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public int DurationSeconds { get; set; }
        public decimal TotalFee { get; set; }
        public string PaymentStatus { get; set; }

        // Navigation
        public Reservation Reservation { get; set; }
    }

}
