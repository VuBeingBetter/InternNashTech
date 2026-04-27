using Moq;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Xunit;

namespace Tests;

public class AccountServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly AccountService _service;

    public AccountServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _transactionRepositoryMock = new Mock<ITransactionRepository>();

        _unitOfWorkMock.SetupGet(x => x.Accounts).Returns(_accountRepositoryMock.Object);
        _unitOfWorkMock.SetupGet(x => x.Transactions).Returns(_transactionRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        _service = new AccountService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAccountAsync_ValidInput_CreatesAndSavesAccount()
    {
        // Arrange
        _accountRepositoryMock.SetupSequence(x => x.GetByAccountNumberAsync(It.IsAny<string>()))
            .ReturnsAsync(new BankAccount("1111111111", "Duplicate", 100))
            .ReturnsAsync((BankAccount?)null);

        // Act
        await _service.CreateAccountAsync("Alice", 200);

        // Assert
        _accountRepositoryMock.Verify(x => x.AddAsync(It.Is<BankAccount>(a => a.OwnerName == "Alice" && a.Balance == 200)), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAccountsAsync_AnyInput_ReturnsAllAccounts()
    {
        // Arrange
        var expected = new[] { new BankAccount("123", "Bob", 100) };
        _accountRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(expected);

        // Act
        var actual = await _service.GetAllAccountsAsync();

        // Assert
        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task GetAccountByNumberAsync_ValidAccountNumber_ReturnsAccount()
    {
        // Arrange
        var expected = new BankAccount("123", "Bob", 100);
        _accountRepositoryMock.Setup(x => x.GetByAccountNumberAsync("123")).ReturnsAsync(expected);

        // Act
        var actual = await _service.GetAccountByNumberAsync("123");

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task DepositAsync_ActiveAccount_UpdatesBalanceAndRecordsTransaction()
    {
        // Arrange
        var account = new BankAccount("123", "Bob", 150);
        _accountRepositoryMock.Setup(x => x.GetByAccountNumberAsync("123")).ReturnsAsync(account);

        // Act
        await _service.DepositAsync("123", 200);

        // Assert
        Assert.Equal(350, account.Balance);
        _transactionRepositoryMock.Verify(x => x.AddAsync(It.Is<Transaction>(t => t.AccountNumber == "123" && t.Type == TransactionType.DEPOSIT && t.Amount == 200)), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DepositAsync_FrozenAccount_ThrowsInvalidOperationException()
    {
        // Arrange
        var account = new BankAccount("123", "Bob", 150) { Status = AccountStatus.FROZEN };
        _accountRepositoryMock.Setup(x => x.GetByAccountNumberAsync("123")).ReturnsAsync(account);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DepositAsync("123", 100));
    }

    [Fact]
    public async Task WithdrawAsync_SufficientBalance_UpdatesBalanceAndRecordsTransaction()
    {
        // Arrange
        var account = new BankAccount("123", "Claire", 500);
        _accountRepositoryMock.Setup(x => x.GetByAccountNumberAsync("123")).ReturnsAsync(account);

        // Act
        await _service.WithdrawAsync("123", 200);

        // Assert
        Assert.Equal(300, account.Balance);
        _transactionRepositoryMock.Verify(x => x.AddAsync(It.Is<Transaction>(t => t.AccountNumber == "123" && t.Type == TransactionType.WITHDRAW && t.Amount == 200)), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task WithdrawAsync_InsufficientBalance_ThrowsInvalidOperationException()
    {
        // Arrange
        var account = new BankAccount("123", "Claire", 150);
        _accountRepositoryMock.Setup(x => x.GetByAccountNumberAsync("123")).ReturnsAsync(account);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.WithdrawAsync("123", 100));
    }

    [Fact]
    public async Task TransferAsync_ValidTransfer_TransfersFundsAndRecordsTwoTransactions()
    {
        // Arrange
        var source = new BankAccount("SRC123", "Don", 500);
        var destination = new BankAccount("DST456", "Eve", 100);
        _accountRepositoryMock.Setup(x => x.GetByAccountNumberAsync("SRC123")).ReturnsAsync(source);
        _accountRepositoryMock.Setup(x => x.GetByAccountNumberAsync("DST456")).ReturnsAsync(destination);

        // Act
        await _service.TransferAsync("SRC123", "DST456", 200);

        // Assert
        Assert.Equal(300, source.Balance);
        Assert.Equal(300, destination.Balance);
        _transactionRepositoryMock.Verify(x => x.AddAsync(It.Is<Transaction>(t => t.AccountNumber == "SRC123" && t.Type == TransactionType.TRANSFER && t.Amount == 200)), Times.Once);
        _transactionRepositoryMock.Verify(x => x.AddAsync(It.Is<Transaction>(t => t.AccountNumber == "DST456" && t.Type == TransactionType.TRANSFER && t.Amount == 200)), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task TransferAsync_MissingDestination_ThrowsException()
    {
        // Arrange
        var source = new BankAccount("SRC123", "Don", 500);
        _accountRepositoryMock.Setup(x => x.GetByAccountNumberAsync("SRC123")).ReturnsAsync(source);
        _accountRepositoryMock.Setup(x => x.GetByAccountNumberAsync("DST456")).ReturnsAsync((BankAccount?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _service.TransferAsync("SRC123", "DST456", 100));
        Assert.Equal("Destination account not found.", ex.Message);
    }

    [Fact]
    public async Task ToggleStatusAsync_ActiveAccount_TogglesStatusToFrozen()
    {
        // Arrange
        var account = new BankAccount("123", "Frank", 200);
        _accountRepositoryMock.Setup(x => x.GetByAccountNumberAsync("123")).ReturnsAsync(account);

        // Act
        await _service.ToggleStatusAsync("123");

        // Assert
        Assert.Equal(AccountStatus.FROZEN, account.Status);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ToggleStatusAsync_FrozenAccount_TogglesStatusToActive()
    {
        // Arrange
        var account = new BankAccount("123", "Frank", 200) { Status = AccountStatus.FROZEN };
        _accountRepositoryMock.Setup(x => x.GetByAccountNumberAsync("123")).ReturnsAsync(account);

        // Act
        await _service.ToggleStatusAsync("123");

        // Assert
        Assert.Equal(AccountStatus.ACTIVE, account.Status);
    }
}
