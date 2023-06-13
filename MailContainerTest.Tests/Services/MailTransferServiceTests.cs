using MailContainerTest.Data;
using MailContainerTest.Services;
using MailContainerTest.Types;
using Moq;
using NUnit.Framework;
using System.Configuration;
using MailContainerTest.Tests.Helper;
using FluentAssertions;

namespace MailContainerTest.Tests.Services
{
    [TestFixture]
    public class MailTransferServiceTests
    {
        private Mock<MakeMailTransferRequest> _mockMakeMailTransferRequest;

        private MailTransferService _sut;

        [SetUp]
        public void SetUp()
        {
            _mockMakeMailTransferRequest = new();

            _sut = new MailTransferService();
        }

        [Test]
        public void MakeMailTransferInvoked_CalledCorrectly()
        {
            //Arrange
            ConfigurationManager.AppSettings["DataStoreType"] = "Backup";

            //Act
            var result = _sut.MakeMailTransfer(_mockMakeMailTransferRequest.Object);

            //Assert
            result.Success.Should().BeFalse();
        }

        [TestCase("Backup")]
        [TestCase("New One")]
        public void GetMailContainerFromDataStoreType_DataStoreTypeHasASpecificValue_ReturnsNotNull(string dataStoreType)
        {
            //Arrange
            object[] parameters = { new MakeMailTransferRequest(), dataStoreType };

            //Act
            var result = CallMethod.CallPrivateMethod<MailContainer>(_sut, "GetMailContainerFromDataStoreType", parameters);

            //Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void IsTheContainerInValidState_MailContainerIsNull_ReturnsFalse()
        {
            //Arrange
            object[] parameters = { new MakeMailTransferRequest(), null };

            //Act
            var result = CallMethod.CallPrivateMethod<bool>(_sut, "IsTheContainerInValidState", parameters);

            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsTheContainerInValidState_StandardMailTypeIsProvidedWithoutAllowedMailType_ReturnsFalse()
        {
            //Arrange
            object[] parameters = { 
                new MakeMailTransferRequest() 
                {
                    MailType = MailType.StandardLetter 
                }, 
                new MailContainer()
            };

            //Act
            var result = CallMethod.CallPrivateMethod<bool>(_sut, "IsTheContainerInValidState", parameters);

            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsTheContainerInValidState_StandardMailTypeIsProvidedWithAllowedMailType_ReturnsTrue()
        {
            //Arrange
            object[] parameters = {
                new MakeMailTransferRequest()
                {
                    MailType = MailType.StandardLetter
                },
                new MailContainer()
                {
                    AllowedMailType = (AllowedMailType)Enum.Parse(typeof(AllowedMailType), "StandardLetter")
                }
            };

            //Act
            var result = CallMethod.CallPrivateMethod<bool>(_sut, "IsTheContainerInValidState", parameters);

            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsTheContainerInValidState_LargeMailTypeIsProvidedWithoutAllowedMailType_ReturnsFalse()
        {
            //Arrange
            object[] parameters = {
                new MakeMailTransferRequest()
                {
                    MailType = MailType.LargeLetter
                },
                new MailContainer()
            };

            //Act
            var result = CallMethod.CallPrivateMethod<bool>(_sut, "IsTheContainerInValidState", parameters);

            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsTheContainerInValidState_LargeMailTypeIsProvidedWillLessContainerCapacity_ReturnsFalse()
        {
            //Arrange
            object[] parameters = {
                new MakeMailTransferRequest()
                {
                    MailType = MailType.LargeLetter,
                    NumberOfMailItems = 5
                },
                new MailContainer()
                {
                    AllowedMailType = (AllowedMailType)Enum.Parse(typeof(AllowedMailType), "LargeLetter")
                }
            };

            //Act
            var result = CallMethod.CallPrivateMethod<bool>(_sut, "IsTheContainerInValidState", parameters);

            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsTheContainerInValidState_LargeMailTypeIsProvidedWillMoreContainerCapacity_ReturnsTrue()
        {
            //Arrange
            object[] parameters = {
                new MakeMailTransferRequest()
                {
                    MailType = MailType.LargeLetter
                },
                new MailContainer()
                {
                    AllowedMailType = (AllowedMailType)Enum.Parse(typeof(AllowedMailType), "LargeLetter"),
                    Capacity = 10
                }
            };

            //Act
            var result = CallMethod.CallPrivateMethod<bool>(_sut, "IsTheContainerInValidState", parameters);

            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsTheContainerInValidState_SmallMailTypeIsProvidedWithoutAllowedMailType_ReturnsFalse()
        {
            //Arrange
            object[] parameters = {
                new MakeMailTransferRequest()
                {
                    MailType = MailType.SmallParcel
                },
                new MailContainer()
            };

            //Act
            var result = CallMethod.CallPrivateMethod<bool>(_sut, "IsTheContainerInValidState", parameters);

            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsTheContainerInValidState_SmallMailTypeIsProvidedWithContainerStatusAsOutOfService_ReturnsFalse()
        {
            //Arrange
            object[] parameters = {
                new MakeMailTransferRequest()
                {
                    MailType = MailType.SmallParcel
                },
                new MailContainer()
                {
                    AllowedMailType = (AllowedMailType)Enum.Parse(typeof(AllowedMailType), "SmallParcel"),
                    Status = MailContainerStatus.OutOfService
                }
            };

            //Act
            var result = CallMethod.CallPrivateMethod<bool>(_sut, "IsTheContainerInValidState", parameters);

            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsTheContainerInValidState_SmallMailTypeIsProvidedWithContainerStatusAsOperational_ReturnsTrue()
        {
            //Arrange
            object[] parameters = {
                new MakeMailTransferRequest()
                {
                    MailType = MailType.SmallParcel
                },
                new MailContainer()
                {
                    AllowedMailType = (AllowedMailType)Enum.Parse(typeof(AllowedMailType), "SmallParcel"),
                    Status = MailContainerStatus.Operational
                }
            };

            //Act
            var result = CallMethod.CallPrivateMethod<bool>(_sut, "IsTheContainerInValidState", parameters);

            //Assert
            result.Should().BeTrue();
        }

        [TestCase("Backup", 0)]
        [TestCase("New One", 0)]
        public void UpdateContainerCapacity_WhenDataStoreTypeHasAValue_ReturnsNotNull(string dataStoreType, int expectedResult)
        {
            //Arrange
            object[] parameters = { new MakeMailTransferRequest(), new MailContainer(), dataStoreType };

            //Act
            var result = CallMethod.CallPrivateMethod<int>(_sut, "UpdateContainerCapacity", parameters);

            //Assert
            result.Should().Be(expectedResult);
        }
    }
}