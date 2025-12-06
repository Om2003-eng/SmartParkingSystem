using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartParking.Core.Entities;
using SmartParking.Core.Interfaces;
using SmartParkingSystem.DTOs.Requests;
using SmartParkingSystem.DTOs.Responses;

[ApiController]
[Route("api/[controller]")]
public class ChatbotLogsController : ControllerBase
{
    private readonly IGenericRepository<ChatbotLog> _repo;
    private readonly IMapper _mapper;

    public ChatbotLogsController(IGenericRepository<ChatbotLog> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatbotLogResponseDTO>>> GetAll()
    {
        var logs = await _repo.GetAllWithIncludesAsync(c => c.User);
        return Ok(_mapper.Map<IEnumerable<ChatbotLogResponseDTO>>(logs));
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateChatbotLogRequestDTO dto)
    {
        var log = _mapper.Map<ChatbotLog>(dto);
        await _repo.AddAsync(log);
        return Ok("Log added");
    }
}
