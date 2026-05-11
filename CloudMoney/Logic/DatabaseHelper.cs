using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CloudMoney.Logic
{
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["CloudMoneyDB"]?.ConnectionString
            ?? throw new InvalidOperationException("CloudMoneyDB connection string is missing.");

        public static SqlConnection CreateOpenConnection()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public static void EnsureDatabase()
        {
            const string sql = @"
IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        AccountNumber CHAR(10) NOT NULL,
        FullName NVARCHAR(120) NOT NULL,
        Email NVARCHAR(150) NOT NULL,
        PasswordPlainText NVARCHAR(200) NOT NULL,
        DateRegistered DATETIME2 NOT NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Users_AccountNumber' AND object_id = OBJECT_ID('dbo.Users'))
BEGIN
    CREATE UNIQUE INDEX UX_Users_AccountNumber ON dbo.Users(AccountNumber);
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Users_Email' AND object_id = OBJECT_ID('dbo.Users'))
BEGIN
    CREATE UNIQUE INDEX UX_Users_Email ON dbo.Users(Email);
END;

IF OBJECT_ID('dbo.Transactions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Transactions
    (
        TransactionId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TransactionType NVARCHAR(20) NOT NULL,
        FromUserId INT NULL,
        ToUserId INT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        TransactionDate DATETIME2 NOT NULL,
        Remarks NVARCHAR(250) NULL,
        CONSTRAINT FK_Transactions_FromUser FOREIGN KEY (FromUserId) REFERENCES dbo.Users(UserId),
        CONSTRAINT FK_Transactions_ToUser FOREIGN KEY (ToUserId) REFERENCES dbo.Users(UserId)
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Transactions_TransactionDate' AND object_id = OBJECT_ID('dbo.Transactions'))
BEGIN
    CREATE INDEX IX_Transactions_TransactionDate ON dbo.Transactions(TransactionDate);
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Transactions_FromUserId' AND object_id = OBJECT_ID('dbo.Transactions'))
BEGIN
    CREATE INDEX IX_Transactions_FromUserId ON dbo.Transactions(FromUserId);
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Transactions_ToUserId' AND object_id = OBJECT_ID('dbo.Transactions'))
BEGIN
    CREATE INDEX IX_Transactions_ToUserId ON dbo.Transactions(ToUserId);
END;
";

            ExecuteNonQuery(sql);
        }

        public static int ExecuteNonQuery(string sql, Dictionary<string, object> parameters = null)
        {
            using (var connection = CreateOpenConnection())
            using (var command = CreateCommand(connection, null, sql, parameters))
            {
                return command.ExecuteNonQuery();
            }
        }

        public static T ExecuteScalar<T>(string sql, Dictionary<string, object> parameters = null)
        {
            using (var connection = CreateOpenConnection())
            using (var command = CreateCommand(connection, null, sql, parameters))
            {
                var result = command.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    return default(T);
                }

                return (T)Convert.ChangeType(result, typeof(T));
            }
        }

        public static DataTable GetDataTable(string sql, Dictionary<string, object> parameters = null)
        {
            using (var connection = CreateOpenConnection())
            using (var command = CreateCommand(connection, null, sql, parameters))
            using (var adapter = new SqlDataAdapter(command))
            {
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        public static SqlCommand CreateCommand(
            SqlConnection connection,
            SqlTransaction transaction,
            string sql,
            Dictionary<string, object> parameters = null)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = sql;

            if (parameters == null)
            {
                return command;
            }

            foreach (var pair in parameters)
            {
                command.Parameters.AddWithValue(pair.Key, pair.Value ?? DBNull.Value);
            }

            return command;
        }
    }
}
