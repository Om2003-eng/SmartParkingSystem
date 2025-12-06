using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartParking.Core.Entities;
using SmartParking.Core.Interfaces;
using SmartParking.Repository;
using SmartParkingSystem.DTOs.Requests;
using SmartParkingSystem.DTOs.Responses;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IGenericRepository<User> _repo;
    private readonly IMapper _mapper;

    public UsersController(IGenericRepository<User> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetAll()
    {
        var users = await _repo.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<UserResponseDTO>>(users));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDTO>> GetById(int id)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(_mapper.Map<UserResponseDTO>(user));
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateUserRequestDTO dto)
    {
        var user = _mapper.Map<User>(dto);
        await _repo.AddAsync(user);
        return Ok("User created");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, CreateUserRequestDTO dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound();

        _mapper.Map(dto, existing);
        await _repo.UpdateAsync(existing);

        return Ok("Updated");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound();

        await _repo.DeleteAsync(existing);
        return Ok("Deleted");
    }
}
