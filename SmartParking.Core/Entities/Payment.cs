using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParking.Core.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Wallet Wallet { get; set; }
    }

}
