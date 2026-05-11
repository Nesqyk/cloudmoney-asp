namespace CloudMoney.Logic
{
    public sealed class LoginResult : OperationResult
    {
        public int UserId { get; set; }

        public string AccountNumber { get; set; }

        public string FullName { get; set; }
    }
}
