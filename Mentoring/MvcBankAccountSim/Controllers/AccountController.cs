using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public IActionResult Index()
    {
        var accounts = _accountService.GetAllAccounts();
        return View(accounts);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(string accountNumber, string ownerName, decimal balance)
    {
        // 1. Manually check if the basic inputs are valid
        if (string.IsNullOrEmpty(accountNumber) || string.IsNullOrEmpty(ownerName))
        {
            ModelState.AddModelError("", "Account Number and Owner Name are required.");
            return View();
        }

        try
        {
            // 2. Use your constructor! This is the ONLY way to set a 'private set' 
            // property during the creation process from a form.
            var newAccount = new BankAccount(accountNumber, ownerName, balance);

            _accountService.Add(newAccount);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            // Scenario: Duplicate account number
            ModelState.AddModelError("AccountNumber", ex.Message);
            return View();
        }
    }
    public IActionResult Details(string accountNumber)
    {
        var account = _accountService.GetAccountByNumber(accountNumber);
        if (account == null) return NotFound();
        return View(account);
    }

    [HttpPost]
    public IActionResult Freeze(string accountNumber)
    {
        var account = _accountService.GetAccountByNumber(accountNumber);
        if (account != null)
        {
            account.ChangeStatus(AccountStatus.FROZEN);
            _accountService.Update(account);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Unfreeze(string accountNumber)
    {
        var account = _accountService.GetAccountByNumber(accountNumber);
        if (account != null)
        {
            account.ChangeStatus(AccountStatus.ACTIVE);
            _accountService.Update(account);
        }
        return RedirectToAction("Index");
    }
}