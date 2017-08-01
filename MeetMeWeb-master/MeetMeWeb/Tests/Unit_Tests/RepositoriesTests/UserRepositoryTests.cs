using MeetMeWeb.Models;
using MeetMeWeb.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace MeetMeWeb.Tests.Unit_Tests.RepositoriesTests
{
    [TestFixture]
    public class UserRepositoryTests : DbSet<User>
    {
        Mock<MeetMeDbContext> _contextMock;
        Mock<UserRepository> _userRepoMock;

        List<User> _users;

        [SetUp]
        public void Setup()
        {
            _contextMock = new Mock<MeetMeDbContext>();
            _userRepoMock = new Mock<UserRepository>();

            _userRepoMock.Object._context = _contextMock.Object;

            _users = new List<User>();         
        }

        #region getUserById

        [Test]
        public void getUserById_UserFound_Repo()
        {
            _users.PopulateUsers(1);
            User user = _users.First();
            User mockedUser = MockingHelper.CreateUserCopy(user);

            _contextMock.Setup(f => f.Users.Find(It.IsAny<string>())).Returns(user);

            var result = _userRepoMock.Object.getUserById(user.Id);

            _contextMock.Verify(f => f.Users.Find(It.Is<string>(k => k == mockedUser.Id)), Times.Once);

            MockingHelper.CheckAssertsForUser(mockedUser, result);
        }

        #endregion

        #region getUserByUsername

        [Test]
        public void getUserByUsername_UserFoundLocal_Repo()
        {
            _users.PopulateUsers(2);
            User user = _users.First();
            User mockedUser = MockingHelper.CreateUserCopy(user);

            ObservableCollection<User> _dbUsers = new ObservableCollection<User>();

            foreach (User u in _users)
            {
                _dbUsers.Add(u);
            }

            _contextMock.Setup(f => f.Users.Local).Returns(_dbUsers);

            var result = _userRepoMock.Object.getUserByUsername(user.UserName);

            _contextMock.Verify(f => f.Users.Local, Times.Once);
        }

        //This test is implemented without using the mocking framework, because in order to write a test
        //which tests the if condition when true, we needed to mock f.Users from the _context object, but
        //we couldn't set a return value due to the protected key DbSet of the DbSet<User> implementation 
        //However this is only a get method and is not altering the database with test content.
        //We will make this kind of "no-mock" tests in similar scenarios.
        [Test]
        public void getUserByUsername_UserNotFound_WithoutMocking_Repo()
        {
            UserRepository userRepository = new UserRepository();

            var result = userRepository.getUserByUsername(Guid.NewGuid().ToString());

            Assert.IsNull(result);
        }

        #endregion

        #region getAll

        [Test]
        public void getAll_Repo()
        {
            UserRepository userRepository = new UserRepository();

            var result = userRepository.getAll();

            Assert.IsNotNull(result);
        }

        #endregion




    }
}