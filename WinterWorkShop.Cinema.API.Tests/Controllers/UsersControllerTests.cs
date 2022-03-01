using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.API.Models.Request;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class UsersControllerTests
    {
        private Mock<IUserService> _mockUserService;
        private User _user;
        private UserDomainModel _userDomainModel;
        private UsersController usersController;

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
                Id = Guid.NewGuid()
            };

            _mockUserService = new Mock<IUserService>();
        }

        [TestMethod]
        public void UsersController_GetAsync_ReturnListOfUsers()
        {
            //Arrange
            usersController = new UsersController(_mockUserService.Object);
            var expectedResoultCount = 1;
            var expectedStatusCode = 200;
            var usersList = new List<UserDomainModel>() { _userDomainModel };
            Task<IEnumerable<UserDomainModel>> usersCollection = Task.FromResult((IEnumerable<UserDomainModel>)usersList);

            //Act
            _mockUserService.Setup(x => x.GetAllAsync()).Returns(usersCollection);
            var resultAction = usersController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)resultAction).Value;
            var usersDomainModelResult = (List<UserDomainModel>)resultList;
            //Assert
            Assert.IsNotNull(usersDomainModelResult);
            Assert.AreEqual(expectedResoultCount, usersDomainModelResult.Count);
            Assert.AreEqual(_userDomainModel.Id, usersDomainModelResult[0].Id);
            Assert.IsInstanceOfType(usersDomainModelResult[0], typeof(UserDomainModel));
            Assert.IsInstanceOfType(usersDomainModelResult, typeof(List<UserDomainModel>));
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void UsersController_GetAsync_ReturnNewList()
        {
            //Arrange
            usersController = new UsersController(_mockUserService.Object);
            var expectedResoultCount = 0;
            var expectedStatusCode = 200;
            List<UserDomainModel> usersList = null;
            Task<IEnumerable<UserDomainModel>> usersCollection = Task.FromResult((IEnumerable<UserDomainModel>)usersList);

            //Act
            _mockUserService.Setup(x => x.GetAllAsync()).Returns(usersCollection);
            var resultAction = usersController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)resultAction).Value;
            var usersDomainModelResult = (List<UserDomainModel>)resultList;
            //Assert
            Assert.IsNotNull(usersDomainModelResult);
            Assert.AreEqual(expectedResoultCount, usersDomainModelResult.Count);
            Assert.IsInstanceOfType(usersDomainModelResult, typeof(List<UserDomainModel>));
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void UsersController_GetByIdAsync_ReturnUser()
        {
            //Arrange
            usersController = new UsersController(_mockUserService.Object);
            var expectedStatusCode = 200;
            Task<UserDomainModel> user = Task.FromResult(_userDomainModel);
            //Act
            _mockUserService.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>())).Returns(user);
            var resultAction = usersController.GetbyIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var userDomainModelResult = (UserDomainModel)result;
            //Assert
            Assert.IsNotNull(userDomainModelResult);
            Assert.AreEqual(_userDomainModel.Id, userDomainModelResult.Id);
            Assert.AreEqual(_userDomainModel.FirstName, userDomainModelResult.FirstName);
            Assert.AreEqual(_userDomainModel.LastName, userDomainModelResult.LastName);
            Assert.AreEqual(_userDomainModel.UserName, userDomainModelResult.UserName);
            Assert.AreEqual(_userDomainModel.Role, userDomainModelResult.Role);
            Assert.AreEqual(_userDomainModel.BonusPoints, userDomainModelResult.BonusPoints);
            Assert.IsInstanceOfType(userDomainModelResult, typeof(UserDomainModel));
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void UsersController_GetByIdAsync_ReturnNull()
        {
            //Arrange
            usersController = new UsersController(_mockUserService.Object);
            var expectedStatusCode = 404;
            Task<UserDomainModel> user = Task.FromResult((UserDomainModel)null);
            //Act
            _mockUserService.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>())).Returns(user);
            var resultAction = usersController.GetbyIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;
            //Assert
            Assert.AreEqual(Messages.USER_NOT_FOUND, result);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void UsersController_GetByUserNameAsync_ReturnUser()
        {
            //Arrange
            usersController = new UsersController(_mockUserService.Object);
            var expectedStatusCode = 200;
            Task<UserDomainModel> user = Task.FromResult(_userDomainModel);
            //Act
            _mockUserService.Setup(x => x.GetUserByUserName(It.IsAny<string>())).Returns(user);
            var resultAction = usersController.GetbyUserNameAsync(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var userDomainModelResult = (UserDomainModel)result;
            //Assert
            Assert.IsNotNull(userDomainModelResult);
            Assert.AreEqual(_userDomainModel.Id, userDomainModelResult.Id);
            Assert.AreEqual(_userDomainModel.FirstName, userDomainModelResult.FirstName);
            Assert.AreEqual(_userDomainModel.LastName, userDomainModelResult.LastName);
            Assert.AreEqual(_userDomainModel.UserName, userDomainModelResult.UserName);
            Assert.AreEqual(_userDomainModel.Role, userDomainModelResult.Role);
            Assert.AreEqual(_userDomainModel.BonusPoints, userDomainModelResult.BonusPoints);
            Assert.IsInstanceOfType(userDomainModelResult, typeof(UserDomainModel));
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void UsersController_GetByUserNameAsync_ReturnNull()
        {
            //Arrange
            usersController = new UsersController(_mockUserService.Object);
            var expectedStatusCode = 404;
            Task<UserDomainModel> user = Task.FromResult((UserDomainModel)null);
            //Act
            _mockUserService.Setup(x => x.GetUserByUserName(It.IsAny<string>())).Returns(user);
            var resultAction = usersController.GetbyUserNameAsync(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;
            var errorModel = (ErrorResponseModel)result;
            //Assert
            Assert.AreEqual(Messages.USER_NOT_FOUND, errorModel.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)resultAction).StatusCode);
        }


        [TestMethod]
        public void UsersController_CreateUser_ReturnUser()
        {
            //Arrange
            usersController = new UsersController(_mockUserService.Object);
            var createUserModel = new CreateUserModel()
            {
                firstName = "M",
                lastName = "T",
                role = "admin",
                userName = "m.t"
            };

            var expectedStatusCode = 201;
            var responseModel = new ResponseModel<UserDomainModel>()
            {
                DomainModel = _userDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };

            Task<ResponseModel<UserDomainModel>> responseModelAsync = Task.FromResult((ResponseModel<UserDomainModel>)responseModel);

            //Act
            _mockUserService.Setup(x => x.AddUser(It.IsAny<UserDomainModel>())).Returns(responseModelAsync);
            var resultAction = usersController.CreateUser(createUserModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((CreatedResult)resultAction).Value;
            var userResponseModel = (UserDomainModel)result;
            //Assert
            Assert.IsNotNull(userResponseModel);
            Assert.AreEqual(responseModel.DomainModel.FirstName, userResponseModel.FirstName);
            Assert.AreEqual(responseModel.DomainModel.LastName, userResponseModel.LastName);
            Assert.AreEqual(responseModel.DomainModel.UserName, userResponseModel.UserName);
            Assert.AreEqual(responseModel.DomainModel.Role, userResponseModel.Role);
            Assert.AreEqual(responseModel.DomainModel.BonusPoints, userResponseModel.BonusPoints);
            Assert.IsInstanceOfType(userResponseModel, typeof(UserDomainModel));
            Assert.IsInstanceOfType(resultAction, typeof(CreatedResult));
            Assert.AreEqual(expectedStatusCode, ((CreatedResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void UsersController_CreateUser_ReturnCreationError()
        {
            //Arrange
            usersController = new UsersController(_mockUserService.Object);
            var createUserModel = new CreateUserModel()
            {
                firstName = "M",
                lastName = "T",
                role = "admin",
                userName = "m.t"
            };
            var expectedStatusCode = 500;
            var responseModel = new ResponseModel<UserDomainModel>()
            {
                DomainModel = null,
                ErrorMessage = Messages.USER_CREATION_ERROR,
                IsSuccessful = false
            };
            Task<ResponseModel<UserDomainModel>> responseModelAsync = Task.FromResult((ResponseModel<UserDomainModel>)responseModel);

            //Act
            _mockUserService.Setup(x => x.AddUser(It.IsAny<UserDomainModel>())).Returns(responseModelAsync);
            var resultAction = usersController.CreateUser(createUserModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((ObjectResult)resultAction).Value;
            var errorModel = ((ErrorResponseModel)result);

            //Assert
            Assert.AreEqual(Messages.USER_CREATION_ERROR, errorModel.ErrorMessage);
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void UsersController_CreateUser_ReturnAlreadyExistUserError()
        {
            //Arrange
            usersController = new UsersController(_mockUserService.Object);
            var createUserModel = new CreateUserModel()
            {
                firstName = "M",
                lastName = "T",
                role = "admin",
                userName = "m.t"
            };
            var expectedStatusCode = 500;
            var responseModel = new ResponseModel<UserDomainModel>()
            {
                DomainModel = null,
                ErrorMessage = Messages.USER_ALREADY_EXISTS,
                IsSuccessful = false
            };
            Task<ResponseModel<UserDomainModel>> responseModelAsync = Task.FromResult((ResponseModel<UserDomainModel>)responseModel);

            //Act
            _mockUserService.Setup(x => x.AddUser(It.IsAny<UserDomainModel>())).Returns(responseModelAsync);
            var resultAction = usersController.CreateUser(createUserModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((ObjectResult)resultAction).Value;
            var errorModel = ((ErrorResponseModel)result);

            //Assert
            Assert.AreEqual(Messages.USER_ALREADY_EXISTS, errorModel.ErrorMessage);
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void UsersController_CreateUser_ReturnBadRequestForDbAccess()
        {
            // Arrange
            usersController = new UsersController(_mockUserService.Object);
            var expectedStatusCode = 400;
            var createUserModel = new CreateUserModel()
            {
                firstName = "M",
                lastName = "T",
                role = "admin",
                userName = "m.t"
            };
            Exception exception = new Exception(Messages.USER_CREATION_ERROR);
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            //Act
            _mockUserService.Setup(x => x.AddUser(It.IsAny<UserDomainModel>())).Throws(dbUpdateException);
            var resultAction = usersController.CreateUser(createUserModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((BadRequestObjectResult)resultAction).Value;
            var errorModel = ((ErrorResponseModel)result);
            //Assert
            Assert.AreEqual(expectedStatusCode, ((BadRequestObjectResult)resultAction).StatusCode);
            Assert.AreEqual(exception.Message, errorModel.ErrorMessage);

        }

    }
}
