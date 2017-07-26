using MeetMeWeb.Models;
using MeetMeWeb.Repositories;
using MeetMeWeb.Repositories.Interfaces;
using MeetMeWeb.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeetMeWeb.Tests.Unit_Tests
{
    [TestFixture]
    public class MeetingServiceTests
    {
        Mock<MeetingService> _meetingServiceMock;

        Mock<IMeetingRepository> _repositoryMock;
        Mock<IMeetingRequestRepository> _repositoryMRMock;

        List<MeetingModel> _meetingModels;
        List<Meeting> _meetings;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IMeetingRepository>();
            _repositoryMRMock = new Mock<IMeetingRequestRepository>();

            _meetingModels = new List<MeetingModel>();
            _meetings = new List<Meeting>();

            _meetingServiceMock = new Mock<MeetingService>(new object[] { _repositoryMock.Object, _repositoryMRMock.Object })
            {
                CallBase = true
            };

        }

        #region createMeeting

        [Test]
        public void createMeeting_AndMeetingRequestIsOk()
        {
            _meetingModels.PopulateMeetingModels(1);
            _meetings.PopulateMeetings(1);

            _meetings.First().Title = _meetingModels.First().Title;
            _meetings.First().Start = _meetingModels.First().Start;
            _meetings.First().End = _meetingModels.First().End;
            _meetings.First().Location = _meetingModels.First().Location;
            _meetings.First().Priority = _meetingModels.First().Priority;
            _meetings.First().creator = _meetingModels.First().creator;

            _repositoryMock.Setup(f => f.CreateMeeting(It.IsAny<Meeting>())).Returns(_meetings.First());
            _repositoryMRMock.Setup(f => f.createMeetingRequest(It.IsAny<MeetingRequest>()));

            _meetingServiceMock.Object.createMeeting(_meetingModels.First());

            _repositoryMock.Verify(f => f.CreateMeeting(It.Is<Meeting>(k => k.Title == _meetingModels.First().Title &&
                                                                            k.Start == _meetingModels.First().Start &&
                                                                            k.End == _meetingModels.First().End &&
                                                                            k.Location == _meetingModels.First().Location &&
                                                                            k.Priority == _meetingModels.First().Priority &&
                                                                            k.creator.Id == _meetingModels.First().creator.Id)), Times.Once);

            for (int i = 0; i < _meetingModels.First().participants.Count; i++)
            {
                _repositoryMRMock.Verify(f => f.createMeetingRequest(It.Is<MeetingRequest>(k => k.Meeting.Title == _meetingModels.First().Title &&
                                                                                                k.Meeting.Start == _meetingModels.First().Start &&
                                                                                                k.Meeting.End == _meetingModels.First().End &&
                                                                                                k.Meeting.Location == _meetingModels.First().Location &&
                                                                                                k.Meeting.Priority == _meetingModels.First().Priority &&
                                                                                                k.Meeting.creator.Id == _meetingModels.First().creator.Id &&
                                                                                                k.Content == "Do you want to accept request for " + _meetingModels.First().Title &&
                                                                                                k.Status == false &&
                                                                                                k.User.Id == _meetingModels.First().participants.Skip(i).First().Id)), Times.Once);

            }

            _repositoryMRMock.Verify(f => f.createMeetingRequest(It.IsAny<MeetingRequest>()), Times.Exactly(_meetingModels.First().participants.Count));

        }
        #endregion

        #region getByTitle

        [Test]
        public void getByTitle_MeetingFound()
        {
            _meetings.PopulateMeetings(1);
            Meeting expected = MockingHelper.CreateMeetingCopy(_meetings.First());

            _repositoryMock.Setup(f => f.getByTitle(It.IsAny<string>())).Returns(_meetings.First());

            var result = _meetingServiceMock.Object.getByTitle(_meetings.First().Title);

            _repositoryMock.Verify(f => f.getByTitle(It.Is<string>(k => k == expected.Title)), Times.Once);

            MockingHelper.CheckAssertsForMeeting(expected, result);
        }

        #endregion

        #region getById

        [Test]
        public void getById_MeetingRequstsFound()
        {
            List<MeetingRequest> meetingRequests = new List<MeetingRequest>();
            meetingRequests.PopulateMeetingRequests(3);

            meetingRequests.Skip(1).First().User.Id = meetingRequests.First().User.Id;
            meetingRequests.Skip(2).First().User.Id = meetingRequests.First().User.Id;

            List<MeetingRequest> expected = new List<MeetingRequest>();

            foreach (MeetingRequest meetingRequest in meetingRequests)
            {
                expected.Add(MockingHelper.CreateMeetingRequestCopy(meetingRequest));
            }

            _repositoryMock.Setup(f => f.getById(It.IsAny<string>())).Returns(meetingRequests);

            var result = _meetingServiceMock.Object.getById(meetingRequests.First().User.Id);

            _repositoryMock.Verify(f => f.getById(It.Is<string>(k => k == expected.First().User.Id)), Times.Once);

            Assert.AreEqual(expected.Count(), result.Count());
            for (int i = 0; i < result.Count; i++)
            {
                MockingHelper.CheckAssertsForMeetingRequest(expected.Skip(i).First(), result.Skip(i).First());
            }

        }

        #endregion

        #region acceptMR

        [Test]
        public void acceptMR_AndEverythingIsOk()
        {
            List<User> users = new List<User>();
            users.PopulateUsers(1);

            _meetings.PopulateMeetings(1);

            _repositoryMock.Setup(f => f.acceptMR(It.IsAny<Meeting>(), It.IsAny<User>(), It.IsAny<string>()));

            _meetingServiceMock.Object.acceptMR(_meetings.First(), users.First(), "testId");

            _repositoryMock.Verify(f => f.acceptMR(It.Is<Meeting>(k => k.ID == _meetings.First().ID &&
                                                                       k.Location == _meetings.First().Location &&
                                                                       k.Priority == _meetings.First().Priority &&
                                                                       k.Title == _meetings.First().Title &&
                                                                       k.Start == _meetings.First().Start &&
                                                                       k.End == _meetings.First().End &&
                                                                       k.creator.Id == _meetings.First().creator.Id),

                                                   It.Is<User>(u => u.Id == users.First().Id &&
                                                                    u.FirstName == users.First().FirstName &&
                                                                    u.UserName == users.First().UserName),

                                                   It.Is<string>(s => s == "testId")), Times.Once);

        }

        #endregion

        #region rejectMR

        [Test]
        public void rejectMR_AndEverythingIsOk()
        {
            List<User> _users = new List<User>();
            _users.PopulateUsers(1);

            _meetings.PopulateMeetings(1);

            _repositoryMock.Setup(f => f.rejectMR(It.IsAny<Meeting>(), It.IsAny<User>(), It.IsAny<string>()));

            _meetingServiceMock.Object.rejectMR(_meetings.First(), _users.First(), "testId");

            _repositoryMock.Verify(f => f.rejectMR(It.Is<Meeting>(k => k.ID == _meetings.First().ID &&
                                                                       k.Location == _meetings.First().Location &&
                                                                       k.Priority == _meetings.First().Priority &&
                                                                       k.Title == _meetings.First().Title &&
                                                                       k.Start == _meetings.First().Start &&
                                                                       k.End == _meetings.First().End &&
                                                                       k.creator.Id == _meetings.First().creator.Id),

                                                   It.Is<User>(u => u.Id == _users.First().Id &&
                                                                    u.FirstName == _users.First().FirstName &&
                                                                    u.UserName == _users.First().UserName),

                                                   It.Is<string>(s => s == "testId")), Times.Once);
        }
        #endregion

        #region getParticipants

        [Test]
        public void getParticipants_EventsFound()
        {
            List<Event> events = new List<Event>();
            events.PopulateEvents(2);

            List<Event> expected = new List<Event>();

            foreach (Event ev in events)
            {
                expected.Add(MockingHelper.CreateEventCopy(ev));
            }

            Event firstEvent = events.First();

            _repositoryMock.Setup(f => f.getParticipants(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<PrioritiesY>())).Returns(events);

            var result = _meetingServiceMock.Object.getParticipants(firstEvent.Title, firstEvent.Start, firstEvent.End, firstEvent.Location, firstEvent.Priority);

            _repositoryMock.Verify(f => f.getParticipants(It.Is<string>(k => k == firstEvent.Title), It.Is<DateTime>(k => k == firstEvent.Start), It.Is<DateTime>(k => k == firstEvent.End), It.Is<string>(k => k == firstEvent.Location), It.Is<PrioritiesY>(k => k == firstEvent.Priority)), Times.Once);

            Assert.AreEqual(expected.Count(), result.Count());

            for (int i = 0; i < result.Count(); i++)
            {
                MockingHelper.CheckAssertsForEvent(expected.Skip(i).First(), result.Skip(i).First());
            }
        }

        #endregion


        #region BlackBoxInputTests

        [Test]
        public void createMeeting_AndInvalidStartEndDates_ShouldNotPerformOperation()
        {
            _meetingModels.PopulateMeetingModels(1);
            _meetingModels.First().Start = DateTime.UtcNow.AddDays(1);
            _meetingModels.First().End = DateTime.UtcNow;
            _meetingModels.First().participants = new List<User>();

            _meetings.PopulateMeetings(1);
            _meetings.First().Start = _meetingModels.First().Start;
            _meetings.First().Start = _meetingModels.First().Start;

            _repositoryMock.Setup(f => f.CreateMeeting(It.IsAny<Meeting>())).Returns(_meetings.First());
            _repositoryMRMock.Setup(f => f.createMeetingRequest(It.IsAny<MeetingRequest>()));

            _meetingServiceMock.Object.createMeeting(_meetingModels.First());

            _repositoryMock.Verify(f => f.CreateMeeting(It.IsAny<Meeting>()), Times.Never);
        }

        #endregion


    }
}