using AutoFixture;
using FluentAssertions;
using MailContainerTest.Abstractions.Data;
using MailContainerTest.Abstractions.Validators;
using MailContainerTest.Services;
using MailContainerTest.Types;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace MailContainerTest.Tests.Services;

public class MailTransferServiceTests
{
    public MailTransferServiceTests()
    {
        _fixture = new Fixture();
        _configuration = new Mock<IConfiguration>(MockBehavior.Strict);
        _configurationSection = new Mock<IConfigurationSection>();
        _backupMailContainerDataStore = new Mock<IBackupMailContainerDataStore>(MockBehavior.Strict);
        _mailContainerDataStore = new Mock<IMailContainerDataStore>(MockBehavior.Strict);
        _mailsTransferValidator = new Mock<IMailsTransferValidator>(MockBehavior.Strict);
        _mailContainer = _fixture.Create<MailContainer>();
        var dataStoreType = _fixture.Create<string>();
        SetConfigurationMock(dataStoreType);

        var _service = new MailTransferService(
            _configuration.Object,
            _backupMailContainerDataStore.Object,
            _mailContainerDataStore.Object,
            _mailsTransferValidator.Object);
    }

    private readonly Mock<IBackupMailContainerDataStore> _backupMailContainerDataStore;
    private readonly Mock<IConfiguration> _configuration;
    private readonly Mock<IMailContainerDataStore> _mailContainerDataStore;
    private readonly Mock<IMailsTransferValidator> _mailsTransferValidator;
    private readonly Fixture _fixture;
    private readonly Mock<IConfigurationSection> _configurationSection;
    private readonly MailContainer _mailContainer;

    [Fact(DisplayName = "MakeMailTransfer when dataStoreType is Backup should get email from backup")]
    public void MakeMailTransfer_WhenDataStoreTypeIsBackup_ShouldGetEmailFromBackup()
    {
        var request = _fixture.Create<MakeMailTransferRequest>();
        SetConfigurationMock(MailTransferService.DATA_STORE_TYPE_BACKUP_VALUE);
        _backupMailContainerDataStore
            .Setup(s => s.GetMailContainer(request.SourceMailContainerNumber))
            .Returns(_mailContainer);
        _mailsTransferValidator
            .Setup(s => s.ValidateMailContainer(request, _mailContainer))
            .Returns(false);
        var service = new MailTransferService(
            _configuration.Object,
            _backupMailContainerDataStore.Object,
            _mailContainerDataStore.Object,
            _mailsTransferValidator.Object);

        var result = service.MakeMailTransfer(request);

        result.Success.Should().BeFalse();
        _backupMailContainerDataStore
            .VerifyAll();
    }

    [Fact(DisplayName = "MakeMailTransfer when dataStoreType isn't Backup should get email from data store")]
    public void MakeMailTransfer_WhenDataStoreTypeIsNotBackup_ShouldGetEmailFromDataStore()
    {
        var request = _fixture.Create<MakeMailTransferRequest>();
        SetConfigurationMock("AnyThing");
        _mailContainerDataStore
            .Setup(s => s.GetMailContainer(request.SourceMailContainerNumber))
            .Returns(_mailContainer);
        _mailsTransferValidator
            .Setup(s => s.ValidateMailContainer(request, _mailContainer))
            .Returns(false);
        var service = new MailTransferService(
            _configuration.Object,
            _backupMailContainerDataStore.Object,
            _mailContainerDataStore.Object,
            _mailsTransferValidator.Object);

        var result = service.MakeMailTransfer(request);

        result.Success.Should().BeFalse();
        _mailContainerDataStore
            .VerifyAll();
    }

    [Fact(DisplayName = "MakeMailTransfer when dataStoreType is Backup should save email to backup with the correct Value")]
    public void MakeMailTransfer_WhenDataStoreTypeIsBackup_ShouldSaveEmailToBackupWithTheCorrectValue()
    {
        var request = _fixture.Create<MakeMailTransferRequest>();
        var capacityBefore = _mailContainer.Capacity;
        var expectedCapacity = capacityBefore - request.NumberOfMailItems;
        SetConfigurationMock(MailTransferService.DATA_STORE_TYPE_BACKUP_VALUE);
        _backupMailContainerDataStore
            .Setup(s => s.GetMailContainer(request.SourceMailContainerNumber))
            .Returns(_mailContainer);
        _backupMailContainerDataStore
            .Setup(s => s.UpdateMailContainer(_mailContainer));
        _mailsTransferValidator
            .Setup(s => s.ValidateMailContainer(request, _mailContainer))
            .Returns(true);
        var service = new MailTransferService(
            _configuration.Object,
            _backupMailContainerDataStore.Object,
            _mailContainerDataStore.Object,
            _mailsTransferValidator.Object);

        var result = service.MakeMailTransfer(request);

        result.Success.Should().BeTrue();
        _mailContainer.Capacity.Should().Be(expectedCapacity);
        _backupMailContainerDataStore
            .VerifyAll();
    }


    [Fact(DisplayName = "MakeMailTransfer when dataStoreType isn't Backup should save email to data store with the correct Value")]
    public void MakeMailTransfer_WhenDataStoreTypeIsNotBackup_ShouldSaveEmailToDataStoreWithTheCorrectValue()
    {
        var request = _fixture.Create<MakeMailTransferRequest>();
        var capacityBefore = _mailContainer.Capacity;
        var expectedCapacity = capacityBefore - request.NumberOfMailItems;
        SetConfigurationMock("AnyThing");
        _mailContainerDataStore
            .Setup(s => s.GetMailContainer(request.SourceMailContainerNumber))
            .Returns(_mailContainer);
        _mailContainerDataStore
            .Setup(s => s.UpdateMailContainer(_mailContainer));
        _mailsTransferValidator
            .Setup(s => s.ValidateMailContainer(request, _mailContainer))
            .Returns(true);
        var service = new MailTransferService(
            _configuration.Object,
            _backupMailContainerDataStore.Object,
            _mailContainerDataStore.Object,
            _mailsTransferValidator.Object);

        var result = service.MakeMailTransfer(request);

        result.Success.Should().BeTrue();
        _mailContainer.Capacity.Should().Be(expectedCapacity);
        _mailContainerDataStore
            .VerifyAll();
    }

    private void SetConfigurationMock(string dataStoreType)
    {
        _configuration
            .Setup(s => s.GetSection(MailTransferService.DATA_STORE_TYPE_KEY))
            .Returns(_configurationSection.Object);
        _configurationSection
            .Setup(s => s.Value)
            .Returns(dataStoreType);
    }
}