using MeetMeWeb.App_Start;
using MeetMeWeb.Models;
using MeetMeWeb.Repositories;
using MeetMeWeb.Services;
using MeetMeWeb.Tests.Mocks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MeetMeWeb.Tests.Unit_Tests.RepositoriesTests
{
    [TestFixture]
    public class AuthRepositoryTests
    {
        DbSetMock<User> _dbSet;
        Mock<MeetMeDbContext> _contextMock;
        Mock<ApplicationUserManager> _userManagerMock;
        Mock<UserStore<User>> _userStoreMock;
        Mock<ApplicationUserManager> _userAppMock;
        Mock<AuthRepository> _authRepoMock;

        List<User> _users;

        [SetUp]
        public void Setup()
        {
            _dbSet = new DbSetMock<User>();
            _contextMock = new Mock<MeetMeDbContext>();
            _userManagerMock = new Mock<ApplicationUserManager>(new UserStore<User>(_contextMock.Object));
            _authRepoMock = new Mock<AuthRepository>();

            //_fileCommentServiceMock = new Mock<FileCommentService>(new object[] { _apiUser, _unitOfWorkMock.Object, _requestInfo, _notificationServiceMock.Object })
            //{
            //    CallBase = true
            //};

            ////_userManagerMock.Object.EmailService = new EmailService();

            _users = new List<User>();

            _authRepoMock.Object._context = _contextMock.Object;
            _authRepoMock.Object._userManager = _userManagerMock.Object;
        }

        #region RegisterUser

        [Test]
        public async Task RegisterUser_Success()
        {
            UserModel userModel = new UserModel
            {
                FirstName = "Magdalena",
                LastName = "Conveska",
                Email = "test@meet.me",
                Password = "passwordTest",
                ConfirmPassword = "passwordTest"
            };

            _userManagerMock.Setup(f => f.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(new IdentityResult());

            var result = await _authRepoMock.Object.RegisterUser(userModel, null);

            _userManagerMock.Verify(f => f.CreateAsync(It.Is<User>(k => k.FirstName == userModel.FirstName &&
                                                                        k.LastName == userModel.LastName &&
                                                                        k.Email == userModel.Email &&
                                                                        k.UserName == userModel.Email &&
                                                                        k.BirthDate <= DateTime.Now), It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region ConfirmEmail
        [Test]
        public async Task ConfirmEmail()
        {
            _userManagerMock.Setup(f => f.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new IdentityResult());

            var result = await _authRepoMock.Object.ConfirmEmail(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            _userManagerMock.Verify(f => f.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            Assert.IsNotNull(result);
        }

        #endregion

        #region FindUser

        [Test]
        public async Task FindUser()
        {
            _userManagerMock.Setup(f => f.FindAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new User());

            var result = await _authRepoMock.Object.FindUser(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            _userManagerMock.Verify(f => f.FindAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            Assert.IsNotNull(result);
        }

        #endregion

        #region FindUser

        [Test]
        public async Task AddLoginAsync()
        {
            _userManagerMock.Setup(f => f.AddLoginAsync(It.IsAny<string>(), It.IsAny<UserLoginInfo>())).ReturnsAsync(new IdentityResult());

            var result = await _authRepoMock.Object.AddLoginAsync(Guid.NewGuid().ToString(), new UserLoginInfo("",""));

            _userManagerMock.Verify(f => f.AddLoginAsync(It.IsAny<string>(), It.IsAny<UserLoginInfo>()), Times.Once);

            Assert.IsNotNull(result);
        }

        #endregion
    }
}