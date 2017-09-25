using MeetMeWeb.Models;
using MeetMeWeb.Repositories;
using MeetMeWeb.Tests.Mocks;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeetMeWeb.Tests.Unit_Tests.RepositoriesTests
{
    [TestFixture]
    public class ConnectionRepositoryTests
    {
        DbSetMock<Connection> _dbSet;
        Mock<MeetMeDbContext> _contextMock;
        Mock<ConnectionRepository> _connectionRepoMock;

        List<Connection> _connections;

        [SetUp]
        public void Setup()
        {
            _dbSet = new DbSetMock<Connection>();
            _contextMock = new Mock<MeetMeDbContext>();
            _connectionRepoMock = new Mock<ConnectionRepository>();

            _connections = new List<Connection>();

            _connectionRepoMock.Object._context = _contextMock.Object;
        }

        #region GetConnection
        [Test]
        public void GetConnection_AsListedParams_ConnectionFound()
        {
            _connections.PopulateConnections(3);
            _connections.ForEach(f => _dbSet.Add(MockingHelper.CreateConnectionCopy(f)));

            _contextMock.Setup(f => f.Connections).Returns(_dbSet);

            var result = _connectionRepoMock.Object.GetConnection(_connections.First().User1.Id, _connections.First().User2.Id);

            _contextMock.Verify(f => f.Connections, Times.Once);
            MockingHelper.CheckAssertsForConnection(_connections.First(), result);
        }

        [Test]
        public void GetConnection_ReverseListedParams_ConnectionFound()
        {
            _connections.PopulateConnections(1);
            _connections.ForEach(f => _dbSet.Add(MockingHelper.CreateConnectionCopy(f)));

            _contextMock.Setup(f => f.Connections).Returns(_dbSet);

            var result = _connectionRepoMock.Object.GetConnection(_connections.First().User2.Id, _connections.First().User1.Id);

            _contextMock.Verify(f => f.Connections, Times.Exactly(2));
            MockingHelper.CheckAssertsForConnection(_connections.First(), result);
        }

        [Test]
        public void GetConnection_ConnectionNotFound()
        {
            _connections.PopulateConnections(1);
            _connections.ForEach(f => _dbSet.Add(MockingHelper.CreateConnectionCopy(f)));

            _contextMock.Setup(f => f.Connections).Returns(_dbSet);

            var result = _connectionRepoMock.Object.GetConnection(Guid.NewGuid().ToString(), _connections.First().User1.Id);

            _contextMock.Verify(f => f.Connections, Times.Exactly(2));

            Assert.IsNull(result);
        }
        #endregion

        #region getFriends()

        [Test]
        public void getFriends_FirstHalfOnly_FriendsFound()
        {
            _connections.PopulateConnections(3);

            _connections.First().User2.UserName = "First-Second";
            _connections.Skip(2).First().User1 = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = Guid.NewGuid().ToString()
            };
            _connections.ForEach(f => _dbSet.Add(MockingHelper.CreateConnectionCopy(f)));

            _contextMock.Setup(f => f.Connections).Returns(_dbSet);

            var result = _connectionRepoMock.Object.getFriends(_connections.First().User1.UserName);

            _contextMock.Verify(f => f.Connections, Times.Exactly(2));

            MockingHelper.CheckAssertsForUser(_connections.First().User2, result.First());
            MockingHelper.CheckAssertsForUser(_connections.Skip(1).First().User2, result.Skip(1).First());
            Assert.AreEqual(result.Count, 2);

        }

        [Test]
        public void getFriends_SecondHalfOnly_FriendsFound()
        {
            _connections.PopulateConnections(3);

            _connections.First().User1.UserName = "First-Second";
            _connections.Skip(2).First().User2 = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = Guid.NewGuid().ToString()
            };
            _connections.ForEach(f => _dbSet.Add(MockingHelper.CreateConnectionCopy(f)));

            _contextMock.Setup(f => f.Connections).Returns(_dbSet);

            var result = _connectionRepoMock.Object.getFriends(_connections.First().User2.UserName);

            _contextMock.Verify(f => f.Connections, Times.Exactly(2));

            MockingHelper.CheckAssertsForUser(_connections.First().User1, result.First());
            MockingHelper.CheckAssertsForUser(_connections.Skip(1).First().User1, result.Skip(1).First());
            Assert.AreEqual(result.Count, 2);

        }

        [Test]
        public void getFriends_BothFor_FriendsFound()
        {
            _connections.PopulateConnections(3);
            List<User> users = new List<User>();
            users.PopulateUsers(4);
            _connections.Skip(1).First().User2.UserName = "First-Second";
            _connections.Skip(2).First().User2 = _connections.First().User1;
            _connections.Skip(2).First().User1 = users.Skip(3).First();

            _connections.ForEach(f => _dbSet.Add(MockingHelper.CreateConnectionCopy(f)));

            _contextMock.Setup(f => f.Connections).Returns(_dbSet);

            var result = _connectionRepoMock.Object.getFriends(_connections.First().User1.UserName);

            _contextMock.Verify(f => f.Connections, Times.Exactly(2));

            MockingHelper.CheckAssertsForUser(_connections.First().User2, result.First());
            MockingHelper.CheckAssertsForUser(_connections.Skip(1).First().User2, result.Skip(1).First());
            MockingHelper.CheckAssertsForUser(_connections.Skip(2).First().User1, result.Skip(2).First());
            Assert.AreEqual(result.Count, 3);

        }

        [Test]
        public void getFriends_FriendsNotFound()
        {
            _connections.PopulateConnections(3);

            _connections.ForEach(f => _dbSet.Add(MockingHelper.CreateConnectionCopy(f)));

            _contextMock.Setup(f => f.Connections).Returns(_dbSet);

            var result = _connectionRepoMock.Object.getFriends(Guid.NewGuid().ToString());

            _contextMock.Verify(f => f.Connections, Times.Exactly(2));

            Assert.AreEqual(result.Count, 0);

        }

        #endregion
    }
}