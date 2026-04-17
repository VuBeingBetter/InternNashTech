using Microsoft.AspNetCore.Mvc;
using MvcBankAccountSim.Application.Interfaces;

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
    public async Task<IActionResult> Deposit(DepositRequest request)
    {
        if (!ModelState.IsValid)
        {
            var account = await _accountService.GetAccountByNumberAsync(request.AccountNumber);
            ViewBag.Account = account;
            return View(request);
        }
        try
        {
            await _accountService.DepositAsync(request.AccountNumber, request.Amount);
            return RedirectToAction("Details", "Account", new {accountNumber = request.AccountNumber});
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            var account = await _accountService.GetAccountByNumberAsync(request.AccountNumber);
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
    public async Task<IActionResult> Withdraw(WithdrawRequest request)
    {
        if (!ModelState.IsValid)
        {
            var account = await _accountService.GetAccountByNumberAsync(request.AccountNumber);
            ViewBag.Account = account;
            return View(request);
        }       
        try
        {
            await _accountService.WithdrawAsync(request.AccountNumber, request.Amount);
            return RedirectToAction("Details", "Account", new { accountNumber = request.AccountNumber });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            var account = await _accountService.GetAccountByNumberAsync(request.AccountNumber);
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
    public async Task<IActionResult> Transfer(TransferRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.FromAccount = await _accountService.GetAccountByNumberAsync(request.FromAccountNumber);
            return View(request);
        }
        try
        {
            // Chuyển logic phức tạp này xuống Service để đảm bảo tính toàn vẹn (Transaction)
            await _accountService.TransferAsync(request.FromAccountNumber, request.ToAccountNumber, request.Amount);
            return RedirectToAction("Details", "Account", new { accountNumber = request.FromAccountNumber });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            var fromAccount = await _accountService.GetAccountByNumberAsync(request.FromAccountNumber);
            ViewBag.FromAccount = fromAccount;
            return View();
        }
    }
}