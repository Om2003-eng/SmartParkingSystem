using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartParking.Core.Entities;
using SmartParking.Core.Interfaces;
using SmartParkingSystem.DTOs.Requests;
using SmartParkingSystem.DTOs.Responses;

[ApiController]
[Route("api/[controller]")]
public class AdminReportsController : ControllerBase
{
    private readonly IGenericRepository<AdminReport> _repo;
    private readonly IMapper _mapper;

    public AdminReportsController(IGenericRepository<AdminReport> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdminReportResponseDTO>>> GetAll()
    {
        var reports = await _repo.GetAllWithIncludesAsync(r => r.Admin);
        return Ok(_mapper.Map<IEnumerable<AdminReportResponseDTO>>(reports));
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateAdminReportRequestDTO dto)
    {
        var report = _mapper.Map<AdminReport>(dto);
        await _repo.AddAsync(report);
        return Ok("Report created");
    }
}
