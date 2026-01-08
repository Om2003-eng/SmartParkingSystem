using Microsoft.EntityFrameworkCore;
using SmartParking.Core.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;



namespace SmartParkingSystem
{
    public class ParkingContext : DbContext
    {
        public ParkingContext(DbContextOptions<ParkingContext> options) : base(options) { }

        // -------------------------
        // DbSets
        // -------------------------
        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ParkingSlot> ParkingSlots { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ChatbotLog> ChatbotLogs { get; set; }
        public DbSet<AdminReport> AdminReports { get; set; }
        public DbSet<CheckInOutLog> CheckInOutLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ------------------- 1. User → Wallet (1:1) -------------------
            builder.Entity<User>()
                .HasKey(u => u.Id);

            builder.Entity<Wallet>()
                .HasKey(w => w.WalletId);

            builder.Entity<User>()
                .HasOne(u => u.Wallet)
                .WithOne(w => w.User)
                .HasForeignKey<Wallet>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ------------------- 2. User → Reservations (1:N) -------------------
            builder.Entity<Reservation>()
                .HasKey(r => r.ReservationId);

            builder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------- 3. ParkingSlot → Reservations (1:N) -------------------
            builder.Entity<ParkingSlot>()
                .HasKey(p => p.SlotId);

            builder.Entity<Reservation>()
                .HasOne(r => r.Slot)
                .WithMany(s => s.Reservations)
                .HasForeignKey(r => r.SlotId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------- 4. Sensor (Weak-like) -------------------
            builder.Entity<Sensor>()
                .HasKey(s => s.SlotId);

            builder.Entity<Sensor>()
                .HasOne(s => s.ParkingSlot)
                .WithOne(p => p.Sensor)
                .HasForeignKey<Sensor>(s => s.SlotId)
                .OnDelete(DeleteBehavior.Cascade);

            // ------------------- 5. CheckInOutLog (True weak) -------------------
            builder.Entity<CheckInOutLog>()
                .HasKey(c => c.ReservationId);

            builder.Entity<CheckInOutLog>()
                .HasOne(c => c.Reservation)
                .WithOne(r => r.CheckInOutLog)
                .HasForeignKey<CheckInOutLog>(c => c.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            // ------------------- 6. Wallet → Payments (1:N) -------------------
            builder.Entity<Payment>()
                .HasKey(p => p.PaymentId);

            builder.Entity<Payment>()
                .HasOne(p => p.Wallet)
                .WithMany(w => w.Payments)
                .HasForeignKey(p => p.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            // ------------------- 7. User → ChatbotLogs (1:N) -------------------
            builder.Entity<ChatbotLog>()
                .HasKey(c => c.ChatId);

            builder.Entity<ChatbotLog>()
                .HasOne(c => c.User)
                .WithMany(u => u.ChatbotLogs)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ------------------- 8. User(Admin) → AdminReports (1:N) -------------------
            builder.Entity<AdminReport>()
                .HasKey(a => a.ReportId);

            builder.Entity<AdminReport>()
                .HasOne(a => a.Admin)
                .WithMany(u => u.AdminReports)
                .HasForeignKey(a => a.AdminId)
                .OnDelete(DeleteBehavior.Restrict);


            // ------------------- 9. Notfications → User (M:1) -------------------

            builder.Entity<Notification>()
                .HasKey(n => n.NotificationId);

            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }

    }


}
