using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class UsersServiceTests
    {
        private Mock<IUsersRepository> _mockUsersRepository;
        private User _user;
        private UserDomainModel _userDomainModel;
        private UserService userService;

        [TestInitialize]
        public void TestInitialize()
        {
            _user = new User()
            {
                FirstName = "Marko",
                LastName = "Tatic",
                UserName = "m.tatic",
                BonusPoints = 0,
                Role = "admin",
                Id = Guid.NewGuid()
            };

            _userDomainModel = new UserDomainModel()
            {
                FirstName = "Marko",
                LastName = "Tatic",
                UserName = "m.tatic",
                BonusPoints = 0,
                Role = "admin",
                Id = _user.Id
            };

            _mockUsersRepository = new Mock<IUsersRepository>();
        }

        [TestMethod]
        public void UsersService_GetAllAsync_ReturnListOfUsers()
        {
            //Arrange
            userService = new UserService(_mockUsersRepository.Object);
            var expectedResoultCount = 1;

            var usersList = new List<User>() { _user };
            Task<IEnumerable<User>> usersCollection = Task.FromResult((IEnumerable<User>)usersList);

            //Act
            _mockUsersRepository.Setup(x => x.GetAll()).Returns(usersCollection);
            var resultAction = userService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<UserDomainModel>)resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResoultCount, result.Count);
            Assert.AreEqual(_user.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(UserDomainModel));
        }

        [TestMethod]
        public void UsersService_GetAllAsync_ReturnNull()
        {
            //Arrange
            userService = new UserService(_mockUsersRepository.Object);
            List<User> usersList = null;
            Task<IEnumerable<User>> usersCollection = Task.FromResult((IEnumerable<User>)usersList);
            //Act
            _mockUsersRepository.Setup(x => x.GetAll()).Returns(usersCollection);
            var resultAction = userService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<UserDomainModel>)resultAction;
            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void UsersService_GetAllAsync_ReturnNoUsersInCollection()
        {
            //Arrange
            userService = new UserService(_mockUsersRepository.Object);
            var expectedResoultCount = 0;
            var usersList = new List<User>() { };
            Task<IEnumerable<User>> usersCollection = Task.FromResult((IEnumerable<User>)usersList);

            //Act
            _mockUsersRepository.Setup(x => x.GetAll()).Returns(usersCollection);
            var resultAction = userService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<UserDomainModel>)resultAction;
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResoultCount, result.Count);
            Assert.IsInstanceOfType(result, typeof(List<UserDomainModel>));
        }

        [TestMethod]
        public void UsersService_GetUserByIdAsync_RetrunUser()
        {
            //Arrange
            userService = new UserService(_mockUsersRepository.Object);
            Task<User> user = Task.FromResult((User)_user);

            //Act
            _mockUsersRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(user);
            var result = userService.GetUserByIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_user.Id, result.Id);
            Assert.AreEqual(_user.FirstName, result.FirstName);
            Assert.AreEqual(_user.LastName, result.LastName);
            Assert.AreEqual(_user.UserName, result.UserName);
            Assert.AreEqual(_user.Role, result.Role);
            Assert.AreEqual(_user.BonusPoints, result.BonusPoints);
            Assert.IsInstanceOfType(result, typeof(UserDomainModel));
        }

        [TestMethod]
        public void UsersService_GetUserByIdAsync_ReturnNull()
        {
            //Arrange
            userService = new UserService(_mockUsersRepository.Object);
            Task<User> user = Task.FromResult((User)null);

            //Act
            _mockUsersRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(user);
            var result = userService.GetUserByIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void UsersService_GetUserByUserName_ReturnUser()
        {
            //Arrange
            userService = new UserService(_mockUsersRepository.Object);
            //Act
            _mockUsersRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(_user);
            var result = userService.GetUserByUserName(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_user.Id, result.Id);
            Assert.AreEqual(_user.FirstName, result.FirstName);
            Assert.AreEqual(_user.LastName, result.LastName);
            Assert.AreEqual(_user.UserName, result.UserName);
            Assert.AreEqual(_user.Role, result.Role);
            Assert.AreEqual(_user.BonusPoints, result.BonusPoints);
            Assert.IsInstanceOfType(result, typeof(UserDomainModel));
        }

        [TestMethod]
        public void UsersService_GetUserByUserName_ReturnNull()
        {
            //Arrange
            userService = new UserService(_mockUsersRepository.Object);
            User user = null;

            //Act
            _mockUsersRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(user);
            var result = userService.GetUserByUserName(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void UsersService_AddUser_ReturnNewUser()
        {
            //Arrange
            userService = new UserService(_mockUsersRepository.Object);
            User user = null;

            //Act
            _mockUsersRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(user);
            _mockUsersRepository.Setup(x => x.Insert(It.IsAny<User>())).Returns(_user);
            _mockUsersRepository.Setup(x => x.Save());

            var resultAction = userService.AddUser(_userDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
            Assert.AreEqual(_userDomainModel.Id, resultAction.DomainModel.Id);
            Assert.AreEqual(_userDomainModel.FirstName, resultAction.DomainModel.FirstName);
            Assert.AreEqual(_userDomainModel.LastName, resultAction.DomainModel.LastName);
            Assert.AreEqual(_userDomainModel.UserName, resultAction.DomainModel.UserName);
            Assert.AreEqual(_userDomainModel.Role, resultAction.DomainModel.Role);
            Assert.AreEqual(_userDomainModel.BonusPoints, resultAction.DomainModel.BonusPoints);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<UserDomainModel>));
        }

        [TestMethod]
        public void UsersService_AddUser_Unknown_User_Role_Return_ErrorMessage()
        {
            //Arrange
            userService = new UserService(_mockUsersRepository.Object);
            User user = new User() {};
            UserDomainModel userDomainModel = new UserDomainModel()
            {
                BonusPoints = _userDomainModel.BonusPoints,
                Role = "verified_user",
                FirstName = _userDomainModel.FirstName,
                Id = _userDomainModel.Id,
                LastName = _userDomainModel.LastName,
                UserName = _userDomainModel.UserName
            };

            //Act
            _mockUsersRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(user);
            _mockUsersRepository.Setup(x => x.Insert(It.IsAny<User>())).Returns(_user);
            _mockUsersRepository.Setup(x => x.Save());

            var resultAction = userService.AddUser(userDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.DomainModel);
            Assert.AreEqual(Messages.UNKNOWN_USER_ROLE, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<UserDomainModel>));
        }

        [TestMethod]
        public void UsersService_AddUser_User_With_That_UserName_Already_Exists_Return_ErrorMessage()
        {
            //Arrange
            userService = new UserService(_mockUsersRepository.Object);
            User user = new User()
            {
                UserName = "m.tatic"
            };

            //Act
            _mockUsersRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(user);
            _mockUsersRepository.Setup(x => x.Insert(It.IsAny<User>())).Returns(_user);
            _mockUsersRepository.Setup(x => x.Save());

            var resultAction = userService.AddUser(_userDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.DomainModel);
            Assert.AreEqual(Messages.USER_ALREADY_EXISTS, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<UserDomainModel>));
        }

        [TestMethod]
        public void UsersService_AddUser_UserCreationError_Return_ErrorMessage()
        {
            //Arrange
            userService = new UserService(_mockUsersRepository.Object);
            User user = null;
            //Act
            _mockUsersRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(user);
            _mockUsersRepository.Setup(x => x.Insert(It.IsAny<User>())).Returns(user);
            _mockUsersRepository.Setup(x => x.Save());

            var resultAction = userService.AddUser(_userDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.DomainModel);
            Assert.AreEqual(Messages.USER_CREATION_ERROR, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<UserDomainModel>));
        }

        [TestMethod]
        public void UsersService_AddBonusPoints_ReturnPoints()
        {
            //Arrange
            userService = new UserService(_mockUsersRepository.Object);
            var expectedBounsPoints = 10;
            //Act
            _mockUsersRepository.Setup(x => x.AddBonusPoints(It.IsAny<Guid>(), It.IsAny<int>())).Returns(10);
            _mockUsersRepository.Setup(x => x.Save());
            var resultAction = userService.AddBonusPoints(It.IsAny<Guid>(), It.IsAny<int>());

            //Assert
            Assert.AreEqual(expectedBounsPoints, resultAction);
            Assert.IsInstanceOfType(resultAction, typeof(int));

        }

        [TestMethod]
        public void UsersService_AddBonusPoints_ReturnErrorPoints()
        {
            //Arrange
            userService = new UserService(_mockUsersRepository.Object);
            int points = -1;
            //Act
            _mockUsersRepository.Setup(x => x.AddBonusPoints(It.IsAny<Guid>(), It.IsAny<int>())).Returns(points);
            _mockUsersRepository.Setup(x => x.Save());
            var resultAction = userService.AddBonusPoints(It.IsAny<Guid>(), It.IsAny<int>());

            //Assert
            Assert.AreEqual(-1, resultAction);
            Assert.IsInstanceOfType(resultAction, typeof(int));

        }
    }
}
