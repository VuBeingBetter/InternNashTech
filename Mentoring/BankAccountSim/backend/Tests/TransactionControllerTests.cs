using Microsoft.AspNetCore.Mvc;
using Moq;
using API.Controllers;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Xunit;

namespace Tests;

public class TransactionControllerTests
{
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<IAccountService> _accountServiceMock;
    private readonly TransactionController _controller;

    public TransactionControllerTests()
    {
        _transactionServiceMock = new Mock<ITransactionService>();
        _accountServiceMock = new Mock<IAccountService>();
        _controller = new TransactionController(_transactionServiceMock.Object, _accountServiceMock.Object);
    }

    [Fact]
    public async Task History_MissingAccountNumber_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.History(string.Empty);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task History_AccountNotFound_ReturnsNotFound()
    {
        // Arrange
        _accountServiceMock.Setup(x => x.GetAccountByNumberAsync("123")).ReturnsAsync((BankAccount?)null);

        // Act
        var result = await _controller.History("123");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task History_ExistingAccount_ReturnsViewWithTransactions()
    {
        // Arrange
        var account = new BankAccount("123", "Pam", 150);
        var transactions = new[] { new Transaction("123", TransactionType.DEPOSIT, 100, "Deposit") };
        _accountServiceMock.Setup(x => x.GetAccountByNumberAsync("123")).ReturnsAsync(account);
        _transactionServiceMock.Setup(x => x.GetHistoryAsync("123", "all")).ReturnsAsync(transactions);

        // Act
        var result = await _controller.History("123", "all");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(transactions, viewResult.Model);
    }

    [Fact]
    public async Task Deposit_GetRequest_AccountNotFound_ReturnsNotFound()
    {
        // Arrange
        _accountServiceMock.Setup(x => x.GetAccountByNumberAsync("123")).ReturnsAsync((BankAccount?)null);

        // Act
        var result = await _controller.Deposit("123");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Deposit_GetRequest_ExistingAccount_ReturnsView()
    {
        // Arrange
        var account = new BankAccount("123", "Pam", 150);
        _accountServiceMock.Setup(x => x.GetAccountByNumberAsync("123")).ReturnsAsync(account);

        // Act
        var result = await _controller.Deposit("123");

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Deposit_ValidInput_RedirectsToDetails()
    {
        // Arrange
        var request = new DepositRequest("123", 100);
        _accountServiceMock.Setup(x => x.DepositAsync("123", 100)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Deposit(request);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectResult.ActionName);
        Assert.Equal("Account", redirectResult.ControllerName);
        Assert.Equal("123", redirectResult.RouteValues!["accountNumber"]);
    }

    [Fact]
    public async Task Deposit_ServiceThrowsException_ReturnsViewWithModelError()
    {
        // Arrange
        var request = new DepositRequest("123", 100);
        _accountServiceMock.Setup(x => x.DepositAsync("123", 100)).ThrowsAsync(new Exception("Bad deposit"));
        _accountServiceMock.Setup(x => x.GetAccountByNumberAsync("123")).ReturnsAsync(new BankAccount("123", "Pam", 150));

        // Act
        var result = await _controller.Deposit(request);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Withdraw_GetRequest_AccountNotFound_ReturnsNotFound()
    {
        // Arrange
        _accountServiceMock.Setup(x => x.GetAccountByNumberAsync("123")).ReturnsAsync((BankAccount?)null);

        // Act
        var result = await _controller.Withdraw("123");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Withdraw_GetRequest_ExistingAccount_ReturnsView()
    {
        // Arrange
        _accountServiceMock.Setup(x => x.GetAccountByNumberAsync("123")).ReturnsAsync(new BankAccount("123", "Pam", 150));

        // Act
        var result = await _controller.Withdraw("123");

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Withdraw_ValidInput_RedirectsToDetails()
    {
        // Arrange
        var request = new WithdrawRequest("123", 50);
        _accountServiceMock.Setup(x => x.WithdrawAsync("123", 50)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Withdraw(request);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectResult.ActionName);
        Assert.Equal("Account", redirectResult.ControllerName);
        Assert.Equal("123", redirectResult.RouteValues!["accountNumber"]);
    }

    [Fact]
    public async Task Withdraw_ServiceThrowsException_ReturnsViewWithModelError()
    {
        // Arrange
        var request = new WithdrawRequest("123", 50);
        _accountServiceMock.Setup(x => x.WithdrawAsync("123", 50)).ThrowsAsync(new Exception("Bad withdrawal"));
        _accountServiceMock.Setup(x => x.GetAccountByNumberAsync("123")).ReturnsAsync(new BankAccount("123", "Pam", 150));

        // Act
        var result = await _controller.Withdraw(request);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.NotNull(viewResult.ViewData);
    }

    [Fact]
    public async Task Transfer_GetRequest_AccountNotFound_ReturnsNotFound()
    {
        // Arrange
        _accountServiceMock.Setup(x => x.GetAccountByNumberAsync("SRC123")).ReturnsAsync((BankAccount?)null);

        // Act
        var result = await _controller.Transfer("SRC123");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Transfer_GetRequest_ExistingAccount_ReturnsView()
    {
        // Arrange
        _accountServiceMock.Setup(x => x.GetAccountByNumberAsync("SRC123")).ReturnsAsync(new BankAccount("SRC123", "Pam", 150));

        // Act
        var result = await _controller.Transfer("SRC123");

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Transfer_ValidInput_RedirectsToDetails()
    {
        // Arrange
        var request = new TransferRequest("SRC123", "DST123", 75);
        _accountServiceMock.Setup(x => x.TransferAsync("SRC123", "DST123", 75)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Transfer(request);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectResult.ActionName);
        Assert.Equal("Account", redirectResult.ControllerName);
        Assert.Equal("SRC123", redirectResult.RouteValues!["accountNumber"]);
    }

    [Fact]
    public async Task Transfer_ServiceThrowsException_ReturnsViewWithModelError()
    {
        // Arrange
        var request = new TransferRequest("SRC123", "DST123", 75);
        _accountServiceMock.Setup(x => x.TransferAsync("SRC123", "DST123", 75)).ThrowsAsync(new Exception("Bad transfer"));
        _accountServiceMock.Setup(x => x.GetAccountByNumberAsync("SRC123")).ReturnsAsync(new BankAccount("SRC123", "Pam", 150));

        // Act
        var result = await _controller.Transfer(request);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }
}
