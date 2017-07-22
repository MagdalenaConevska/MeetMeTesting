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