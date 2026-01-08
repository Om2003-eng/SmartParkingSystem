using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartParking.Core.Entities;
using SmartParking.Core.Interfaces;
using SmartParkingSystem.DTOs.ResponseDTOs;
using System.Security.Claims;

[ApiController]
[Route("api/user/history")]
[Authorize]
public class UserHistoryController : ControllerBase
{
    private readonly IGenericRepository<CheckInOutLog> _logRepo;

    public UserHistoryController(IGenericRepository<CheckInOutLog> logRepo)
    {
        _logRepo = logRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserHistory()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var logs = await _logRepo.GetAllWithIncludesAsync(
            l => l.Reservation,
            l => l.Reservation.Slot
        );

        var history = logs
            .Where(l => l.Reservation.UserId == userId)
            .Select(l => new UserHistoryResponseDTO
            {
                ReservationId = l.ReservationId,
                SlotNumber = l.Reservation.Slot.SlotNumber,
                Area = l.Reservation.Slot.Area,
                CheckInTime = l.CheckInTime,
                CheckOutTime = l.CheckOutTime,
                DurationSeconds = l.DurationSeconds,
                TotalFee = l.TotalFee,
                PaymentStatus = l.PaymentStatus
            });

        return Ok(history);
    }
}
