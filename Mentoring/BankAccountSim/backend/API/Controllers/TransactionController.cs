using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IAccountService _accountService;

    public TransactionController(ITransactionService transactionService, IAccountService accountService)
    {
        _transactionService = transactionService;
        _accountService = accountService;
    }

    [HttpGet("{accountNumber}/history")]
    public async Task<IActionResult> History(string accountNumber, string filter = "all", int page = 1)
    {
        int pageSize = 10;
        
        var account = await _accountService.GetAccountByNumberAsync(accountNumber);
        if (account == null) return NotFound(new { message = "Account not found" });

        var transactions = await _transactionService.GetHistoryAsync(accountNumber, filter);

        var pageTransactions = transactions
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(new
        {
            Account = account,
            Transactions = pageTransactions,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)transactions.Count() / pageSize),
            Filter = filter
        });
    }


    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody]DepositRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _accountService.DepositAsync(request.AccountNumber, request.Amount);
            return Ok(new { message = "Deposit successfully!"});
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
             
        try
        {
            await _accountService.WithdrawAsync(request.AccountNumber, request.Amount);
            return Ok(new { message = "Withdraw successfully!"});
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpPost]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _accountService.TransferAsync(request.FromAccountNumber, request.ToAccountNumber, request.Amount);
            return Ok(new { message = $"Transfer successfully!"});
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}