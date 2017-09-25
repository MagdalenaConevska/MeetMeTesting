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
    public class ConnectionNotificationRepositoryTests
    {
        DbSetMock<ConnectionNotification> _dbSet;
        Mock<MeetMeDbContext> _contextMock;
        Mock<ConnectionNotificationRepository> _connectionNotifRepoMock;

        List<ConnectionNotification> _connectionNotifs;

        [SetUp]
        public void Setup()
        {
            _dbSet = new DbSetMock<ConnectionNotification>();
            _contextMock = new Mock<MeetMeDbContext>();
            _connectionNotifRepoMock = new Mock<ConnectionNotificationRepository>();

            _connectionNotifs = new List<ConnectionNotification>();

            _connectionNotifRepoMock.Object._context = _contextMock.Object;
        }

        #region getConnectionNotifications

        [Test]
        public void getConnectionNotifications_LessCNFound()
        {
            _connectionNotifs.PopulateConnectionNotifications(5);

            _connectionNotifs.First().Date = DateTime.Now;

            _connectionNotifs.ForEach(f => _dbSet.Add(MockingHelper.CreateConnectionNotificationCopy(f)));

            _contextMock.Setup(f => f.ConnectionNotifications).Returns(_dbSet);

            var result = _connectionNotifRepoMock.Object.getConnectionNotifications(_connectionNotifs.First().User2.Id, 0, 2);

            _contextMock.Verify(f => f.ConnectionNotifications, Times.Once);

            MockingHelper.CheckAssertsForConnectionNotification(_connectionNotifs.First(), result.First());
            MockingHelper.CheckAssertsForConnectionNotification(_connectionNotifs.Skip(1).First(), result.Skip(1).First());

            Assert.AreEqual(result.Count, 2);
        }

        [Test]
        public void getConnectionNotifications_OrderingTest_AllCNFound()
        {
            _connectionNotifs.PopulateConnectionNotifications(5);

            _connectionNotifs.Skip(4).First().Date = DateTime.UtcNow.AddDays(4);
            _connectionNotifs.Skip(3).First().Date = DateTime.UtcNow.AddDays(3);
            _connectionNotifs.Skip(2).First().Date = DateTime.UtcNow.AddDays(2);
            _connectionNotifs.Skip(1).First().Date = DateTime.UtcNow.AddDays(1);

            _connectionNotifs.ForEach(f => _dbSet.Add(MockingHelper.CreateConnectionNotificationCopy(f)));

            _contextMock.Setup(f => f.ConnectionNotifications).Returns(_dbSet);

            var result = _connectionNotifRepoMock.Object.getConnectionNotifications(_connectionNotifs.First().User2.Id, 0, 6);

            _contextMock.Verify(f => f.ConnectionNotifications, Times.Once);

            for (int i = 0; i < result.Count(); i++)
            {
                MockingHelper.CheckAssertsForConnectionNotification(result.Skip(i).First(), _connectionNotifs.Skip(result.Count() - (i+1)).First());
            }
            Assert.AreEqual(result.Count, 5);
        }

        #endregion
    }
}