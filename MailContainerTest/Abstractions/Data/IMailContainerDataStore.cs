using MailContainerTest.Types;

namespace MailContainerTest.Abstractions.Data;

public interface IMailContainerDataStore
{
    MailContainer GetMailContainer(string mailContainerNumber);
    void UpdateMailContainer(MailContainer mailContainer);
}