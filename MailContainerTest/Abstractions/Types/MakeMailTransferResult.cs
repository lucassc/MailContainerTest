namespace MailContainerTest.Types
{
    public class MakeMailTransferResult
    {
        public MakeMailTransferResult(bool success) =>
            Success = success;

        public bool Success { get; }
    }
}