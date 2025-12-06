using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartParking.Core.Entities;
using SmartParking.Core.Interfaces;
using SmartParkingSystem.DTOs.Requests;
using SmartParkingSystem.DTOs.Responses;

[ApiController]
[Route("api/[controller]")]
public class WalletsController : ControllerBase
{
    private readonly IGenericRepository<Wallet> _repo;
    private readonly IMapper _mapper;

    public WalletsController(IGenericRepository<Wallet> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WalletResponseDTO>>> GetAll()
    {
        var wallets = await _repo.GetAllWithIncludesAsync(w => w.Payments);
        return Ok(_mapper.Map<IEnumerable<WalletResponseDTO>>(wallets));
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateWalletRequestDTO dto)
    {
        var wallet = _mapper.Map<Wallet>(dto);
        await _repo.AddAsync(wallet);
        return Ok("Wallet created");
    }
}
