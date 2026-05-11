using System;
using System.Globalization;

namespace CloudMoney.Logic
{
    public static class ValidationHelper
    {
        public const decimal MinimumTransactionAmount = 100m;
        public const decimal MaximumTransactionAmount = 2000m;
        public const decimal MaximumBalance = 10000m;

        public static bool TryParseAmount(string rawAmount, out decimal amount, out string message)
        {
            var normalized = (rawAmount ?? string.Empty).Replace("P", string.Empty).Replace("p", string.Empty).Trim();

            if (!decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out amount)
                && !decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.CurrentCulture, out amount))
            {
                message = "Enter a valid amount.";
                return false;
            }

            return ValidateAmount(amount, out message);
        }

        public static bool ValidateAmount(decimal amount, out string message)
        {
            if (amount < MinimumTransactionAmount)
            {
                message = "Amount must be at least P100.00.";
                return false;
            }

            if (amount > MaximumTransactionAmount)
            {
                message = "Amount must not exceed P2,000.00.";
                return false;
            }

            if (amount % 100m != 0m)
            {
                message = "Amount must be divisible by 100.00.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        public static bool ValidateDateRange(DateTime startDate, DateTime endDate, out string message)
        {
            var today = DateTime.Today;
            var start = startDate.Date;
            var end = endDate.Date;

            if (start > today || end > today)
            {
                message = "Date filters cannot be in the future.";
                return false;
            }

            if (start >= end)
            {
                message = "Start date must be earlier than end date.";
                return false;
            }

            message = string.Empty;
            return true;
        }
    }
}
