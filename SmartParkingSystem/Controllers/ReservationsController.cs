using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartParking.Core.Entities;
using SmartParking.Core.Interfaces;
using SmartParkingSystem.DTOs.Requests;
using SmartParkingSystem.DTOs.Responses;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly IGenericRepository<Reservation> _repo;
    private readonly IMapper _mapper;

    public ReservationsController(IGenericRepository<Reservation> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
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
    public async Task<ActionResult> Create(CreateReservationRequestDTO dto)
    {
        var reservation = _mapper.Map<Reservation>(dto);
        await _repo.AddAsync(reservation);
        return Ok("Reservation made");
    }
}
