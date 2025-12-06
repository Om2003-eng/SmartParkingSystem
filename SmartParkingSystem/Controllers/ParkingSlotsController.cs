using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartParking.Core.Entities;
using SmartParking.Core.Interfaces;
using SmartParkingSystem.DTOs.Requests;
using SmartParkingSystem.DTOs.Responses;

[ApiController]
[Route("api/[controller]")]
public class ParkingSlotsController : ControllerBase
{
    private readonly IGenericRepository<ParkingSlot> _repo;
    private readonly IMapper _mapper;

    public ParkingSlotsController(IGenericRepository<ParkingSlot> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ParkingSlotResponseDTO>>> GetAll()
    {
        var slots = await _repo.GetAllWithIncludesAsync(s => s.Sensor);
        return Ok(_mapper.Map<IEnumerable<ParkingSlotResponseDTO>>(slots));
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateParkingSlotRequestDTO dto)
    {
        var slot = _mapper.Map<ParkingSlot>(dto);
        await _repo.AddAsync(slot);
        return Ok("Slot created");
    }
}
