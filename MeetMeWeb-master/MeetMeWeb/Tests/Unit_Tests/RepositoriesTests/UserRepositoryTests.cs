using MeetMeWeb.Models;
using MeetMeWeb.Repositories;
using MeetMeWeb.Tests.Mocks;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeetMeWeb.Tests.Unit_Tests.RepositoriesTests
{
    [TestFixture]
    public class UserRepositoryTests
    {
        DbSetMock<User> _dbSet;
        Mock<MeetMeDbContext> _contextMock;
        Mock<UserRepository> _userRepoMock;

        List<User> _users;

        [SetUp]
        public void Setup()
        {
            _dbSet = new DbSetMock<User>();
            _contextMock = new Mock<MeetMeDbContext>();
            _userRepoMock = new Mock<UserRepository>();

            _users = new List<User>();

            _userRepoMock.Object._context = _contextMock.Object;
        }

        #region getUserById
        [Test]
        public void getUserById_UserFound_Repo()
        {
            _users.PopulateUsers(3);
            _users.ForEach(f => _dbSet.Add(MockingHelper.CreateUserCopy(f)));

            _contextMock.Setup(f => f.Users).Returns(_dbSet);

            User result = _userRepoMock.Object.getUserById(_users.First().Id);

            _contextMock.Verify(f => f.Users, Times.Once);

            MockingHelper.CheckAssertsForUser(_users.First(), result);
        }

        [Test]
        public void getUserById_UserNotFound_Repo()
        {
            _users.PopulateUsers(2);
            _users.ForEach(f => _dbSet.Add(f));

            _contextMock.Setup(f => f.Users).Returns(_dbSet);

            var result = _userRepoMock.Object.getUserById(Guid.NewGuid().ToString());

            _contextMock.Verify(f => f.Users, Times.Once);

            Assert.IsNull(result);
        }
        #endregion
        #region getUserByUsername
        [Test]
        public void getUserByUsername_UserFoundInLocal_Repo()
        {
            _users.PopulateUsers(2);
            _users.ForEach(f => _dbSet.Add(MockingHelper.CreateUserCopy(f)));

            _contextMock.Setup(f => f.Users).Returns(_dbSet);

            var result = _userRepoMock.Object.getUserByUsername(_users.First().UserName);

            _contextMock.Verify(f => f.Users, Times.Once);

            MockingHelper.CheckAssertsForUser(_users.First(), result);
        }

        [Test]
        public void getUserByUsername_UserFoundNotInLocal_Repo()
        {
            _users.PopulateUsers(2);
            _users.ForEach(f => _dbSet.Add(MockingHelper.CreateUserCopy(f)));

            _dbSet.Local.RemoveAt(0);

            _contextMock.Setup(f => f.Users).Returns(_dbSet);

            var result = _userRepoMock.Object.getUserByUsername(_users.First().UserName);

            _contextMock.Verify(f => f.Users, Times.Exactly(2));

            MockingHelper.CheckAssertsForUser(_users.First(), result);
        }

        [Test]
        public void getUserByUsername_UserNotFound_Repo()
        {
            _users.PopulateUsers(1);
            _users.ForEach(f => _dbSet.Add(MockingHelper.CreateUserCopy(f)));

            _contextMock.Setup(f => f.Users).Returns(_dbSet);

            var result = _userRepoMock.Object.getUserByUsername(Guid.NewGuid().ToString());

            _contextMock.Verify(f => f.Users, Times.Exactly(2));

            Assert.IsNull(result);
        }

        [Test]
        public void getUserByUsername_UsenameNotUnique_ThrowsInvalidOperationException()
        {
            _users.PopulateUsers(3);
            _users.Skip(1).First().UserName = _users.First().UserName;
            _users.ForEach(f => _dbSet.Add(MockingHelper.CreateUserCopy(f)));

            _contextMock.Setup(f => f.Users).Returns(_dbSet);

            Assert.Throws<InvalidOperationException>(() => {

               _userRepoMock.Object.getUserByUsername(_users.First().UserName);

            });
            
            _contextMock.Verify(f => f.Users, Times.Exactly(1));
        }

        #endregion

        #region getAll

        [Test]
        public void getAll()
        {
            _users.PopulateUsers(3);
            _users.ForEach(f => _dbSet.Add(MockingHelper.CreateUserCopy(f)));

            _contextMock.Setup(f => f.Users).Returns(_dbSet);

            var result = _userRepoMock.Object.getAll();

            _contextMock.Verify(f => f.Users, Times.Once);

            int i = 0;
            _users.ForEach(f => MockingHelper.CheckAssertsForUser(f, result.Skip(i++).First()));

        }

        #endregion

    }
}