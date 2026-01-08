using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartParking.Core.Entities;
using SmartParking.Core.Interfaces;
using SmartParkingSystem.DTOs.Requests;
using SmartParkingSystem.DTOs.Responses;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ParkingSlotsController : ControllerBase
{
    private readonly IGenericRepository<ParkingSlot> _repo;
    private readonly IGenericRepository<Reservation> _reservationRepo;
    private readonly IMapper _mapper;

    public ParkingSlotsController(
        IGenericRepository<ParkingSlot> repo,
        IGenericRepository<Reservation> reservationRepo,
        IMapper mapper)
    {
        _repo = repo;
        _reservationRepo = reservationRepo;
        _mapper = mapper;
    }

    // 🔹 Get all slots with availability status
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ParkingSlotResponseDTO>>> GetAll()
    {
        var slots = await _repo.GetAllWithIncludesAsync(s => s.Reservations);

        var slotDtos = slots.Select(s =>
        {
            var dto = _mapper.Map<ParkingSlotResponseDTO>(s);
            // A slot is available if it has no active reservations
            dto.Status = s.Reservations.Any(r => r.Status == "active") ? "Reserved" : "Available";
            return dto;
        });

        return Ok(slotDtos);
    }

    // 🔹 Create a new parking slot
    [HttpPost]
    public async Task<ActionResult> Create(CreateParkingSlotRequestDTO dto)
    {
        var slot = _mapper.Map<ParkingSlot>(dto);
        slot.Status = "Available"; // default status
        await _repo.AddAsync(slot);
        return Ok(new { message = "Slot created successfully" });
    }

    // 🔹 Get a single slot by id with availability
    [HttpGet("{id}")]
    public async Task<ActionResult<ParkingSlotResponseDTO>> GetById(int id)
    {
        var slot = await _repo.GetByIdWithIncludesAsync(id, s => s.Reservations);
        if (slot == null)
            return NotFound(new { message = "Slot not found" });

        var dto = _mapper.Map<ParkingSlotResponseDTO>(slot);
        dto.Status = slot.Reservations.Any(r => r.Status == "active") ? "Reserved" : "Available";
        return Ok(dto);
    }
}
