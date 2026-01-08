using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SmartParking.Core.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Nullable to support Google users
        public string? Password{ get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Role { get; set; }

        public bool EmailConfirmed { get; set; }
        public string? EmailConfirmationToken { get; set; }

        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Wallet Wallet { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
        public ICollection<ChatbotLog> ChatbotLogs { get; set; }
        public ICollection<AdminReport> AdminReports { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }

}
