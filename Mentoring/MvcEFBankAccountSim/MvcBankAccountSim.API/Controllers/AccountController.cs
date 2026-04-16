using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MvcBankAccountSim.Application.Interfaces;
using MvcBankAccountSim.Domain.Entities;
using MvcBankAccountSim.Domain.Enums;

namespace MvcBankAccountSim.API.Controllers;


public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<IActionResult> Index()
    {
        var accounts = await _accountService.GetAllAccountsAsync();
        return View(accounts);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(string ownerName, decimal balance)
    {
        // 1. Manually check if the basic inputs are valid
        if (string.IsNullOrWhiteSpace(ownerName))
        {
            ModelState.AddModelError("", "Owner Name is required.");
            return View();
        }
        if (balance < 100)
        {
            ModelState.AddModelError("", "Initial Balance cannot be less than 100.");
            return View();
        }

        try
        {
            await _accountService.CreateAccountAsync(ownerName, balance);
            return RedirectToAction(nameof(Index));
        }
        
        catch (Exception ex)
        {
            // Scenario: Duplicate account number
            ModelState.AddModelError("AccountNumber", ex.Message);
            return View();
        }
    }
    public async Task<IActionResult> Details(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber)) return NotFound();
        var account = await _accountService.GetAccountByNumberAsync(accountNumber);
        if (account == null) return NotFound();
        ViewBag.Account = account;
        return View(account);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(string accountNumber)
    {
        try
        {
            await _accountService.ToggleStatusAsync(accountNumber);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetOwnerName(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            return BadRequest();

        var account = await _accountService.GetAccountByNumberAsync(accountNumber);
        
        if (account == null)
        {
            return Json(new { found = false }); 
        }

        // Trả về JSON để JavaScript ở View có thể đọc được
        return Json(new { found = true, name = account.OwnerName });
    }
}