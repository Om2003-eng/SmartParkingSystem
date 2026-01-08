using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartParking.Core.Entities;
using SmartParking.Core.Interfaces;
using SmartParkingSystem.DTOs;
using SmartParkingSystem.DTOs.Requests;
using SmartParkingSystem.DTOs.Responses;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly IGenericRepository<Reservation> _repo;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Notification> _notificationRepo;
    private readonly IGenericRepository<CheckInOutLog> _checkInOutLogRepo;


    public ReservationsController(IGenericRepository<Reservation> repo, IMapper mapper, IGenericRepository<Notification> notificationRepo, IGenericRepository<CheckInOutLog> checkInOutLogRepo)
    {
        _repo = repo;
        _mapper = mapper;
        _notificationRepo = notificationRepo;
        _checkInOutLogRepo = checkInOutLogRepo;

    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservationResponseDTO>>> GetAll()
    {
        var reservations = await _repo.GetAllWithIncludesAsync(
            r => r.User,
            r => r.Slot
        );

        return Ok(_mapper.Map<IEnumerable<ReservationResponseDTO>>(reservations));
    }

    [HttpPost]

    [Authorize]
    public async Task<IActionResult> Create(CreateReservationRequestDTO dto)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // 1️⃣ Check slot availability
        var slotTaken = (await _repo.GetAllAsync())
            .Any(r => r.SlotId == dto.SlotId && r.Status == "active");

        if (slotTaken)
            return BadRequest(new { message = "Slot already reserved." });

        // 2️⃣ Create reservation
        var reservation = _mapper.Map<Reservation>(dto);
        reservation.UserId = userId;
        reservation.StartTime = DateTime.UtcNow;
        reservation.Status = "active";
        reservation.QRCode = Guid.NewGuid().ToString();

        await _repo.AddAsync(reservation);

        return Ok(new
        {
            reservation_id = reservation.ReservationId,
            status = reservation.Status,
            start_time = reservation.StartTime
        });
    }

    // 🔹 Get active reservation for logged-in user
    [HttpGet("active")]

    [Authorize]
    public async Task<IActionResult> GetActiveReservation()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var reservation = (await _repo.GetAllAsync())
            .FirstOrDefault(r => r.UserId == userId && r.Status == "active");

        if (reservation == null)
            return NotFound(new { message = "No active reservation found." });

        return Ok(new
        {
            reservation_id = reservation.ReservationId,
            slot_id = reservation.SlotId,
            status = reservation.Status,
            start_time = reservation.StartTime
        });
    }

    [HttpPost("end")]

    [Authorize]
    public async Task<IActionResult> EndParking()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var reservation = (await _repo.GetAllAsync())
            .FirstOrDefault(r => r.UserId == userId && r.Status == "active");

        if (reservation == null)
            return NotFound(new { message = "No active reservation found." });

        reservation.Status = "completed";
        reservation.EndTime = DateTime.UtcNow;

        await _repo.UpdateAsync(reservation);

        await _checkInOutLogRepo.AddAsync(new CheckInOutLog
        {
            ReservationId = reservation.ReservationId,
            CheckInTime = reservation.StartTime,
            CheckOutTime = reservation.EndTime,
            DurationSeconds = (int)(reservation.EndTime - reservation.StartTime).TotalSeconds,
            TotalFee = CalculateFee(reservation), // your fee logic
            PaymentStatus = "Paid" // or whatever applies
        });

        await _notificationRepo.AddAsync(new Notification
        {
            UserId = userId,
            Message = "Your parking session has ended successfully.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });

        return Ok(new
        {
            message = "Parking ended successfully",
            reservation_id = reservation.ReservationId,
            end_time = reservation.EndTime
        });



    }
    private decimal CalculateFee(Reservation reservation)
    {
        var durationMinutes = (reservation.EndTime - reservation.StartTime).TotalMinutes;
        decimal ratePerHour = 10; // example rate
        decimal fee = (decimal)(durationMinutes / 60) * ratePerHour;
        return Math.Round(fee, 2);
    }

}




