using MeetMeWeb.Models;
using MeetMeWeb.Repositories.Interfaces;
using MeetMeWeb.Services;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MeetMeWeb.Tests.Unit_Tests
{
    public class MeetingRequestServiceTests
    {
        Mock<MeetingRequestService> _meetingRequestServiceMock;
        Mock<IMeetingRequestRepository> _meetingRequestRepoMock;

        [SetUp]
        public void Setup()
        {
            _meetingRequestRepoMock = new Mock<IMeetingRequestRepository>();

            _meetingRequestServiceMock = new Mock<MeetingRequestService>(new object[] { _meetingRequestRepoMock.Object })
            {
                CallBase = true
            };
        }

        #region createMeetingRequest

        [Test]
        public void createMeetingRequest_RequestCreated()
        {
            List<MeetingRequest> meetingRequests = new List<MeetingRequest>();
            meetingRequests.PopulateMeetingRequests(1);

            _meetingRequestRepoMock.Setup(f => f.createMeetingRequest(It.IsAny<MeetingRequest>()));

            _meetingRequestServiceMock.Object.createMeetingRequest(meetingRequests.First());

            _meetingRequestRepoMock.Verify(f => f.createMeetingRequest(It.Is<MeetingRequest>(k => k.ID == meetingRequests.First().ID &&
                                                                                                  k.Meeting.ID == meetingRequests.First().Meeting.ID &&
                                                                                                  k.Meeting.Title == meetingRequests.First().Meeting.Title &&
                                                                                                  k.Status == meetingRequests.First().Status &&
                                                                                                  k.User.Id == meetingRequests.First().User.Id)), Times.Once);
        }

        #endregion
    }
}