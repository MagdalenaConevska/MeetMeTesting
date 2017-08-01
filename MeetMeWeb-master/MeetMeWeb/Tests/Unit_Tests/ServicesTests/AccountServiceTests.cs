using MeetMeWeb.Models;
using MeetMeWeb.Repositories.Interfaces;
using MeetMeWeb.Services;
using Microsoft.AspNet.Identity;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace MeetMeWeb.Tests.Unit_Tests
{
    [TestFixture]
    public class AccountServiceTests
    {
        Mock<AccountService> _accountServiceMock;
        Mock<IAuthRepository> _authRepoMock;

        [SetUp]
        public void Setup()
        {
            _authRepoMock = new Mock<IAuthRepository>();

            _accountServiceMock = new Mock<AccountService>(new object[] { _authRepoMock.Object })
            {
                CallBase = true
            };
        }

        #region RegisterUser

        [Test]
        public async Task RegisterUser_UserRegisteredAsync()
        {
            string password = Guid.NewGuid().ToString();

            var userModel = new UserModel
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid() + "@gmail.com",
                Password = password,
                ConfirmPassword = password
            };

            var identityResult = new IdentityResult();

            _authRepoMock.Setup(f => f.RegisterUser(It.IsAny<UserModel>(), It.IsAny<string>())).ReturnsAsync(identityResult);

            await _accountServiceMock.Object.RegisterUser(userModel, Guid.NewGuid().ToString());

            _authRepoMock.Verify(f => f.RegisterUser(It.Is<UserModel>(k => k.FirstName == userModel.FirstName &&
                                                                           k.LastName == userModel.LastName &&
                                                                           k.Email == userModel.Email &&
                                                                           k.Password == userModel.Password &&
                                                                           k.ConfirmPassword == userModel.ConfirmPassword), It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region ConfirmEmail

        [Test]
        public async Task ConfirmEmail_EmailConfirmedAsync()
        {
            var identityResult = new IdentityResult();

            _authRepoMock.Setup(f => f.ConfirmEmail(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(identityResult);

            await _accountServiceMock.Object.ConfirmEmail(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            _authRepoMock.Verify(f => f.ConfirmEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion

    }
}