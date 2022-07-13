namespace MailContainerTest.Types
{
    public class MailContainer
    {
        public int Capacity { get; set; }
        public MailContainerStatus Status { get; set; }
        public AllowedMailType AllowedMailType { get; set; }
    }
}