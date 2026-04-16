using Microsoft.AspNetCore.Mvc;
using MvcBankAccountSim.Application.Interfaces;
using MvcBankAccountSim.Domain.Entities;
using MvcBankAccountSim.Domain.Enums;

namespace MvcBankAccountSim.API.Controllers;


public class TransactionController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly IAccountService _accountService;

    public TransactionController(ITransactionService transactionService, IAccountService accountService)
    {
        _transactionService = transactionService;
        _accountService = accountService;
    }

    public async Task<IActionResult> History(string accountNumber, string filter = "all")
    {
        // WHAT IF FILTER == "All"?
        if (string.IsNullOrWhiteSpace(accountNumber)) return BadRequest();
        
        var account = await _accountService.GetAccountByNumberAsync(accountNumber);
        if (account == null) return NotFound();

        var transactions = await _transactionService.GetHistoryAsync(accountNumber, filter);

        ViewBag.Account = account;
        ViewBag.Filter = filter;
        return View(transactions);
    }

    public async Task<IActionResult> Deposit(string accountNumber)
    {
        var account = await _accountService.GetAccountByNumberAsync(accountNumber);
        if (account == null) return NotFound();

        ViewBag.Account = account;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Deposit(string accountNumber, decimal amount)
    {
        try
        {
            await _accountService.DepositAsync(accountNumber, amount);
            return RedirectToAction("Details", "Account", new {accountNumber = accountNumber});
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            var account = await _accountService.GetAccountByNumberAsync(accountNumber);
            ViewBag.Account = account;
            return View();
        }
    }

    public async Task<IActionResult> Withdraw(string accountNumber)
    {
        var account = await _accountService.GetAccountByNumberAsync(accountNumber);
        if (account == null) return NotFound();

        ViewBag.Account = account;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Withdraw(string accountNumber, decimal amount)
    {        
        try
        {
            await _accountService.WithdrawAsync(accountNumber, amount);
            return RedirectToAction("Details", "Account", new { accountNumber = accountNumber });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            var account = await _accountService.GetAccountByNumberAsync(accountNumber);
            ViewBag.Account = account;
            return View();
        }
    }

    public async Task<IActionResult> Transfer(string fromAccountNumber)
    {
        var account = await _accountService.GetAccountByNumberAsync(fromAccountNumber);
        if (account == null) return NotFound();

        ViewBag.FromAccount = account;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Transfer(string fromAccountNumber, string toAccountNumber, decimal amount)
    {
        try
        {
            // Chuyển logic phức tạp này xuống Service để đảm bảo tính toàn vẹn (Transaction)
            await _accountService.TransferAsync(fromAccountNumber, toAccountNumber, amount);
            return RedirectToAction("Details", "Account", new { accountNumber = fromAccountNumber });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            var fromAccount = await _accountService.GetAccountByNumberAsync(fromAccountNumber);
            ViewBag.FromAccount = fromAccount;
            return View();
        }
    }
}