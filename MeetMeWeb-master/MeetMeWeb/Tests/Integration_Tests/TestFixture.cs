using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Moq;
using NUnit.Framework;
using MeetMeWeb.Models;
using MeetMeWeb.Services;
using MeetMeWeb.Repositories;
using System.Threading;

namespace MeetMeWeb.Tests.Integration_Tests
{

    [TestFixture]
    public class TestFixture
    {

        private UserRepository _userRepo;
        private ConnectionNotificationRepository _connectionNotifRepo;
        private EventRepository _eventRepo;
        private MeetingRepository _meetingRepo;
        private MeetingRequestRepository _meetingRequestRepo;

        private UserService _userService;
        private EventService _eventService;
        private MeetingService _meetingService;
        private User associatedUser;
        private List<User> allUsers;

        [SetUp]
        public void Setup()
        {
            _userRepo = new UserRepository();
            _connectionNotifRepo = new ConnectionNotificationRepository();
            _eventRepo = new EventRepository();
            _meetingRepo = new MeetingRepository();
            _meetingRequestRepo = new MeetingRequestRepository();

            _userService = new UserService(_userRepo, _connectionNotifRepo);
            _eventService = new EventService(_eventRepo);
            _meetingService = new MeetingService(_meetingRepo, _meetingRequestRepo);
        }

        [TearDown]
        public void Dispose()
        {
            _userRepo.Dispose();
            _connectionNotifRepo.Dispose();
            _eventRepo.Dispose();
            _meetingRepo.Dispose();
            _meetingRequestRepo.Dispose();
            _userService.Dispose();
            _eventService.Dispose();
           _meetingService.Dispose();
        }

        [Test]
        public async System.Threading.Tasks.Task createDeleteEvent_withoutMRAsync()
        {
            associatedUser = _userService.getAll()[0];

            // Before creating event
            List<Event> eventsPerUserBefore = _eventService.getEvents(associatedUser.UserName);
            //Console.WriteLine(eventsPerUserBefore.ToString());
            Event eventToAdd = new Event
            {
                Title = "Integration_Tests_createEvent_withoutMR",
                flag = false,
                Location = Guid.NewGuid().ToString(),
                User = associatedUser,
                Priority = PrioritiesY.High,
                MR = null,
                End = DateTime.UtcNow,
                Start = DateTime.UtcNow
            };

            // Adding event
            Event e = await _eventService.createEvent(eventToAdd);
            // Before creating event
            List<Event> eventsPerUserAfter = _eventService.getEvents(associatedUser.UserName);
            //Removing event, cleaning DB
            Event resultDelete = await _eventService.deleteEvent(e.Title, e.ID, e.User.UserName);
            Assert.AreNotEqual(eventsPerUserBefore.Count, eventsPerUserAfter.Count);
            Assert.That(eventsPerUserAfter.Any(p => p.Title == e.Title && p.ID == e.ID));
        }

        [Test]
        public async System.Threading.Tasks.Task createDeleteMeeting_withoutParticipantsAsync()
        {
            allUsers = _userService.getAll();
            User creator = associatedUser;
            Meeting meeting = new Meeting
            {
                Title = "Integration_Tests_createMeeting",
                Location = Guid.NewGuid().ToString(),
                Priority = PrioritiesY.High,
                Start = DateTime.Now,
                End = DateTime.Now,
                creator = creator
            };

            Random r = new Random();
            List<User> meetingParticipants = new List<User> ();
            //int numUsers = allUsers.Count;
            //int firstParticipantIndex = r.Next(1, numUsers);
            //int secondParticipantIndex = firstParticipantIndex;
            /*while (secondParticipantIndex == firstParticipantIndex)
            {
                secondParticipantIndex = r.Next(1, numUsers);
            }*/
           
            // meetingParticipants.Add(allUsers[2]);

            MeetingModel meetingModel = new MeetingModel
            {
                Title = meeting.Title,
                Location = meeting.Location,
                Priority = meeting.Priority,
                Start = meeting.Start,
                End = meeting.End,
                creator = creator,
                participants = meetingParticipants
            };

            _meetingService.createMeeting(meetingModel);
            Meeting createdMeeting = _meetingService.getByTitle(meeting.Title);
            Assert.IsNotNull(createdMeeting);
            Event e = await _eventService.deleteEvent(meeting.Title, createdMeeting.ID, createdMeeting.creator.UserName);

        }
    }
}