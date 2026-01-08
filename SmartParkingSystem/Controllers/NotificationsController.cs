using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartParking.Core.Entities;
using SmartParking.Core.Interfaces;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IGenericRepository<Notification> _repo;

    public NotificationsController(IGenericRepository<Notification> repo)
    {
        _repo = repo;
    }

    // 🔹 Get all notifications for logged-in user
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var notifications = (await _repo.GetAllWithIncludesAsync(n => n.User))
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new
            {
                n.NotificationId,
                n.UserId,
                n.Message,
                n.IsRead,
                n.CreatedAt,
                FullName = n.User.FullName
            });

        return Ok(notifications);
    }


    // 🔹 Get unread notifications count
    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadNotifications()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var unread = (await _repo.GetAllWithIncludesAsync(n => n.User))
            .Where(n => n.UserId == userId && !n.IsRead);

        return Ok(new
        {
            count = unread.Count(),
            notifications = unread.Select(n => new
            {
                n.NotificationId,
                n.UserId,
                n.Message,
                n.IsRead,
                n.CreatedAt,
                FullName = n.User.FullName
            })
        });
    }

    // 🔹 Mark single notification as read
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var notification = (await _repo.GetAllAsync())
            .FirstOrDefault(n => n.NotificationId == id && n.UserId == userId);

        if (notification == null)
            return NotFound(new { message = "Notification not found" });

        notification.IsRead = true;
        await _repo.UpdateAsync(notification);

        return Ok(new { message = "Notification marked as read" });
    }

    // 🔹 Mark all notifications as read
    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var notifications = (await _repo.GetAllAsync())
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToList();

        foreach (var n in notifications)
        {
            n.IsRead = true;
            await _repo.UpdateAsync(n);
        }

        return Ok(new { message = "All notifications marked as read" });
    }

 
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(int id)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var notification = (await _repo.GetAllAsync())
            .FirstOrDefault(n => n.NotificationId == id && n.UserId == userId);

        if (notification == null)
            return NotFound(new { message = "Notification not found" });

        await _repo.DeleteAsync(notification);

        return Ok(new { message = "Notification deleted" });
    }
}
