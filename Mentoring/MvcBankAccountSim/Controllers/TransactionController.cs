using Microsoft.AspNetCore.Mvc;

public class TransactionController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly IAccountService _accountService;

    public TransactionController(ITransactionService transactionService, IAccountService accountService)
    {
        _transactionService = transactionService;
        _accountService = accountService;
    }

    public IActionResult History(string accountNumber, string filter = "all")
    {
        var account = _accountService.GetAccountByNumber(accountNumber);
        if (account == null) return NotFound();

        var transactions = _transactionService.GetTransactionsByAccountNumber(accountNumber);

        if (!string.IsNullOrWhiteSpace(filter) && filter.ToLower() != "all")
        {
            transactions = transactions.Where(t => t.Type.ToString().Equals(filter, StringComparison.OrdinalIgnoreCase)).ToList();  
        }

        ViewBag.Account = account;
        ViewBag.Filter = filter;
        return View(transactions);
    }

    public IActionResult Deposit(string accountNumber)
    {
        var account = _accountService.GetAccountByNumber(accountNumber);
        if (account == null) return NotFound();

        ViewBag.Account = account;
        return View();
    }

    [HttpPost]
    public IActionResult Deposit(string accountNumber, decimal amount)
    {
        var account = _accountService.GetAccountByNumber(accountNumber);

        if (amount <= 0 || account == null || account.Balance + amount < 100 || account.Status == AccountStatus.FROZEN)
        {
            if (amount <= 0) ModelState.AddModelError("amount", "Amount must be greater than zero.");
            if (account != null && account.Balance + amount < 100) ModelState.AddModelError("amount", "Balance must remain at least $100.");
            if (account?.Status == AccountStatus.FROZEN) ModelState.AddModelError("", "Account is frozen.");

            ViewBag.Account = account; 
            return View();
        }

        account.Deposit(amount);
        _accountService.Update(account);

        var transaction = new Transaction
        {
            AccountNumber = accountNumber,
            Type = TransactionType.DEPOSIT,
            Amount = amount,
            CreatedAt = DateTime.Now
        };
        _transactionService.Add(transaction);

        // After success:
        return RedirectToAction("Details", "Account", new { accountNumber = accountNumber });
    }

    public IActionResult Withdraw(string accountNumber)
    {
        var account = _accountService.GetAccountByNumber(accountNumber);
        if (account == null) return NotFound();

        ViewBag.Account = account;
        return View();
    }

    [HttpPost]
    public IActionResult Withdraw(string accountNumber, decimal amount)
    {        
        var account = _accountService.GetAccountByNumber(accountNumber);

        if (amount <= 0 || account == null || account.Balance - amount < 100 || account.Status == AccountStatus.FROZEN)
        {
            if (amount <= 0) ModelState.AddModelError("amount", "Amount must be greater than zero.");
            if (account != null && account.Balance - amount < 100) ModelState.AddModelError("amount", "Balance must remain at least $100.");
            if (account?.Status == AccountStatus.FROZEN) ModelState.AddModelError("", "Account is frozen.");

            ViewBag.Account = account; 
            return View();
        }

        account.Withdraw(amount);
        _accountService.Update(account);

        var transaction = new Transaction
        {
            AccountNumber = accountNumber,
            Type = TransactionType.WITHDRAW,
            Amount = amount,
            CreatedAt = DateTime.Now
        };
        _transactionService.Add(transaction);

        // After success:
        return RedirectToAction("Details", "Account", new { accountNumber = accountNumber });
    }

    public IActionResult Transfer(string fromAccountNumber)
    {
        var account = _accountService.GetAccountByNumber(fromAccountNumber);
        if (account == null) return NotFound();

        ViewBag.FromAccount = account;
        return View();
    }

    [HttpPost]
    public IActionResult Transfer(string fromAccountNumber, string toAccountNumber, decimal amount)
    {
        var fromAccount = _accountService.GetAccountByNumber(fromAccountNumber);
        var toAccount = _accountService.GetAccountByNumber(toAccountNumber);

        if (amount <= 0 || fromAccount == null || fromAccount.Balance - amount < 100 || fromAccount.Status == AccountStatus.FROZEN)
        {
            if (amount <= 0) ModelState.AddModelError("amount", "Amount must be greater than zero.");
            if (fromAccount != null && fromAccount.Balance - amount < 100) ModelState.AddModelError("amount", "Balance must remain at least $100.");
            if (fromAccount?.Status == AccountStatus.FROZEN) ModelState.AddModelError("", "Account is frozen.");

            ViewBag.FromAccount = fromAccount;
            return View();
        }

        if (toAccount == null)
        {
            ModelState.AddModelError("toAccountNumber", "Destination account not found.");
            ViewBag.FromAccount = fromAccount;
            return View();
        }

        fromAccount.Withdraw(amount);
        toAccount.Deposit(amount);
        _accountService.Update(fromAccount);
        _accountService.Update(toAccount);

        var transactionOut = new Transaction
        {
            AccountNumber = fromAccountNumber,
            Type = TransactionType.TRANSFER,
            Amount = amount,
            CreatedAt = DateTime.Now,
            Description = $"Transfer to {toAccountNumber}"
        };

        var transactionIn = new Transaction
        {
            AccountNumber = toAccountNumber,
            Type = TransactionType.TRANSFER,
            Amount = amount,
            CreatedAt = DateTime.Now,
            Description = $"Transfer from {fromAccountNumber}"
        };

        _transactionService.Add(transactionOut);
        _transactionService.Add(transactionIn);

        // After success:
        return RedirectToAction("Details", "Account", new { accountNumber = fromAccountNumber });
    }
}