using MeetMeWeb.Models;
using MeetMeWeb.Repositories;
using MeetMeWeb.Tests.Mocks;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MeetMeWeb.Tests.Unit_Tests.RepositoriesTests
{
    [TestFixture]
    public class MeetingRepositoryTests
    {
        DbSetMock<Meeting> _dbSet;
        Mock<MeetMeDbContext> _contextMock;
        Mock<MeetingRepository> _meetingRepoMock;

        List<Meeting> _meetings;

        [SetUp]
        public void Setup()
        {
            _dbSet = new DbSetMock<Meeting>();
            _contextMock = new Mock<MeetMeDbContext>();
            _meetingRepoMock = new Mock<MeetingRepository>();

            _meetings = new List<Meeting>();

            _meetingRepoMock.Object._context = _contextMock.Object;
        }

        #region getByTitle
        [Test]
        public void getByTitle_MeetingFound()
        {
            _meetings.PopulateMeetings(1);
            _meetings.ForEach(f => _dbSet.Add(MockingHelper.CreateMeetingCopy(f)));

            _contextMock.Setup(f => f.Meetings).Returns(_dbSet);

            var result = _meetingRepoMock.Object.getByTitle(_meetings.First().Title);

            _contextMock.Verify(f => f.Meetings, Times.Once);

            MockingHelper.CheckAssertsForMeeting(_meetings.First(), result);
        }

        // if a meeting with such title does not exists this method should return null, not throw an exception
        // more because it is not handled anywhere in code
        //NullNotHandled in corresponding service
        [Test]
        public void getByTitle_MeetingNotFound()
        {
            _meetings.PopulateMeetings(1);
            _meetings.ForEach(f => _dbSet.Add(MockingHelper.CreateMeetingCopy(f)));

            _contextMock.Setup(f => f.Meetings).Returns(_dbSet);

            var result = _meetingRepoMock.Object.getByTitle(Guid.NewGuid().ToString());

            _contextMock.Verify(f => f.Meetings, Times.Once);

            Assert.IsNull(result);
        }

        [Test]
        public void getByTitle_TitleNotUnique_ThrowsInvalidOperationException()
        {
            _meetings.PopulateMeetings(3);
            _meetings.Skip(1).First().Title = _meetings.First().Title;
            _meetings.ForEach(f => _dbSet.Add(MockingHelper.CreateMeetingCopy(f)));

            _contextMock.Setup(f => f.Meetings).Returns(_dbSet);

            Assert.Throws<InvalidOperationException>(() =>
            {

                _meetingRepoMock.Object.getByTitle(_meetings.First().Title);
            });

            _contextMock.Verify(f => f.Meetings, Times.Exactly(1));
        }
        #endregion

        #region getById

        [Test]
        public void getById_MeetingRequestsForUserFound()
        {
            DbSetMock<MeetingRequest> _dbRequestSet = new DbSetMock<MeetingRequest>();
            List<MeetingRequest> _meetingRequests = new List<MeetingRequest>();
            _meetingRequests.PopulateMeetingRequests(2);
            _meetingRequests.Skip(1).First().User.Id = _meetingRequests.First().User.Id;
            _meetingRequests.Skip(1).First().Status = _meetingRequests.First().Status = false;

            _meetingRequests.ForEach(f => _dbRequestSet.Add(MockingHelper.CreateMeetingRequestCopy(f)));

            _contextMock.Setup(f => f.MeetingRequests).Returns(_dbRequestSet);

            var result = _meetingRepoMock.Object.getById(_meetingRequests.First().User.Id);

            _contextMock.Verify(f => f.MeetingRequests, Times.Once);

            Assert.AreEqual(2, result.Count);

            MockingHelper.CheckAssertsForMeetingRequest(_meetingRequests.First(), result.First());
            MockingHelper.CheckAssertsForMeetingRequest(_meetingRequests.Skip(1).First(), result.Skip(1).First());
        }

        [Test]
        public void getById_MeetingRequestsForUserNotFound()
        {
            DbSetMock<MeetingRequest> _dbRequestSet = new DbSetMock<MeetingRequest>();
            List<MeetingRequest> _meetingRequests = new List<MeetingRequest>();
            _meetingRequests.PopulateMeetingRequests(2);
            _meetingRequests.Skip(1).First().User.Id = _meetingRequests.First().User.Id;

            _meetingRequests.ForEach(f => _dbRequestSet.Add(MockingHelper.CreateMeetingRequestCopy(f)));

            _contextMock.Setup(f => f.MeetingRequests).Returns(_dbRequestSet);

            var result = _meetingRepoMock.Object.getById(_meetingRequests.First().User.Id);

            _contextMock.Verify(f => f.MeetingRequests, Times.Once);

            Assert.AreEqual(0, result.Count);

        }
        #endregion

        #region rejectMR
        // unnecessary arguments to this method: Meeting and User -> not used anywhere in the method
        //NullNotHandled in corresponding service
        [Test]
        public void rejectMR_MRRejected()
        {
            _meetings.PopulateMeetings(1);

            DbSetMock<MeetingRequest> _dbRequestSet = new DbSetMock<MeetingRequest>();
            List<MeetingRequest> _meetingRequests = new List<MeetingRequest>();
            _meetingRequests.PopulateMeetingRequests(3);
            _meetingRequests.Skip(1).First().Status = _meetingRequests.First().Status = false;
            _meetingRequests.First().Meeting = _meetings.First();
            _meetingRequests.First().ID = Guid.NewGuid();

            _meetingRequests.ForEach(f => _dbRequestSet.Add(MockingHelper.CreateMeetingRequestCopy(f)));

            _contextMock.Setup(f => f.MeetingRequests).Returns(_dbRequestSet);
            _contextMock.Setup(f => f.SaveChanges()).Returns(0);

            _meetingRepoMock.Object.rejectMR(_meetings.First(), _meetings.First().creator, _meetingRequests.First().ID.ToString());

            Assert.AreEqual(_dbRequestSet.Count(), 2);

            _contextMock.Verify(f => f.MeetingRequests, Times.Exactly(2));
            _contextMock.Verify(f => f.SaveChanges(), Times.Once);

        }

        #endregion

        #region getParticipants

        [Test]
        public void getParticipants_ResultObtained()
        {
            DbSetMock<Event> _dbEventSet = new DbSetMock<Event>();
            List<Event> _events = new List<Event>();
            _events.PopulateEvents(2);
            _events.ForEach(f => _dbEventSet.Add(MockingHelper.CreateEventCopy(f)));

            _contextMock.Setup(f => f.Events).Returns(_dbEventSet);

            var result = _meetingRepoMock.Object.getParticipants(_events.First().Title, _events.First().Start, _events.First().End, _events.First().Location, _events.First().Priority);

            _contextMock.Verify(f => f.Events, Times.Once);

            Assert.AreEqual(result.Count, 1);

            MockingHelper.CheckAssertsForEvent(_events.First(), result.First());
        }
        #endregion
    }
}