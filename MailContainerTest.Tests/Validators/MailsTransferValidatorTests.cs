using AutoFixture;
using FluentAssertions;
using MailContainerTest.Services;
using MailContainerTest.Types;
using MailContainerTest.Validators;
using Xunit;

namespace MailContainerTest.Tests.Validators
{
    public class MailsTransferValidatorTests
    {
        private readonly MailContainer _mailContainer;
        private readonly MailsTransferValidator _validator;
        private readonly MakeMailTransferRequest _request;

        public MailsTransferValidatorTests()
        {
            var fixture = new Fixture();
            _mailContainer = fixture.Create<MailContainer>();
            _request = fixture.Create<MakeMailTransferRequest>();

            _validator = new MailsTransferValidator();
        }

        [Fact(DisplayName = "ValidateMailContainer when mail container is null should result false")]
        public void ValidateMailContainer_WhenMailContainerNotFound_ShouldResultFalse()
        {
            var result = _validator.ValidateMailContainer(null, null);

            result.Should().BeFalse();
        }

        [Theory(DisplayName =
            "ValidateMailContainer request when mail type is StandardLetter should validate correctly")]
        [InlineData(AllowedMailType.StandardLetter, true)]
        [InlineData(AllowedMailType.LargeLetter, false)]
        [InlineData(AllowedMailType.SmallParcel, true)]
        public void ValidateMailContainer_WhenRequestMailTypeIsStandardLetter_ShouldValidateCorrectly(
            AllowedMailType allowedMailType,
            bool expectedResult)
        {
            _request.MailType = MailType.StandardLetter;
            _mailContainer.AllowedMailType = allowedMailType;

            var result = _validator.ValidateMailContainer(_request, _mailContainer);

            result.Should().Be(expectedResult);
        }

        [Theory(DisplayName = "ValidateMailContainer when request mail type is LargeLetter should validate correctly")]
        [InlineData(AllowedMailType.StandardLetter, 9, 10, false)]
        [InlineData(AllowedMailType.StandardLetter, 10, 10, false)]
        [InlineData(AllowedMailType.StandardLetter, 11, 10, false)]
        [InlineData(AllowedMailType.LargeLetter, 9, 10, true)]
        [InlineData(AllowedMailType.LargeLetter, 10, 10, true)]
        [InlineData(AllowedMailType.LargeLetter, 11, 10, false)]
        [InlineData(AllowedMailType.SmallParcel, 9, 10, true)]
        [InlineData(AllowedMailType.SmallParcel, 10, 10, true)]
        [InlineData(AllowedMailType.SmallParcel, 11, 10, false)]
        public void ValidateMailContainer_WhenRequestMailTypeIsLargeLetter_ShouldValidateCorrectly(
            AllowedMailType allowedMailType,
            int numberOfMailItems,
            int capacity,
            bool expectedResult)
        {
            _request.MailType = MailType.LargeLetter;
            _mailContainer.AllowedMailType = allowedMailType;
            _mailContainer.Capacity = capacity;
            _request.NumberOfMailItems = numberOfMailItems;

            var result = _validator.ValidateMailContainer(_request, _mailContainer);

            result.Should().Be(expectedResult);
        }

        [Theory(DisplayName = "ValidateMailContainer when request mail type is SmallParcel should validate correctly")]
        [InlineData(AllowedMailType.StandardLetter, MailContainerStatus.Operational, false)]
        [InlineData(AllowedMailType.StandardLetter, MailContainerStatus.NoTransfersIn, false)]
        [InlineData(AllowedMailType.StandardLetter, MailContainerStatus.OutOfService, false)]
        [InlineData(AllowedMailType.LargeLetter, MailContainerStatus.Operational, false)]
        [InlineData(AllowedMailType.LargeLetter, MailContainerStatus.NoTransfersIn, false)]
        [InlineData(AllowedMailType.LargeLetter, MailContainerStatus.OutOfService, false)]
        [InlineData(AllowedMailType.SmallParcel, MailContainerStatus.Operational, true)]
        [InlineData(AllowedMailType.SmallParcel, MailContainerStatus.NoTransfersIn, false)]
        [InlineData(AllowedMailType.SmallParcel, MailContainerStatus.OutOfService, false)]
        public void ValidateMailContainer_WhenRequestMailTypeIsSmallParcel_ShouldValidateCorrectly(
            AllowedMailType allowedMailType,
            MailContainerStatus status,
            bool expectedResult)
        {
            _request.MailType = MailType.SmallParcel;
            _mailContainer.AllowedMailType = allowedMailType;
            _mailContainer.Status = status;

            var result = _validator.ValidateMailContainer(_request, _mailContainer);

            result.Should().Be(expectedResult);
        }
    }
}