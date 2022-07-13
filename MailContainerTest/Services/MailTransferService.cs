using MailContainerTest.Types;
using MailContainerTest.Abstractions.Data;
using MailContainerTest.Abstractions.Validators;
using Microsoft.Extensions.Configuration;

namespace MailContainerTest.Services
{
    public class MailTransferService : IMailTransferService
    {
        private readonly IBackupMailContainerDataStore _backupMailContainerDataStore;
        private readonly IMailContainerDataStore _mailContainerDataStore;
        private readonly IMailsTransferValidator _mailsTransferValidator;
        private readonly string _dataStoreType;


        public MailTransferService(
            IConfiguration configuration,
            IBackupMailContainerDataStore backupMailContainerDataStore,
            IMailContainerDataStore mailContainerDataStore,
            IMailsTransferValidator mailsTransferValidator)
        {
            _backupMailContainerDataStore = backupMailContainerDataStore;
            _mailContainerDataStore = mailContainerDataStore;
            _mailsTransferValidator = mailsTransferValidator;
            _dataStoreType = configuration.GetValue<string>("DataStoreType");
        }


        public MakeMailTransferResult MakeMailTransfer(MakeMailTransferRequest request)
        {
            var mailContainer = GetMailContainer(request);

            var success = _mailsTransferValidator.ValidateMailContainer(request, mailContainer);
            var result = new MakeMailTransferResult(success);

            if (!success) return result;

            mailContainer.Capacity -= request.NumberOfMailItems;
            UpdateMailContainer(mailContainer);

            return result;
        }

        private void UpdateMailContainer(MailContainer mailContainer)
        {
            if (_dataStoreType == "Backup")
            {
                _backupMailContainerDataStore.UpdateMailContainer(mailContainer);
                return;
            }

            _mailContainerDataStore.UpdateMailContainer(mailContainer);
        }

        private MailContainer GetMailContainer(MakeMailTransferRequest request) =>
            _dataStoreType.Equals("Backup")
                ? _backupMailContainerDataStore.GetMailContainer(request.SourceMailContainerNumber)
                : _mailContainerDataStore.GetMailContainer(request.SourceMailContainerNumber);
    }
}