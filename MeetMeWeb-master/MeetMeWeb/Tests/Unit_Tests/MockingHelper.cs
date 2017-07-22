using MeetMeWeb.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeetMeWeb.Tests.Unit_Tests
{
    public static class MockingHelper
    {
        #region PopulationExtensionMethods

        public static List<User> PopulateUsers(this List<User> users, int number)
        {
            for (int i = 0; i < number; i++)
            {
                users.Add(new User
                {
                    Id = number.ToString(),
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = Guid.NewGuid().ToString(),
                    Email = Guid.NewGuid().ToString()+"@gmail.com",
                    BirthDate = DateTime.UtcNow,
                    UserName = Guid.NewGuid().ToString()
                });
            }
            return users;
        }

        public static List<ConnectionNotification> PopulateConnectionNotifications(this List<ConnectionNotification> connectionNotifications, int number)
        {
            List<User> users = new List<User>();
            users.PopulateUsers(2);

            for (int i = 0; i < number; i++)
            {
                connectionNotifications.Add(new ConnectionNotification
                {
                    ID = Guid.NewGuid(),
                    Content = Guid.NewGuid().ToString(),
                    Date = DateTime.UtcNow,
                    Read = true,
                    User1 = users.Skip(0).First(),
                    User2 = users.Skip(1).First()
                });
            }
            return connectionNotifications;
        }

        public static List<Meeting> PopulateMeetings(this List<Meeting> meetings, int number)
        {
            List<User> users = new List<User>();
            users.PopulateUsers(1);

            for (int i = 0; i < number; i++)
            {
                meetings.Add(new Meeting
                {
                    ID = Guid.NewGuid(),
                    Start = DateTime.Now,
                    Title = Guid.NewGuid().ToString(),
                    Priority = PrioritiesY.Medium,
                    Location = Guid.NewGuid().ToString(),
                    creator = users.First(),
                    End = DateTime.UtcNow.AddHours(2)
                });
            }
            return meetings;
        }

        public static List<MeetingModel> PopulateMeetingModels(this List<MeetingModel> meetingModels, int number)
        {
            List<User> participants = new List<User>();
            participants.PopulateUsers(3);

            for (int i = 0; i < number; i++)
            {
                meetingModels.Add(new MeetingModel
                {
                    participants= participants,
                    Start = DateTime.Now,
                    Title = Guid.NewGuid().ToString(),
                    Priority = PrioritiesY.Medium,
                    Location = Guid.NewGuid().ToString(),
                    creator = participants.First(),
                    End = DateTime.UtcNow.AddHours(2)
                });
            }
            return meetingModels;
        }

        #endregion

        #region CreatingCopiesMethods

        public static User CreateUserCopy(User original)
        {
            return new User
            {
                Id = original.Id,
                FirstName = original.FirstName,
                LastName = original.LastName,
                Email = original.Email,
                BirthDate = original.BirthDate,
                UserName = original.UserName
            };
        }

        public static ConnectionNotification CreateConnectionNotificationCopy(ConnectionNotification original)
        {
            return new ConnectionNotification
            {
                ID = original.ID,
                Content = original.Content,
                Date = original.Date,
                Read = original.Read,
                User1 = CreateUserCopy(original.User1),
                User2 = CreateUserCopy(original.User2)
            };
        }
        #endregion

        #region CheckAssertsMethods

        public static void CheckAssertsForUser(User expected, User actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.Email, actual.Email);
            Assert.AreEqual(expected.UserName, actual.UserName);
            Assert.AreEqual(expected.BirthDate, actual.BirthDate);
        }

        public static void CheckAssertsForConnectionNotification(ConnectionNotification expected, ConnectionNotification actual)
        {
            Assert.AreEqual(expected.ID, actual.ID);
            Assert.AreEqual(expected.Content, actual.Content);
            Assert.AreEqual(expected.Date, actual.Date);
            CheckAssertsForUser(expected.User1, actual.User1);
            CheckAssertsForUser(expected.User2, actual.User2);
        }

        #endregion
    }
}