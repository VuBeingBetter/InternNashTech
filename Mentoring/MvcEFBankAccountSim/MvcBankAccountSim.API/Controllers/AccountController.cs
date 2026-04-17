using Microsoft.AspNetCore.Mvc;
using MvcBankAccountSim.Application.DTOs;
using MvcBankAccountSim.Application.Interfaces;
using MvcBankAccountSim.Domain.Entities;

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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAccountRequest request)
    {
        // 1. Manually check if the basic inputs are valid
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        try
        {
            await _accountService.CreateAccountAsync(request.OwnerName, request.Balance);
            return RedirectToAction(nameof(Index));
        }
        
        catch (Exception ex)
        {
            // Scenario: Duplicate account number
            ModelState.AddModelError("AccountNumber", ex.Message);
            return View(request);
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