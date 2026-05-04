using System;
using System.Collections.Generic;
using System.Data;

namespace CloudMoney.Logic
{
    public static class ReportManager
    {
        public static DataTable GetStatementOfAccount(int userId, DateTime startDate, DateTime endDate)
        {
            const string sql = @"
SELECT
    t.TransactionDate,
    t.TransactionType,
    t.Amount,
    CASE
        WHEN t.TransactionType = 'Deposit' THEN 'Cash In'
        WHEN t.TransactionType = 'Withdrawal' THEN 'Cash Out'
        WHEN t.TransactionType = 'Transfer' AND t.FromUserId = @UserId THEN 'Sent'
        WHEN t.TransactionType = 'Transfer' AND t.ToUserId = @UserId THEN 'Received'
        ELSE '-'
    END AS Direction,
    ISNULL(sender.AccountNumber, '-') AS SenderAccount,
    ISNULL(sender.FullName, '-') AS SenderName,
    ISNULL(receiver.AccountNumber, '-') AS RecipientAccount,
    ISNULL(receiver.FullName, '-') AS RecipientName,
    ISNULL(t.Remarks, '-') AS Remarks
FROM dbo.Transactions t
LEFT JOIN dbo.Users sender ON sender.UserId = t.FromUserId
LEFT JOIN dbo.Users receiver ON receiver.UserId = t.ToUserId
WHERE
    (
        (t.TransactionType = 'Deposit' AND t.ToUserId = @UserId)
        OR (t.TransactionType = 'Withdrawal' AND t.FromUserId = @UserId)
        OR (t.TransactionType = 'Transfer' AND (t.FromUserId = @UserId OR t.ToUserId = @UserId))
    )
    AND t.TransactionDate >= @StartDate
    AND t.TransactionDate < @EndDateExclusive
ORDER BY t.TransactionDate DESC, t.TransactionId DESC;";

            return DatabaseHelper.GetDataTable(sql, CreateDateParams(userId, startDate, endDate));
        }

        public static DataTable GetMyDepositsWithdrawals(int userId, DateTime startDate, DateTime endDate, string typeFilter)
        {
            var sql = @"
SELECT
    t.TransactionDate,
    t.TransactionType,
    t.Amount,
    ISNULL(t.Remarks, '-') AS Remarks
FROM dbo.Transactions t
WHERE
    (
        (t.TransactionType = 'Deposit' AND t.ToUserId = @UserId)
        OR (t.TransactionType = 'Withdrawal' AND t.FromUserId = @UserId)
    )
    AND t.TransactionDate >= @StartDate
    AND t.TransactionDate < @EndDateExclusive";

            var parameters = CreateDateParams(userId, startDate, endDate);

            if (string.Equals(typeFilter, "Deposit", StringComparison.OrdinalIgnoreCase))
            {
                sql += " AND t.TransactionType = 'Deposit'";
            }
            else if (string.Equals(typeFilter, "Withdrawal", StringComparison.OrdinalIgnoreCase))
            {
                sql += " AND t.TransactionType = 'Withdrawal'";
            }

            sql += " ORDER BY t.TransactionDate DESC, t.TransactionId DESC;";
            return DatabaseHelper.GetDataTable(sql, parameters);
        }

        public static DataTable GetSentReceivedHistory(int userId, DateTime startDate, DateTime endDate, string typeFilter)
        {
            var sql = @"
SELECT
    t.TransactionDate,
    CASE
        WHEN t.FromUserId = @UserId THEN 'Sent'
        ELSE 'Received'
    END AS TransferType,
    t.Amount,
    CASE
        WHEN t.FromUserId = @UserId THEN receiver.AccountNumber
        ELSE sender.AccountNumber
    END AS CounterpartyAccount,
    CASE
        WHEN t.FromUserId = @UserId THEN receiver.FullName
        ELSE sender.FullName
    END AS CounterpartyName,
    ISNULL(t.Remarks, '-') AS Remarks
FROM dbo.Transactions t
INNER JOIN dbo.Users sender ON sender.UserId = t.FromUserId
INNER JOIN dbo.Users receiver ON receiver.UserId = t.ToUserId
WHERE
    t.TransactionType = 'Transfer'
    AND (t.FromUserId = @UserId OR t.ToUserId = @UserId)
    AND t.TransactionDate >= @StartDate
    AND t.TransactionDate < @EndDateExclusive";

            var parameters = CreateDateParams(userId, startDate, endDate);

            if (string.Equals(typeFilter, "Sent", StringComparison.OrdinalIgnoreCase))
            {
                sql += " AND t.FromUserId = @UserId";
            }
            else if (string.Equals(typeFilter, "Received", StringComparison.OrdinalIgnoreCase))
            {
                sql += " AND t.ToUserId = @UserId";
            }

            sql += " ORDER BY t.TransactionDate DESC, t.TransactionId DESC;";
            return DatabaseHelper.GetDataTable(sql, parameters);
        }

        private static Dictionary<string, object> CreateDateParams(int userId, DateTime startDate, DateTime endDate)
        {
            return new Dictionary<string, object>
            {
                {"@UserId", userId },
                {"@StartDate", startDate.Date },
                {"@EndDateExclusive", endDate.Date.AddDays(1) }
            };
        }
    }
}
