using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;

namespace CloudMoney.Logic
{
    public static class AuthManager
    {
        public static RegisterResult Register(string fullName, string email, string password, string confirmPassword)
        {
            var cleanName = (fullName ?? string.Empty).Trim();
            var cleanEmail = (email ?? string.Empty).Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(cleanName))
            {
                return new RegisterResult { Success = false, Message = "Full name is required." };
            }

            if (string.IsNullOrWhiteSpace(cleanEmail))
            {
                return new RegisterResult { Success = false, Message = "Email is required." };
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return new RegisterResult { Success = false, Message = "Password is required." };
            }

            if (password.Length < 6)
            {
                return new RegisterResult { Success = false, Message = "Password must be at least 6 characters." };
            }

            if (password != confirmPassword)
            {
                return new RegisterResult { Success = false, Message = "Passwords do not match." };
            }

            const string duplicateSql = "SELECT COUNT(1) FROM dbo.Users WHERE Email = @Email";
            var emailCount = DatabaseHelper.ExecuteScalar<int>(duplicateSql, new Dictionary<string, object>
            {
                {"@Email", cleanEmail }
            });

            if (emailCount > 0)
            {
                return new RegisterResult { Success = false, Message = "Email address is already registered." };
            }

            var accountNumber = GenerateUniqueAccountNumber();
            const string insertSql = @"
INSERT INTO dbo.Users (AccountNumber, FullName, Email, PasswordPlainText, DateRegistered)
VALUES (@AccountNumber, @FullName, @Email, @PasswordPlainText, @DateRegistered)";

            DatabaseHelper.ExecuteNonQuery(insertSql, new Dictionary<string, object>
            {
                {"@AccountNumber", accountNumber },
                {"@FullName", cleanName },
                {"@Email", cleanEmail },
                {"@PasswordPlainText", password },
                {"@DateRegistered", DateTime.Now }
            });

            return new RegisterResult
            {
                Success = true,
                Message = "Registration successful.",
                AccountNumber = accountNumber
            };
        }

        public static LoginResult Login(string email, string password)
        {
            var cleanEmail = (email ?? string.Empty).Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(cleanEmail) || string.IsNullOrWhiteSpace(password))
            {
                return new LoginResult { Success = false, Message = "Email and password are required." };
            }

            const string sql = @"
SELECT UserId, AccountNumber, FullName
FROM dbo.Users
WHERE Email = @Email AND PasswordPlainText = @Password";

            var table = DatabaseHelper.GetDataTable(sql, new Dictionary<string, object>
            {
                {"@Email", cleanEmail },
                {"@Password", password }
            });

            if (table.Rows.Count == 0)
            {
                return new LoginResult { Success = false, Message = "Invalid email or password." };
            }

            var row = table.Rows[0];
            return new LoginResult
            {
                Success = true,
                Message = "Login successful.",
                UserId = Convert.ToInt32(row["UserId"]),
                AccountNumber = Convert.ToString(row["AccountNumber"]),
                FullName = Convert.ToString(row["FullName"])
            };
        }

        public static OperationResult ChangePassword(int userId, string currentPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                return OperationResult.Fail("Current password is required.");
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                return OperationResult.Fail("New password is required.");
            }

            if (newPassword.Length < 6)
            {
                return OperationResult.Fail("New password must be at least 6 characters.");
            }

            if (newPassword != confirmPassword)
            {
                return OperationResult.Fail("New password and confirmation do not match.");
            }

            const string getCurrentSql = "SELECT PasswordPlainText FROM dbo.Users WHERE UserId = @UserId";
            var storedPassword = DatabaseHelper.ExecuteScalar<string>(getCurrentSql, new Dictionary<string, object>
            {
                {"@UserId", userId }
            });

            if (storedPassword == null)
            {
                return OperationResult.Fail("User account could not be found.");
            }

            if (!string.Equals(storedPassword, currentPassword, StringComparison.Ordinal))
            {
                return OperationResult.Fail("Current password is incorrect.");
            }

            const string updateSql = "UPDATE dbo.Users SET PasswordPlainText = @Password WHERE UserId = @UserId";
            DatabaseHelper.ExecuteNonQuery(updateSql, new Dictionary<string, object>
            {
                {"@Password", newPassword },
                {"@UserId", userId }
            });

            return OperationResult.Ok("Password changed successfully.");
        }

        public static bool ValidatePassword(int userId, string password)
        {
            const string sql = "SELECT COUNT(1) FROM dbo.Users WHERE UserId = @UserId AND PasswordPlainText = @Password";
            var count = DatabaseHelper.ExecuteScalar<int>(sql, new Dictionary<string, object>
            {
                {"@UserId", userId },
                {"@Password", password }
            });

            return count > 0;
        }

        public static DataRow GetUserById(int userId)
        {
            const string sql = @"
SELECT UserId, AccountNumber, FullName, Email, DateRegistered
FROM dbo.Users
WHERE UserId = @UserId";

            var table = DatabaseHelper.GetDataTable(sql, new Dictionary<string, object>
            {
                {"@UserId", userId }
            });

            return table.Rows.Count == 0 ? null : table.Rows[0];
        }

        private static string GenerateUniqueAccountNumber()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                for (var attempt = 0; attempt < 50; attempt++)
                {
                    var buffer = new char[10];
                    for (var i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = (char)('0' + NextDigit(rng));
                    }

                    if (buffer[0] == '0')
                    {
                        buffer[0] = (char)('1' + NextDigit(rng, 9));
                    }

                    var accountNumber = new string(buffer);
                    var count = DatabaseHelper.ExecuteScalar<int>(
                        "SELECT COUNT(1) FROM dbo.Users WHERE AccountNumber = @AccountNumber",
                        new Dictionary<string, object> { {"@AccountNumber", accountNumber } });

                    if (count == 0)
                    {
                        return accountNumber;
                    }
                }
            }

            throw new InvalidOperationException("Could not allocate a unique account number. Please try again.");
        }

        private static int NextDigit(RandomNumberGenerator rng, int maxExclusive = 10)
        {
            var bytes = new byte[4];
            int value;

            do
            {
                rng.GetBytes(bytes);
                value = BitConverter.ToInt32(bytes, 0) & int.MaxValue;
            } while (value >= maxExclusive * (int.MaxValue / maxExclusive));

            return value % maxExclusive;
        }
    }
}
