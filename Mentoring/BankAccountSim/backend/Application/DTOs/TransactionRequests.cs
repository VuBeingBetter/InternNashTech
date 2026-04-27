// File: Application/DTOs/TransactionRequests.cs
public record DepositRequest(string AccountNumber, decimal Amount);
public record WithdrawRequest(string AccountNumber, decimal Amount);
public record TransferRequest(string FromAccountNumber, string ToAccountNumber, decimal Amount);