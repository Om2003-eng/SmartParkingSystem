using System;
using System.Collections.Generic;
using System.Linq;
using SmartParking.Core.Entities;

namespace SmartParkingSystem
{
    public static class DbInitializer
    {
        public static void Initialize(ParkingContext context)
        {
            context.Database.EnsureCreated();

            // ------------------------------------------
            // Check if users exist
            // ------------------------------------------
            if (context.Users.Any())
                return; // DB has been seeded

            // ------------------------------------------
            // Users
            // ------------------------------------------
            var users = new List<User>
            {
                new User { FullName = "Mohamed Ali", Email = "mohamed.ali@gmail.com", Password= "hashedpassword", PhoneNumber = "01234567890", Role = "User", CreatedAt = DateTime.Now },
                new User { FullName = "Ahmed Hassan", Email = "ahmed.hassan@gmail.com", Password = "hashedpassword", PhoneNumber = "01234567891", Role = "User", CreatedAt = DateTime.Now },
                new User { FullName = "Sara Mohamed", Email = "sara.mohamed@gmail.com", Password = "hashedpassword", PhoneNumber = "01234567892", Role = "User", CreatedAt = DateTime.Now },
                new User { FullName = "Fatma Youssef", Email = "fatma.youssef@gmail.com", Password = "hashedpassword", PhoneNumber = "01234567893", Role = "User", CreatedAt = DateTime.Now },
                new User { FullName = "Khaled Samir", Email = "khaled.samir@gmail.com", Password = "hashedpassword", PhoneNumber = "01234567894", Role = "User", CreatedAt = DateTime.Now },
                new User { FullName = "Mona Adel", Email = "mona.adel@gmail.com", Password = "hashedpassword", PhoneNumber = "01234567895", Role = "User", CreatedAt = DateTime.Now },
                new User { FullName = "Omar Farouk", Email = "omar.farouk@gmail.com", Password = "hashedpassword", PhoneNumber = "01234567896", Role = "User", CreatedAt = DateTime.Now },
                new User { FullName = "Hany Mostafa", Email = "hany.mostafa@gmail.com", Password = "hashedpassword", PhoneNumber = "01234567897", Role = "User", CreatedAt = DateTime.Now },
                new User { FullName = "Nour El-Sayed", Email = "nour.elsayed@gmail.com", Password = "hashedpassword", PhoneNumber = "01234567898", Role = "User", CreatedAt = DateTime.Now },
                new User { FullName = "Yasmine Adel", Email = "yasmine.adel@gmail.com", Password = "hashedpassword", PhoneNumber = "01234567899", Role = "User", CreatedAt = DateTime.Now },

                // Admins
                new User { FullName = "Admin One", Email = "admin1@smartparking.com", Password = "hashedpassword", PhoneNumber = "01000000001", Role = "Admin", CreatedAt = DateTime.Now },
                new User { FullName = "Admin Two", Email = "admin2@smartparking.com", Password = "hashedpassword", PhoneNumber = "01000000002", Role = "Admin", CreatedAt = DateTime.Now },

                // More Users
            new User { FullName = "Mohsen Hossam", Email = "mohsen.hossam@gmail.com", Password = "hashedpassword", PhoneNumber = "01234567901", Role = "User", CreatedAt = DateTime.Now },
            new User { FullName = "Doaa Gamal", Email = "doaa.gamal@gmail.com", Password = "hashedpassword", PhoneNumber = "01234567902", Role = "User", CreatedAt = DateTime.Now },
            new User { FullName = "Tamer Salah", Email = "tamer.salah@gmail.com", Password = "hashedpassword", PhoneNumber = "01234567903", Role = "User", CreatedAt = DateTime.Now },
            new User { FullName = "Rana Fathy", Email = "rana.fathy@gmail.com", Password = "hashedpassword", PhoneNumber = "01234567904", Role = "User", CreatedAt = DateTime.Now },
                new User { FullName = "Mahmoud Nabil", Email = "mahmoud.nabil@gmail.com", Password = "hashedpassword", PhoneNumber = "01234567905", Role = "User", CreatedAt = DateTime.Now },
                new User { FullName = "Eman Khalil", Email = "eman.khalil@gmail.com", Password = "hashedpassword", PhoneNumber = "01234567906", Role = "User", CreatedAt = DateTime.Now },
                new User { FullName = "Sherif Mostafa", Email = "sherif.mostafa@gmail.com", Password = "hashedpassword", PhoneNumber = "01234567907", Role = "User", CreatedAt = DateTime.Now },
                new User { FullName = "Salma Magdy", Email = "salma.magdy@gmail.com", Password = "hashedpassword", PhoneNumber = "01234567908", Role = "User", CreatedAt = DateTime.Now }
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            // ------------------------------------------
            // Wallets
            // ------------------------------------------
            var wallets = users.Select(u => new Wallet
            {
                UserId = u.Id,
                Balance = 1000m,
                LastUpdated = DateTime.Now
            }).ToList();
            context.Wallets.AddRange(wallets);
            context.SaveChanges();

            // ------------------------------------------
            // Parking Slots (real Alexandria addresses)
            // ------------------------------------------
            var parkingAddresses = new List<string>
            {
                "Corniche El Maadi Street, Alexandria",
                "Fleming Area, Alexandria",
                "Raml Station Street, Alexandria",
                "Sidi Gaber District, Alexandria",
                "El Hadra Street, Alexandria",
                "Stanley Bridge Area, Alexandria",
                "Gleem District, Alexandria",
                "El Mandara Street, Alexandria",
                "Moharam Bek Street, Alexandria",
                "San Stefano Area, Alexandria",
                "Miami Beach Street, Alexandria",
                "Kafr Abdu District, Alexandria",
                "Sporting District, Alexandria",
                "El Shatby Street, Alexandria",
                "Alexandria University Area, Alexandria",
                "Smouha District, Alexandria",
                "El Ibrahimiya Street, Alexandria",
                "Attarin Area, Alexandria",
                "El Montaza Street, Alexandria",
                "El Agamy District, Alexandria"
            };

            var slots = new List<ParkingSlot>();
            for (int i = 0; i < parkingAddresses.Count; i++)
            {
                slots.Add(new ParkingSlot
                {
                    Area = parkingAddresses[i],
                    SlotNumber = i + 1,
                    Status = "Available"
                });
            }
            context.ParkingSlots.AddRange(slots);
            context.SaveChanges();

            // ------------------------------------------
            // Sensors
            // ------------------------------------------
            var sensors = slots.Select(s => new Sensor
            {
                SlotId = s.SlotId,
                Status = "Free",
                LastUpdateTime = DateTime.Now
            }).ToList();
            context.Sensors.AddRange(sensors);
            context.SaveChanges();

            // ------------------------------------------
            // Reservations
            // ------------------------------------------
            var random = new Random();
            var reservations = new List<Reservation>();
            for (int i = 1; i <= 10; i++)
            {
                var user = users[random.Next(users.Count)];
                var slot = slots[random.Next(slots.Count)];

                var start = DateTime.Now.AddHours(-random.Next(1, 10));
                var end = start.AddHours(random.Next(1, 5));

                reservations.Add(new Reservation
                {
                    UserId = user.Id,
                    SlotId = slot.SlotId,
                    StartTime = start,
                    EndTime = end,
                    QRCode = $"QR{i:D4}",
                    Status = "Completed"
                });
            }
            context.Reservations.AddRange(reservations);
            context.SaveChanges();

            // ------------------------------------------
            // CheckInOut Logs
            // ------------------------------------------
            var checkInOutLogs = reservations.Select(r => new CheckInOutLog
            {
                ReservationId = r.ReservationId,
                CheckInTime = r.StartTime,
                CheckOutTime = r.EndTime,
                DurationSeconds = (int)(r.EndTime - r.StartTime).TotalSeconds,
                TotalFee = (decimal)(r.EndTime - r.StartTime).TotalHours * 5m,
                PaymentStatus = "Paid"
            }).ToList();
            context.CheckInOutLogs.AddRange(checkInOutLogs);
            context.SaveChanges();

            // ------------------------------------------
            // Payments
            // ------------------------------------------
            var payments = checkInOutLogs.Select(c => new Payment
            {
                WalletId = wallets.First(w => w.UserId == reservations.First(r => r.ReservationId == c.ReservationId).UserId).WalletId,
                Amount = c.TotalFee,
                PaymentMethod = "Cash",
                Status = c.PaymentStatus,
                CreatedAt = DateTime.Now
            }).ToList();
            context.Payments.AddRange(payments);
            context.SaveChanges();
        }
    }
}
