using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartParking.Core.Entities;
using SmartParking.Core.Interfaces;
using SmartParkingSystem.DTOs.Requests;
using SmartParkingSystem.DTOs.Responses;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    private readonly IGenericRepository<Sensor> _repo;
    private readonly IMapper _mapper;

    public SensorsController(IGenericRepository<Sensor> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SensorResponseDTO>>> GetAll()
    {
        var sensors = await _repo.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<SensorResponseDTO>>(sensors));
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateSensorRequestDTO dto)
    {
        var sensor = _mapper.Map<Sensor>(dto);
        await _repo.AddAsync(sensor);
        return Ok("Sensor created");
    }
}
