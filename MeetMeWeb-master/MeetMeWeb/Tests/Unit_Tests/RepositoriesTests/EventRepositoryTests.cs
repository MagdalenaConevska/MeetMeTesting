using MeetMeWeb.Models;
using MeetMeWeb.Repositories;
using MeetMeWeb.Tests.Mocks;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetMeWeb.Tests.Unit_Tests.RepositoriesTests
{
    [TestFixture]
    public class EventRepositoryTests
    {
        DbSetMock<Event> _dbSet;
        DbSetMock<Meeting> _dbMeetingSet;
        DbSetMock<MeetingRequest> _dbRequestsSet;
        Mock<MeetMeDbContext> _contextMock;
        Mock<EventRepository> _eventRepoMock;

        List<Event> _events;
        List<Meeting> _meetings;
        List<MeetingRequest> _meetingRequests;

        [SetUp]
        public void Setup()
        {
            _dbSet = new DbSetMock<Event>();
            _dbMeetingSet = new DbSetMock<Meeting>();
            _dbRequestsSet = new DbSetMock<MeetingRequest>();
            _contextMock = new Mock<MeetMeDbContext>();
            _eventRepoMock = new Mock<EventRepository>();

            _events = new List<Event>();
            _meetings = new List<Meeting>();
            _meetingRequests = new List<MeetingRequest>();

            _eventRepoMock.Object._context = _contextMock.Object;
        }

        #region deleteEvent

        [Test]
        public async Task deleteEvent_NoRelatedMeeting_EventDeleted()
        {
            _events.PopulateEvents(1);
            _events.ForEach(f => _dbSet.Add(MockingHelper.CreateEventCopy(f)));

            _meetings.PopulateMeetings(2);
            _meetings.ForEach(f => _dbMeetingSet.Add(MockingHelper.CreateMeetingCopy(f)));

            _meetingRequests.PopulateMeetingRequests(2);
            _meetingRequests.ForEach(f => _dbRequestsSet.Add(MockingHelper.CreateMeetingRequestCopy(f)));

            _contextMock.Setup(f => f.Events).Returns(_dbSet);
            _contextMock.Setup(f => f.Meetings).Returns(_dbMeetingSet);
            _contextMock.Setup(f => f.MeetingRequests).Returns(_dbRequestsSet);
            _contextMock.Setup(f => f.SaveChanges()).Returns(0);

            var result = await _eventRepoMock.Object.DeleteEvent(_events.First().Title, _events.First().ID, Guid.NewGuid().ToString());

            _contextMock.Verify(f => f.Events, Times.Exactly(3));
            _contextMock.Verify(f => f.Meetings, Times.Once);
            _contextMock.Verify(f => f.MeetingRequests, Times.Never);

            Assert.AreEqual(_dbSet.Count(), 0);
            MockingHelper.CheckAssertsForEvent(_events.First(), result);
        }

        [Test]
        public async Task deleteEvent_WithMeetingCreatorEventsAndMR_EventDeleted()
        {
            _events.PopulateEvents(3);
            _meetings.PopulateMeetings(2);
            _meetingRequests.PopulateMeetingRequests(1);

            _meetings.First().Title = _events.First().Title;
            _events.Skip(1).First().Title = _events.First().Title;
            _meetingRequests.First().Meeting.ID = _meetings.First().ID;

            _events.ForEach(f => _dbSet.Add(MockingHelper.CreateEventCopy(f)));
            _meetings.ForEach(f => _dbMeetingSet.Add(MockingHelper.CreateMeetingCopy(f)));
            _meetingRequests.ForEach(f => _dbRequestsSet.Add(MockingHelper.CreateMeetingRequestCopy(f)));

            _contextMock.Setup(f => f.Events).Returns(_dbSet);
            _contextMock.Setup(f => f.Meetings).Returns(_dbMeetingSet);
            _contextMock.Setup(f => f.MeetingRequests).Returns(_dbRequestsSet);
            _contextMock.Setup(f => f.SaveChanges()).Returns(0);

            var result = await _eventRepoMock.Object.DeleteEvent(_events.First().Title, _events.First().ID, _meetings.First().creator.UserName);

            _contextMock.Verify(f => f.Events, Times.Exactly(4));
            _contextMock.Verify(f => f.Meetings, Times.Exactly(2));
            _contextMock.Verify(f => f.MeetingRequests, Times.Exactly(2));
            _contextMock.Verify(f => f.SaveChanges(), Times.Once);

            Assert.AreEqual(_dbSet.Count(), 1);
            Assert.AreEqual(_dbRequestsSet.Count(), 0);
            Assert.AreEqual(_dbMeetingSet.Count(), 1);
            MockingHelper.CheckAssertsForEvent(_events.First(), result);
        }

        [Test]
        public async Task deleteEvent_WithMeetingCreatorAndEvents_EventDeleted()
        {
            _events.PopulateEvents(3);
            _meetings.PopulateMeetings(2);
            _meetingRequests.PopulateMeetingRequests(1);

            _meetings.First().Title = _events.First().Title;
            _events.Skip(1).First().Title = _events.First().Title;
            _meetingRequests.First().Meeting.ID = Guid.NewGuid();

            _events.ForEach(f => _dbSet.Add(MockingHelper.CreateEventCopy(f)));
            _meetings.ForEach(f => _dbMeetingSet.Add(MockingHelper.CreateMeetingCopy(f)));
            _meetingRequests.ForEach(f => _dbRequestsSet.Add(MockingHelper.CreateMeetingRequestCopy(f)));

            _contextMock.Setup(f => f.Events).Returns(_dbSet);
            _contextMock.Setup(f => f.Meetings).Returns(_dbMeetingSet);
            _contextMock.Setup(f => f.MeetingRequests).Returns(_dbRequestsSet);
            _contextMock.Setup(f => f.SaveChanges()).Returns(0);

            var result = await _eventRepoMock.Object.DeleteEvent(_events.First().Title, _events.First().ID, _meetings.First().creator.UserName);

            _contextMock.Verify(f => f.Events, Times.Exactly(4));
            _contextMock.Verify(f => f.Meetings, Times.Exactly(2));
            _contextMock.Verify(f => f.MeetingRequests, Times.Exactly(1));
            _contextMock.Verify(f => f.SaveChanges(), Times.Once);

            Assert.AreEqual(_dbSet.Count(), 1);
            Assert.AreEqual(_dbRequestsSet.Count(), 1);
            Assert.AreEqual(_dbMeetingSet.Count(), 1);
            MockingHelper.CheckAssertsForEvent(_events.First(), result);
        }

        [Test]
        public async Task deleteEvent_WithMeetingCreatorAndMR_EventDeleted()
        {
            _events.PopulateEvents(3);
            _meetings.PopulateMeetings(2);
            _meetingRequests.PopulateMeetingRequests(1);

            _meetings.First().Title = _events.First().Title;
            _meetingRequests.First().Meeting.ID = _meetings.First().ID;

            _events.ForEach(f => _dbSet.Add(MockingHelper.CreateEventCopy(f)));
            _meetings.ForEach(f => _dbMeetingSet.Add(MockingHelper.CreateMeetingCopy(f)));
            _meetingRequests.ForEach(f => _dbRequestsSet.Add(MockingHelper.CreateMeetingRequestCopy(f)));

            _contextMock.Setup(f => f.Events).Returns(_dbSet);
            _contextMock.Setup(f => f.Meetings).Returns(_dbMeetingSet);
            _contextMock.Setup(f => f.MeetingRequests).Returns(_dbRequestsSet);
            _contextMock.Setup(f => f.SaveChanges()).Returns(0);

            var result = await _eventRepoMock.Object.DeleteEvent(_events.First().Title, _events.First().ID, _meetings.First().creator.UserName);

            _contextMock.Verify(f => f.Events, Times.Exactly(3));
            _contextMock.Verify(f => f.Meetings, Times.Exactly(2));
            _contextMock.Verify(f => f.MeetingRequests, Times.Exactly(2));
            _contextMock.Verify(f => f.SaveChanges(), Times.Once);

            Assert.AreEqual(_dbSet.Count(), 2);
            Assert.AreEqual(_dbRequestsSet.Count(), 0);
            Assert.AreEqual(_dbMeetingSet.Count(), 1);
            MockingHelper.CheckAssertsForEvent(_events.First(), result);
        }

        [Test]
        public async Task deleteEvent_WithMeetingAndCreator_EventDeleted()
        {
            _events.PopulateEvents(3);
            _meetings.PopulateMeetings(2);
            _meetingRequests.PopulateMeetingRequests(1);

            _meetings.First().Title = _events.First().Title;

            _events.ForEach(f => _dbSet.Add(MockingHelper.CreateEventCopy(f)));
            _meetings.ForEach(f => _dbMeetingSet.Add(MockingHelper.CreateMeetingCopy(f)));
            _meetingRequests.ForEach(f => _dbRequestsSet.Add(MockingHelper.CreateMeetingRequestCopy(f)));

            _contextMock.Setup(f => f.Events).Returns(_dbSet);
            _contextMock.Setup(f => f.Meetings).Returns(_dbMeetingSet);
            _contextMock.Setup(f => f.MeetingRequests).Returns(_dbRequestsSet);
            _contextMock.Setup(f => f.SaveChanges()).Returns(0);

            var result = await _eventRepoMock.Object.DeleteEvent(_events.First().Title, _events.First().ID, _meetings.First().creator.UserName);

            _contextMock.Verify(f => f.Events, Times.Exactly(3));
            _contextMock.Verify(f => f.Meetings, Times.Exactly(2));
            _contextMock.Verify(f => f.MeetingRequests, Times.Exactly(1));
            _contextMock.Verify(f => f.SaveChanges(), Times.Once);

            Assert.AreEqual(_dbSet.Count(), 2);
            Assert.AreEqual(_dbRequestsSet.Count(), 1);
            Assert.AreEqual(_dbMeetingSet.Count(), 1);
            MockingHelper.CheckAssertsForEvent(_events.First(), result);
        }

        [Test]
        public async Task deleteEvent_WithMeeting_EventDeleted()
        {
            _events.PopulateEvents(3);
            _meetings.PopulateMeetings(2);
            _meetingRequests.PopulateMeetingRequests(1);

            _meetings.First().Title = _events.First().Title;

            _events.ForEach(f => _dbSet.Add(MockingHelper.CreateEventCopy(f)));
            _meetings.ForEach(f => _dbMeetingSet.Add(MockingHelper.CreateMeetingCopy(f)));
            _meetingRequests.ForEach(f => _dbRequestsSet.Add(MockingHelper.CreateMeetingRequestCopy(f)));

            _contextMock.Setup(f => f.Events).Returns(_dbSet);
            _contextMock.Setup(f => f.Meetings).Returns(_dbMeetingSet);
            _contextMock.Setup(f => f.MeetingRequests).Returns(_dbRequestsSet);
            _contextMock.Setup(f => f.SaveChanges()).Returns(0);

            var result = await _eventRepoMock.Object.DeleteEvent(_events.First().Title, _events.First().ID, Guid.NewGuid().ToString());

            _contextMock.Verify(f => f.Events, Times.Exactly(3));
            _contextMock.Verify(f => f.Meetings, Times.Exactly(1));
            _contextMock.Verify(f => f.MeetingRequests, Times.Never);
            _contextMock.Verify(f => f.SaveChanges(), Times.Once);

            Assert.AreEqual(_dbSet.Count(), 2);
            Assert.AreEqual(_dbRequestsSet.Count(), 1);
            Assert.AreEqual(_dbMeetingSet.Count(), 2);
            MockingHelper.CheckAssertsForEvent(_events.First(), result);
        }

        #endregion

        #region getEvents

        [Test]
        public void getEvents_EventsForUserFound()
        {
            _events.PopulateEvents(3);
            _events.First().User = _events.Skip(1).First().User;
            _events.Skip(2).First().User = new User
            {
                UserName = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid().ToString()
            };
            _events.ForEach(f => _dbSet.Add(MockingHelper.CreateEventCopy(f)));

            _contextMock.Setup(f => f.Events).Returns(_dbSet);

            var result = _eventRepoMock.Object.getEvents(_events.First().User.UserName);

            Assert.AreEqual(result.Count(), 2);
            MockingHelper.CheckAssertsForEvent(_events.First(), result.First());
            MockingHelper.CheckAssertsForEvent(_events.Skip(1).First(), result.Skip(1).First());
        }

        [Test]
        public void getEvents_EventsForUserNotFound()
        {
            _events.PopulateEvents(3);

            _events.ForEach(f => _dbSet.Add(MockingHelper.CreateEventCopy(f)));

            _contextMock.Setup(f => f.Events).Returns(_dbSet);

            var result = _eventRepoMock.Object.getEvents(Guid.NewGuid().ToString());

            Assert.AreEqual(result.Count(), 0);
        }
        #endregion

        #region EditEvent

        [Test]
        public void EditEvent_EventSuccessfullyEdited()
        {
            _events.PopulateEvents(3);
            _events.ForEach(f => _dbSet.Add(f));

            _contextMock.Setup(f => f.Events).Returns(_dbSet);

            DateTime newStartDate = DateTime.UtcNow.AddDays(-1);
            DateTime newEndDate = DateTime.UtcNow;

            var result = _eventRepoMock.Object.EditEvent("NewTitle", _events.First().ID, newStartDate, newEndDate);

            _contextMock.Verify(f => f.Events, Times.Once);

            Assert.AreEqual(_events.First().Title, result.Title);
            Assert.AreEqual(newStartDate, result.Start);
            Assert.AreEqual(newEndDate, result.End);

        }

        [Test]
        public void EditEvent_InvalidStartEndDatesButStill_EventEdited()
        {
            _events.PopulateEvents(3);
            _events.ForEach(f => _dbSet.Add(f));

            _contextMock.Setup(f => f.Events).Returns(_dbSet);

            DateTime newEndDate = DateTime.UtcNow.AddDays(-1);
            DateTime newStartDate = DateTime.UtcNow;

            Assert.Throws<Exception>(() =>
            {
                _eventRepoMock.Object.EditEvent("NewTitle", _events.First().ID, newStartDate, newEndDate);
            });

            _contextMock.Verify(f => f.Events, Times.Once);
        }

        #endregion

    }
}