using MeetMeWeb.Models;
using MeetMeWeb.Repositories.Interfaces;
using MeetMeWeb.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetMeWeb.Tests.Unit_Tests
{
    [TestFixture]
    public class ConnectionServiceTests
    {
        Mock<IConnectionRepository> _connectionRepoMock;
        Mock<IUserRepository> _usersRepoMock;
        Mock<IConnectionNotificationRepository> _connectionNotificationRepoMock;

        Mock<ConnectionService> _connectionServiceMock;

        List<Connection> _connections;

        [SetUp]
        public void Setup()
        {
            _connectionRepoMock = new Mock<IConnectionRepository>();
            _usersRepoMock = new Mock<IUserRepository>();
            _connectionNotificationRepoMock = new Mock<IConnectionNotificationRepository>();

            _connections = new List<Connection>();

            _connectionServiceMock = new Mock<ConnectionService>(new object[] { _connectionRepoMock.Object, _usersRepoMock.Object, _connectionNotificationRepoMock.Object })
            {
                CallBase = true
            };
        }

        #region GetConnection

        [Test]
        public void GetConnection_ConnectionFound()
        {
            _connections.PopulateConnections(1);
            Connection connection = _connections.First();

            Connection mockedConnection = MockingHelper.CreateConnectionCopy(connection);

            _connectionRepoMock.Setup(f => f.GetConnection(It.IsAny<string>(), It.IsAny<string>())).Returns(connection);

            var result = _connectionServiceMock.Object.GetConnection(connection.User1.Id, connection.User2.Id);

            _connectionRepoMock.Verify(f => f.GetConnection(It.Is<string>(k => k == connection.User1.Id),
                                                            It.Is<string>(k => k == connection.User2.Id)), Times.Once);

            MockingHelper.CheckAssertsForConnection(mockedConnection, result);
        }

        #endregion

        #region CreateConnection 

        [Test] //TODO-Check blackbox when CreateConnection invoked with those parameters
        public async Task CreateConnection_ConnectionCreatedAsync()
        {
            _connections.PopulateConnections(1);
            Connection connection = _connections.First();

            Connection mockedConnection = MockingHelper.CreateConnectionCopy(connection);

            _connectionRepoMock.Setup(f => f.CreateConnection(It.IsAny<Connection>())).ReturnsAsync(connection);
            _connectionNotificationRepoMock.Setup(f => f.createNotification(It.IsAny<ConnectionNotification>()));

            var result = await _connectionServiceMock.Object.CreateConnection(connection);

            _connectionRepoMock.Verify(f => f.CreateConnection(It.Is<Connection>(k => k.ID == connection.ID &&
                                                                                      k.StartDate == connection.StartDate &&
                                                                                      k.Status == connection.Status &&
                                                                                      k.User1.Id == connection.User1.Id &&
                                                                                      k.User2.Id == connection.User2.Id)), Times.Once);
            _connectionNotificationRepoMock.Verify(f => f.createNotification(It.Is<ConnectionNotification>(k => k.User1.Id == connection.User1.Id &&
                                                                                                                k.User2.Id == connection.User2.Id &&
                                                                                                                k.Read == false &&
                                                                                                                k.Content == "send you request for connection" &&
                                                                                                                k.Date.ToShortDateString() == DateTime.Now.ToShortDateString())), Times.Once);
            MockingHelper.CheckAssertsForConnection(mockedConnection, result);
        }

        #endregion

        #region DeleteConnection

        [Test]
        public void DeleteConnection_ConnectionDeleted()
        {
            _connections.PopulateConnections(1);
            Connection connection = _connections.First();

            _connectionRepoMock.Setup(f => f.DeleteConnection(It.IsAny<Connection>()));

            _connectionServiceMock.Object.DeleteConnection(connection);

            _connectionRepoMock.Verify(f => f.DeleteConnection(It.Is<Connection>(k => k.ID == connection.ID &&
                                                                                      k.StartDate == connection.StartDate &&
                                                                                      k.Status == connection.Status &&
                                                                                      k.User1.Id == connection.User1.Id &&
                                                                                      k.User2.Id == connection.User2.Id)), Times.Once);
        }

        #endregion

        #region AcceptConnection

        [Test]
        public async Task AcceptConnection_ConnectionAcceptedAsync()
        {
            _connections.PopulateConnections(1);
            Connection connection = _connections.First();

            Connection mockedConnection = MockingHelper.CreateConnectionCopy(connection);

            _connectionRepoMock.Setup(f => f.AcceptConnection(It.IsAny<Connection>())).ReturnsAsync(connection);

            _connectionNotificationRepoMock.Setup(f => f.createNotification(It.IsAny<ConnectionNotification>()));

            var result = await _connectionServiceMock.Object.AcceptConnection(connection);

            _connectionRepoMock.Verify(f => f.AcceptConnection(It.Is<Connection>(k => k.ID == connection.ID &&
                                                                                      k.StartDate == connection.StartDate &&
                                                                                      k.Status == connection.Status &&
                                                                                      k.User1.Id == connection.User1.Id &&
                                                                                      k.User2.Id == connection.User2.Id)), Times.Once);
            _connectionNotificationRepoMock.Verify(f => f.createNotification(It.Is<ConnectionNotification>(k => k.User1.Id == connection.User2.Id &&
                                                                                                                k.User2.Id == connection.User1.Id &&
                                                                                                                k.Read == false &&
                                                                                                                k.Content == "accepted your request for connection" &&
                                                                                                                k.Date.ToShortDateString() == DateTime.Now.ToShortDateString())), Times.Once);
            MockingHelper.CheckAssertsForConnection(mockedConnection, result);
        }

        #endregion

        #region getFriends

        [Test]
        public void getFriends_FriendsFound()
        {
            List<User> users = new List<User>();
            users.PopulateUsers(3);

            List<User> mockedUsers = new List<User>();

            foreach (User user in users)
            {
                mockedUsers.Add(MockingHelper.CreateUserCopy(user));
            }

            _connectionRepoMock.Setup(f => f.getFriends(It.IsAny<string>())).Returns(users);

            var result = _connectionServiceMock.Object.getFriends(users.First().UserName);

            _connectionRepoMock.Verify(f => f.getFriends(It.Is<string>(k => k == users.First().UserName)), Times.Once);

            Assert.AreEqual(mockedUsers.Count, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                MockingHelper.CheckAssertsForUser(mockedUsers.Skip(i).First(), result.Skip(i).First());
            }
        }

        #endregion

    }
}