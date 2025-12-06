using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartParking.Core.Entities;
using SmartParking.Core.Interfaces;
using SmartParkingSystem.DTOs.Requests;
using SmartParkingSystem.DTOs.Responses;

[ApiController]
[Route("api/[controller]")]
public class CheckInOutLogsController : ControllerBase
{
    private readonly IGenericRepository<CheckInOutLog> _repo;
    private readonly IMapper _mapper;

    public CheckInOutLogsController(IGenericRepository<CheckInOutLog> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CheckInOutLogResponseDTO>>> GetAll()
    {
        var logs = await _repo.GetAllWithIncludesAsync(
            c => c.Reservation
        );

        return Ok(_mapper.Map<IEnumerable<CheckInOutLogResponseDTO>>(logs));
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateCheckInOutLogRequestDTO dto)
    {
        var log = _mapper.Map<CheckInOutLog>(dto);
        await _repo.AddAsync(log);
        return Ok("Check-in/out saved");
    }
}
