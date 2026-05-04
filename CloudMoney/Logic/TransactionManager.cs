using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CloudMoney.Logic
{
    public static class TransactionManager
    {
        public static BalanceSnapshot GetBalanceSnapshot(int userId)
        {
            const string sql = @"
SELECT
    CAST(ISNULL(SUM(
        CASE
            WHEN TransactionType = 'Deposit' AND ToUserId = @UserId THEN Amount
            WHEN TransactionType = 'Withdrawal' AND FromUserId = @UserId THEN -Amount
            WHEN TransactionType = 'Transfer' AND ToUserId = @UserId THEN Amount
            WHEN TransactionType = 'Transfer' AND FromUserId = @UserId THEN -Amount
            ELSE 0
        END
    ), 0) AS DECIMAL(18,2)) AS CurrentBalance,
    CAST(ISNULL(SUM(
        CASE
            WHEN TransactionType = 'Transfer' AND FromUserId = @UserId THEN Amount
            ELSE 0
        END
    ), 0) AS DECIMAL(18,2)) AS TotalSent
FROM dbo.Transactions;";

            var table = DatabaseHelper.GetDataTable(sql, new Dictionary<string, object>
            {
                {"@UserId", userId }
            });

            if (table.Rows.Count == 0)
            {
                return new BalanceSnapshot();
            }

            var row = table.Rows[0];
            return new BalanceSnapshot
            {
                CurrentBalance = Convert.ToDecimal(row["CurrentBalance"]),
                TotalSent = Convert.ToDecimal(row["TotalSent"])
            };
        }

        public static OperationResult Deposit(int userId, decimal amount)
        {
            if (!ValidationHelper.ValidateAmount(amount, out var error))
            {
                return OperationResult.Fail(error);
            }

            var balance = GetBalanceSnapshot(userId).CurrentBalance;
            var newBalance = balance + amount;

            if (newBalance > ValidationHelper.MaximumBalance)
            {
                return OperationResult.Fail("Deposit will exceed the maximum balance of P10,000.00.");
            }

            const string sql = @"
INSERT INTO dbo.Transactions (TransactionType, FromUserId, ToUserId, Amount, TransactionDate, Remarks)
VALUES ('Deposit', NULL, @UserId, @Amount, @TransactionDate, @Remarks);";

            DatabaseHelper.ExecuteNonQuery(sql, new Dictionary<string, object>
            {
                {"@UserId", userId },
                {"@Amount", amount },
                {"@TransactionDate", DateTime.Now },
                {"@Remarks", "Cash deposit" }
            });

            return OperationResult.Ok("Deposit completed successfully.");
        }

        public static OperationResult Withdraw(int userId, decimal amount)
        {
            if (!ValidationHelper.ValidateAmount(amount, out var error))
            {
                return OperationResult.Fail(error);
            }

            var balance = GetBalanceSnapshot(userId).CurrentBalance;
            if (balance < amount)
            {
                return OperationResult.Fail("Insufficient balance for this withdrawal.");
            }

            const string sql = @"
INSERT INTO dbo.Transactions (TransactionType, FromUserId, ToUserId, Amount, TransactionDate, Remarks)
VALUES ('Withdrawal', @UserId, NULL, @Amount, @TransactionDate, @Remarks);";

            DatabaseHelper.ExecuteNonQuery(sql, new Dictionary<string, object>
            {
                {"@UserId", userId },
                {"@Amount", amount },
                {"@TransactionDate", DateTime.Now },
                {"@Remarks", "Cash withdrawal" }
            });

            return OperationResult.Ok("Withdrawal completed successfully.");
        }

        public static RecipientLookupResult FindRecipient(string accountNumber, int currentUserId)
        {
            var cleanAccount = (accountNumber ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(cleanAccount))
            {
                return new RecipientLookupResult { Success = false, Message = "Recipient account number is required." };
            }

            const string sql = @"
SELECT UserId, AccountNumber, FullName
FROM dbo.Users
WHERE AccountNumber = @AccountNumber";

            var table = DatabaseHelper.GetDataTable(sql, new Dictionary<string, object>
            {
                {"@AccountNumber", cleanAccount }
            });

            if (table.Rows.Count == 0)
            {
                return new RecipientLookupResult { Success = false, Message = "Recipient account number was not found." };
            }

            var row = table.Rows[0];
            var recipientId = Convert.ToInt32(row["UserId"]);
            if (recipientId == currentUserId)
            {
                return new RecipientLookupResult { Success = false, Message = "You cannot send money to your own account." };
            }

            return new RecipientLookupResult
            {
                Success = true,
                Message = "Recipient found.",
                UserId = recipientId,
                AccountNumber = Convert.ToString(row["AccountNumber"]),
                FullName = Convert.ToString(row["FullName"])
            };
        }

        public static OperationResult SendMoney(int fromUserId, int toUserId, decimal amount, string password)
        {
            if (!ValidationHelper.ValidateAmount(amount, out var error))
            {
                return OperationResult.Fail(error);
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return OperationResult.Fail("Password confirmation is required.");
            }

            if (!AuthManager.ValidatePassword(fromUserId, password))
            {
                return OperationResult.Fail("Password confirmation is incorrect.");
            }

            var balance = GetBalanceSnapshot(fromUserId).CurrentBalance;
            if (balance < amount)
            {
                return OperationResult.Fail("Insufficient balance to send CloudMoney.");
            }

            using (var connection = DatabaseHelper.CreateOpenConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Verify recipient inside transaction to avoid stale recipient ids.
                    const string recipientSql = "SELECT COUNT(1) FROM dbo.Users WHERE UserId = @RecipientId";
                    using (var checkRecipient = DatabaseHelper.CreateCommand(connection, transaction, recipientSql, new Dictionary<string, object>
                    {
                        {"@RecipientId", toUserId }
                    }))
                    {
                        var exists = Convert.ToInt32(checkRecipient.ExecuteScalar());
                        if (exists == 0)
                        {
                            transaction.Rollback();
                            return OperationResult.Fail("Recipient account no longer exists.");
                        }
                    }

                    const string balanceSql = @"
SELECT CAST(ISNULL(SUM(
    CASE
        WHEN TransactionType = 'Deposit' AND ToUserId = @UserId THEN Amount
        WHEN TransactionType = 'Withdrawal' AND FromUserId = @UserId THEN -Amount
        WHEN TransactionType = 'Transfer' AND ToUserId = @UserId THEN Amount
        WHEN TransactionType = 'Transfer' AND FromUserId = @UserId THEN -Amount
        ELSE 0
    END), 0) AS DECIMAL(18,2))
FROM dbo.Transactions;";

                    decimal refreshedBalance;
                    using (var getBalance = DatabaseHelper.CreateCommand(connection, transaction, balanceSql, new Dictionary<string, object>
                    {
                        {"@UserId", fromUserId }
                    }))
                    {
                        refreshedBalance = Convert.ToDecimal(getBalance.ExecuteScalar());
                    }

                    if (refreshedBalance < amount)
                    {
                        transaction.Rollback();
                        return OperationResult.Fail("Insufficient balance to send CloudMoney.");
                    }

                    const string insertSql = @"
INSERT INTO dbo.Transactions (TransactionType, FromUserId, ToUserId, Amount, TransactionDate, Remarks)
VALUES ('Transfer', @FromUserId, @ToUserId, @Amount, @TransactionDate, @Remarks);";

                    using (var insert = DatabaseHelper.CreateCommand(connection, transaction, insertSql, new Dictionary<string, object>
                    {
                        {"@FromUserId", fromUserId },
                        {"@ToUserId", toUserId },
                        {"@Amount", amount },
                        {"@TransactionDate", DateTime.Now },
                        {"@Remarks", "CloudMoney transfer" }
                    }))
                    {
                        insert.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return OperationResult.Ok("CloudMoney sent successfully.");
                }
                catch (SqlException)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public static DataTable GetRecentReceivedTransfers(int userId, int topCount)
        {
            const string sql = @"
SELECT TOP (@TopCount)
    t.Amount,
    t.TransactionDate,
    u.AccountNumber AS SenderAccountNumber,
    u.FullName AS SenderFullName
FROM dbo.Transactions t
INNER JOIN dbo.Users u ON u.UserId = t.FromUserId
WHERE t.TransactionType = 'Transfer' AND t.ToUserId = @UserId
ORDER BY t.TransactionDate DESC;";

            return DatabaseHelper.GetDataTable(sql, new Dictionary<string, object>
            {
                {"@TopCount", topCount },
                {"@UserId", userId }
            });
        }
    }
}
