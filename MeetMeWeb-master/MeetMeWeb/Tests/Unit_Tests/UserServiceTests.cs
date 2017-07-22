using Moq;
using MeetMeWeb.Repositories.Interfaces;
using NUnit.Framework;
using MeetMeWeb.Models;
using MeetMeWeb.Services;
using System;

namespace MeetMeWeb.Tests.Unit_Tests
{
    [TestFixture]
    public class UserServiceTests
    { 
        Mock<UserService> _userServiceMock;
            
        Mock<IUserRepository> _userRepositoryMock;
        Mock<IConnectionNotificationRepository> _connectionNotificationRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _connectionNotificationRepositoryMock = new Mock<IConnectionNotificationRepository>();

            _userServiceMock = new Mock<UserService>(new object[] { _userRepositoryMock.Object, _connectionNotificationRepositoryMock.Object })
            {
                CallBase = true
            };
        }

        #region getUserById
        //This is the first unit test for this project, so we'll leave some comments for presentation purposes
        [Test]
        public void getUserById_UserFound()
        {
            User mockedUser = new User {
                Id = "TestId",
                FirstName = "Magdalena",
                LastName = "Conevska",
                Email = "magde@gmail.com",
                BirthDate = DateTime.UtcNow
            };
            //We use AAA syntax in each unit test in this project

            //Arrange
            _userRepositoryMock.Setup(f => f.getUserById(It.IsAny<string>())).Returns(mockedUser);

            //Act
            var result = _userServiceMock.Object.getUserById("TestId");

            //Assert
            _userRepositoryMock.Verify(f => f.getUserById(It.Is<string>(v => v == mockedUser.Id)), Times.Once);

            Assert.AreEqual(mockedUser.Id, result.Id);
            Assert.AreEqual(mockedUser.FirstName, result.FirstName);
            Assert.AreEqual(mockedUser.LastName, result.LastName);
            Assert.AreEqual(mockedUser.Email, result.Email);
            Assert.AreEqual(mockedUser.BirthDate, result.BirthDate);
        }
        #endregion
    }
}