using MailContainerTest.Types;

namespace MailContainerTest.Abstractions.Data;

public interface IBackupMailContainerDataStore
{
    MailContainer GetMailContainer(string mailContainerNumber);

    void UpdateMailContainer(MailContainer mailContainer);
}