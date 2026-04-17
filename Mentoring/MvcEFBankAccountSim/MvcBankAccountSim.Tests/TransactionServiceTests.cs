using Moq;
using MvcBankAccountSim.Application.Interfaces;
using MvcBankAccountSim.Application.Services;
using MvcBankAccountSim.Domain.Entities;
using MvcBankAccountSim.Domain.Enums;
using Xunit;

namespace MvcBankAccountSim.Tests;

public class TransactionServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly TransactionService _service;

    public TransactionServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _unitOfWorkMock.SetupGet(x => x.Transactions).Returns(_transactionRepositoryMock.Object);
        _service = new TransactionService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetHistoryAsync_EmptyFilter_ReturnsAllTransactions()
    {
        // Arrange
        var transactions = new[]
        {
            new Transaction("111", TransactionType.DEPOSIT, 100, "Deposit"),
            new Transaction("111", TransactionType.WITHDRAW, 50, "Withdraw")
        };
        _transactionRepositoryMock.Setup(x => x.GetByAccountNumberAsync("111")).ReturnsAsync(transactions);

        // Act
        var result = await _service.GetHistoryAsync("111", string.Empty);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetHistoryAsync_FilterMatchesType_ReturnsFilteredTransactions()
    {
        // Arrange
        var transactions = new[]
        {
            new Transaction("111", TransactionType.DEPOSIT, 100, "Deposit"),
            new Transaction("111", TransactionType.WITHDRAW, 50, "Withdraw")
        };
        _transactionRepositoryMock.Setup(x => x.GetByAccountNumberAsync("111")).ReturnsAsync(transactions);

        // Act
        var result = await _service.GetHistoryAsync("111", "withdraw");

        // Assert
        Assert.Single(result);
        Assert.All(result, t => Assert.Equal(TransactionType.WITHDRAW, t.Type));
    }
}
