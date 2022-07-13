using MailContainerTest.Types;

namespace MailContainerTest.Abstractions.Validators;

public interface IMailsTransferValidator
{
    bool ValidateMailContainer(MakeMailTransferRequest request, MailContainer mailContainer);
}