using Moq;
using MeetMeWeb.Repositories.Interfaces;
using NUnit.Framework;
using MeetMeWeb.Models;
using MeetMeWeb.Services;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MeetMeWeb.Tests.Unit_Tests
{
    [TestFixture]
    public class EventServiceTests
    {
        Mock<EventService> _eventServiceMock;
        Mock<IEventRepository> _eventRepoMock;

        List<Event> _events;

        [SetUp]
        public void Setup()
        {
            _eventRepoMock = new Mock<IEventRepository>();

            _events = new List<Event>();

            _eventServiceMock = new Mock<EventService>(new object[] { _eventRepoMock.Object })
            {
                CallBase = true
            };
        }

        #region createEvent

        [Test]
        public async Task createEvent_EventCreatedAsync()
        {
            _events.PopulateEvents(1);
            Event eventModel = _events.First();

            _eventRepoMock.Setup(f => f.CreateEvent(It.IsAny<Event>())).ReturnsAsync(eventModel);

            var result = await _eventServiceMock.Object.createEvent(eventModel);

            _eventRepoMock.Verify(f => f.CreateEvent(It.Is<Event>(k => k.ID == eventModel.ID &&
                                                                       k.Location == eventModel.Location &&
                                                                       k.Title == eventModel.Title &&
                                                                       k.User.Id == eventModel.User.Id &&
                                                                       k.Priority == eventModel.Priority &&
                                                                       k.MR.ID == eventModel.MR.ID)), Times.Once);
            MockingHelper.CheckAssertsForEvent(eventModel, result);
        }

        #endregion

        #region deleteEvent

        [Test]
        public async Task deleteEvent_EventDeletedAsync()
        {
            _events.PopulateEvents(1);
            Event eventModel = _events.First();

            _eventRepoMock.Setup(f => f.DeleteEvent(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(eventModel);

            var result = await _eventServiceMock.Object.deleteEvent(eventModel.Title, eventModel.ID, eventModel.User.UserName);

            _eventRepoMock.Verify(f => f.DeleteEvent(It.Is<string>(k => k == eventModel.Title),
                                                     It.Is<Guid>(k => k == eventModel.ID),
                                                     It.Is<string>(k => k == eventModel.User.UserName)), Times.Once);

            MockingHelper.CheckAssertsForEvent(eventModel, result);
        }

        #endregion

        #region editEvent

        [Test]
        public void editEvent_EventEdited()
        {
            _events.PopulateEvents(1);
            Event eventModel = _events.First();

            _eventRepoMock.Setup(f => f.EditEvent(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(eventModel);

            var result = _eventServiceMock.Object.editEvent(eventModel.Title, eventModel.ID, eventModel.Start, eventModel.End);

            _eventRepoMock.Verify(f => f.EditEvent(It.Is<string>(k => k == eventModel.Title),
                                                     It.Is<Guid>(k => k == eventModel.ID),
                                                     It.Is<DateTime>(k => k == eventModel.Start),
                                                     It.Is<DateTime>(k => k == eventModel.End)), Times.Once);

            MockingHelper.CheckAssertsForEvent(eventModel, result);

        }

        #endregion

        #region getEvents

        [Test]
        public void getEvents_EventsFound()
        {
            _events.PopulateEvents(2);
            Event firstEvent = _events.First();
            Event secondEvent = _events.Skip(1).First();

            secondEvent.User.UserName = firstEvent.User.UserName;

            _eventRepoMock.Setup(f => f.getEvents(It.IsAny<string>())).Returns(_events);

            var result = _eventServiceMock.Object.getEvents(firstEvent.User.UserName);

            _eventRepoMock.Verify(f => f.getEvents(It.Is<string>(k => k == firstEvent.User.UserName)), Times.Once);

            for (int i = 0; i < _events.Count(); i++)
            {
                MockingHelper.CheckAssertsForEvent(_events.Skip(i).First(), result.Skip(i).First());
            }
        }

        #endregion

    }
}