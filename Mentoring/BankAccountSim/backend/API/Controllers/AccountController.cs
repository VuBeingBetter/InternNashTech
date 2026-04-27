using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, string search = "")
    {
        var pageSize = 10;

        // Call service
        var (accounts, totalAccounts) = await _accountService.GetAccountsAsync(page, pageSize, search);

        return Ok(new
        {
            Data = accounts,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalAccounts / pageSize),
            Search = search
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
    {
        // 1. Manually check if the basic inputs are valid
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _accountService.CreateAccountAsync(request.OwnerName, request.Balance);
            return Ok(new { message = "Account created successfully" });
        }
        
        catch (Exception ex)
        {
            // Scenario: Duplicate account number
            return BadRequest(new { message = ex.Message});
        }
    }

    [HttpGet("{accountNumber}/details")]
    public async Task<IActionResult> Details(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber)) return NotFound();
        var account = await _accountService.GetAccountByNumberAsync(accountNumber);
        if (account == null) return NotFound();
        
        return Ok(account);
    }

    [HttpPost("{accountNumber}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(string accountNumber)
    {
        try
        {
            await _accountService.ToggleStatusAsync(accountNumber);
            return Ok(new { message = "State updated" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{accountNumber}/owner")]
    public async Task<IActionResult> GetOwnerName(string accountNumber)
    {
        var account = await _accountService.GetAccountByNumberAsync(accountNumber);
        if (account == null) return NotFound(new { found = false }); 

        return Ok(new { found = true, name = account.OwnerName });
    }
}