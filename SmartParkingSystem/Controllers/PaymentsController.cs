using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartParking.Core.Entities;
using SmartParking.Core.Interfaces;
using SmartParkingSystem.DTOs.Requests;
using SmartParkingSystem.DTOs.Responses;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IGenericRepository<Payment> _repo;
    private readonly IMapper _mapper;

    public PaymentsController(IGenericRepository<Payment> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentResponseDTO>>> GetAll()
    {
        var payments = await _repo.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<PaymentResponseDTO>>(payments));
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreatePaymentRequestDTO dto)
    {
        var payment = _mapper.Map<Payment>(dto);
        await _repo.AddAsync(payment);
        return Ok("Payment added");
    }
}
