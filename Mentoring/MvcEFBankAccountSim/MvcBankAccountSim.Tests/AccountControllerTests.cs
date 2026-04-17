using Microsoft.AspNetCore.Mvc;
using Moq;
using MvcBankAccountSim.API.Controllers;
using MvcBankAccountSim.Application.DTOs;
using MvcBankAccountSim.Application.Interfaces;
using MvcBankAccountSim.Domain.Entities;
using Xunit;

namespace MvcBankAccountSim.Tests;

public class AccountControllerTests
{
    private readonly Mock<IAccountService> _accountServiceMock;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        _accountServiceMock = new Mock<IAccountService>();
        _controller = new AccountController(_accountServiceMock.Object);
    }

    [Fact]
    public async Task Index_AnyInput_ReturnsViewWithAccounts()
    {
        // Arrange
        var accounts = new[] { new BankAccount("123", "Joe", 200) };
        _accountServiceMock.Setup(x => x.GetAllAccountsAsync()).ReturnsAsync(accounts);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(accounts, viewResult.Model);
    }

    [Fact]
    public void Create_GetRequest_ReturnsView()
    {
        // Act
        var result = _controller.Create();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_InvalidOwnerName_ReturnsViewWithModelError()
    {
        // Arrange
        var request = new CreateAccountRequest { OwnerName = string.Empty, Balance = 200 };
        _controller.ModelState.AddModelError("OwnerName", "Owner name is required.");

        // Act
        var result = await _controller.Create(request);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Contains(_controller.ModelState, kvp => kvp.Value.Errors.Count > 0);
    }

    [Fact]
    public async Task Create_InvalidBalance_ReturnsViewWithModelError()
    {
        // Arrange
        var request = new CreateAccountRequest { OwnerName = "Jane", Balance = 50 };
        _controller.ModelState.AddModelError("Balance", "Balance cannot be less than $100.");

        // Act
        var result = await _controller.Create(request);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Create_ValidInput_RedirectsToIndex()
    {
        // Arrange
        var request = new CreateAccountRequest { OwnerName = "Jane", Balance = 200 };
        _accountServiceMock.Setup(x => x.CreateAccountAsync("Jane", 200)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AccountController.Index), redirectResult.ActionName);
    }

    [Fact]
    public async Task Create_ServiceThrowsException_AddsModelErrorAndReturnsView()
    {
        // Arrange
        var request = new CreateAccountRequest { OwnerName = "Jane", Balance = 200 };
        _accountServiceMock.Setup(x => x.CreateAccountAsync("Jane", 200)).ThrowsAsync(new Exception("Duplicate account number"));

        // Act
        var result = await _controller.Create(request);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Contains(_controller.ModelState["AccountNumber"].Errors, e => e.ErrorMessage == "Duplicate account number");
    }

    [Fact]
    public async Task Details_MissingAccountNumber_ReturnsNotFound()
    {
        // Act
        var result = await _controller.Details(string.Empty);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_AccountNotFound_ReturnsNotFound()
    {
        // Arrange
        _accountServiceMock.Setup(x => x.GetAccountByNumberAsync("123")).ReturnsAsync((BankAccount?)null);

        // Act
        var result = await _controller.Details("123");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ExistingAccount_ReturnsViewWithAccount()
    {
        // Arrange
        var account = new BankAccount("123", "Joe", 200);
        _accountServiceMock.Setup(x => x.GetAccountByNumberAsync("123")).ReturnsAsync(account);

        // Act
        var result = await _controller.Details("123");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(account, viewResult.Model);
    }

    [Fact]
    public async Task ToggleStatus_ServiceThrowsException_RedirectsToIndexWithError()
    {
        // Arrange
        _accountServiceMock.Setup(x => x.ToggleStatusAsync("123")).ThrowsAsync(new Exception("Oops"));

        // Act
        var result = await _controller.ToggleStatus("123");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AccountController.Index), redirectResult.ActionName);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task GetOwnerName_MissingAccountNumber_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetOwnerName(string.Empty);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetOwnerName_AccountNotFound_ReturnsJsonFoundFalse()
    {
        // Arrange
        _accountServiceMock.Setup(x => x.GetAccountByNumberAsync("123")).ReturnsAsync((BankAccount?)null);

        // Act
        var result = await _controller.GetOwnerName("123");

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.False((bool?)GetJsonValue(jsonResult, "found"));
    }

    [Fact]
    public async Task GetOwnerName_ExistingAccount_ReturnsJsonWithName()
    {
        // Arrange
        var account = new BankAccount("123", "Laura", 150);
        _accountServiceMock.Setup(x => x.GetAccountByNumberAsync("123")).ReturnsAsync(account);

        // Act
        var result = await _controller.GetOwnerName("123");

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.True((bool?)GetJsonValue(jsonResult, "found"));
        Assert.Equal("Laura", (string?)GetJsonValue(jsonResult, "name"));
    }

    private static object? GetJsonValue(JsonResult jsonResult, string propertyName)
    {
        return jsonResult.Value?.GetType().GetProperty(propertyName)?.GetValue(jsonResult.Value);
    }
}
