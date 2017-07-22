using Moq;
using MeetMeWeb.Repositories.Interfaces;
using NUnit.Framework;
using MeetMeWeb.Models;
using MeetMeWeb.Services;
using System.Linq;
using System.Collections.Generic;
using System;

namespace MeetMeWeb.Tests.Unit_Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        Mock<UserService> _userServiceMock;

        Mock<IUserRepository> _userRepositoryMock;
        Mock<IConnectionNotificationRepository> _connectionNotificationRepositoryMock;

        List<User> _users;
        List<ConnectionNotification> _connectionNotifications;

        List<User> expectedUsers;
        List<ConnectionNotification> expectedConnectionNotifications;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _connectionNotificationRepositoryMock = new Mock<IConnectionNotificationRepository>();

            _users = new List<User>();
            _connectionNotifications = new List<ConnectionNotification>();

            expectedUsers = new List<User>();
            expectedConnectionNotifications = new List<ConnectionNotification>();

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
            User mockedUser = _users.PopulateUsers(1).FirstOrDefault();
            
            User expected = MockingHelper.CreateUserCopy(mockedUser);

            //We use AAA syntax in each unit test in this project

            //Arrange
            _userRepositoryMock.Setup(f => f.getUserById(It.IsAny<string>())).Returns(mockedUser);

            //Act
            var result = _userServiceMock.Object.getUserById(mockedUser.Id);

            //Assert
            _userRepositoryMock.Verify(f => f.getUserById(It.Is<string>(v => v == mockedUser.Id)), Times.Once);

            MockingHelper.CheckAssertsForUser(expected, result);
        }
        #endregion

        #region getUserByUsername

        [Test]
        public void getUserByUsername_UserFound()
        {
            User mockedUser = _users.PopulateUsers(1).FirstOrDefault();

            User expected = MockingHelper.CreateUserCopy(mockedUser);

            _userRepositoryMock.Setup(f => f.getUserByUsername(It.IsAny<string>())).Returns(mockedUser);

            var result = _userServiceMock.Object.getUserByUsername(mockedUser.UserName);

            _userRepositoryMock.Verify(f => f.getUserByUsername(It.Is<string>(v => v == mockedUser.UserName)), Times.Once);

            MockingHelper.CheckAssertsForUser(expected, result);
        }
        #endregion

        #region getAll

        [Test]
        public void getAll_UsersFound()
        {
            _users.PopulateUsers(5);

            foreach (User user in _users)
            {
                expectedUsers.Add(MockingHelper.CreateUserCopy(user));
            }

            _userRepositoryMock.Setup(f => f.getAll()).Returns(_users);

            var result = _userServiceMock.Object.getAll();

            _userRepositoryMock.Verify(f => f.getAll(), Times.Once);

            Assert.AreEqual(expectedUsers.Count, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                MockingHelper.CheckAssertsForUser(expectedUsers.Skip(i).First(), result.Skip(i).First());
            }
        }
        #endregion

        #region getConnectionNotifications

        [Test]
        public void getConnectionNotifications_ResultListNotEmpty()
        {
            _connectionNotifications.PopulateConnectionNotifications(2);

            foreach (ConnectionNotification connectionNotification in _connectionNotifications)
            {
                expectedConnectionNotifications.Add(MockingHelper.CreateConnectionNotificationCopy(connectionNotification));
            }

            _connectionNotificationRepositoryMock.Setup(f => f.getConnectionNotifications(It.IsAny<string>(),
                                                                                          It.IsAny<int>(),
                                                                                          It.IsAny<int>())).Returns(_connectionNotifications);

            _connectionNotificationRepositoryMock.Setup(f => f.readConnestionNotification(It.IsAny<ConnectionNotification>()));

            var result = _userServiceMock.Object.getConnectionNotifications(_connectionNotifications.First().User2.Id, 0, _connectionNotifications.Count);

            _connectionNotificationRepositoryMock.Verify(f => f.getConnectionNotifications(It.Is<string>(k => k == _connectionNotifications.First().User2.Id),
                                                                                           It.Is<int>(k => k == 0),
                                                                                           It.Is<int>(k => k == _connectionNotifications.Count)), Times.Once);

            for (int i = 0; i < _connectionNotifications.Count; i++)
            {
                _connectionNotificationRepositoryMock.Verify(f => f.readConnestionNotification(It.Is<ConnectionNotification>(k => k.User1.Id == _connectionNotifications.Skip(i).First().User1.Id &&
                                                                                                                                  k.User2.Id == _connectionNotifications.Skip(i).First().User2.Id &&
                                                                                                                                  k.ID == _connectionNotifications.Skip(i).First().ID &&
                                                                                                                                  k.Content == _connectionNotifications.Skip(i).First().Content &&
                                                                                                                                  k.Date == _connectionNotifications.Skip(i).First().Date
                                                                                                                                )), Times.Once());
            }

            Assert.AreEqual(expectedConnectionNotifications.Count, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                MockingHelper.CheckAssertsForConnectionNotification(expectedConnectionNotifications.Skip(i).First(), result.Skip(i).First());
            }
        }

        [Test]
        public void getConnectionNotifications_ResultListIsEmpty()
        {
            _connectionNotificationRepositoryMock.Setup(f => f.getConnectionNotifications(It.IsAny<string>(),
                                                                                         It.IsAny<int>(),
                                                                                         It.IsAny<int>())).Returns(_connectionNotifications);

            _connectionNotificationRepositoryMock.Setup(f => f.readConnestionNotification(It.IsAny<ConnectionNotification>()));

            var result = _userServiceMock.Object.getConnectionNotifications("User2TestId", 0, 2);

            _connectionNotificationRepositoryMock.Verify(f => f.getConnectionNotifications(It.Is<string>(k => k == "User2TestId"),
                                                                                           It.Is<int>(k => k == 0),
                                                                                           It.Is<int>(k => k == 2)), Times.Once);

            _connectionNotificationRepositoryMock.Verify(f => f.readConnestionNotification(It.IsAny<ConnectionNotification>()), Times.Never);

            Assert.AreEqual(expectedConnectionNotifications.Count, result.Count);
        }

            #endregion
        }
}