using MailContainerTest.Abstractions.Validators;
using MailContainerTest.Types;

namespace MailContainerTest.Validators
{
    public class MailsTransferValidator : IMailsTransferValidator
    {
        public bool ValidateMailContainer(MakeMailTransferRequest request, MailContainer mailContainer)
        {
            if (mailContainer is null)
                return false;

            return request.MailType switch
            {
                MailType.StandardLetter =>
                    mailContainer.AllowedMailType.HasFlag(AllowedMailType.StandardLetter),
                MailType.LargeLetter =>
                    mailContainer.AllowedMailType.HasFlag(AllowedMailType.LargeLetter) &&
                    mailContainer.Capacity >= request.NumberOfMailItems,
                MailType.SmallParcel =>
                    mailContainer.AllowedMailType.HasFlag(AllowedMailType.SmallParcel) &&
                    mailContainer.Status == MailContainerStatus.Operational,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}