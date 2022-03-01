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
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class CinemasControllerTests
    {
        private Mock<ICinemaService> _mockCinemasService;
        private Data.Cinema _cinema;
        private CinemaDomainModel _cinemaDomainModel;
        private CinemasController cinemasController;

        [TestInitialize]
        public void TestInitialize()
        {
            _cinema = new Data.Cinema()
            {
                Id = 1,
                Name = "Cinestar",
                Auditoriums = new List<Auditorium>() { }
            };

            _cinemaDomainModel = new CinemaDomainModel()
            {
                Id = _cinema.Id,
                Name = "Cinestar"
            };

            _mockCinemasService = new Mock<ICinemaService>();
        }


        [TestMethod]
        public void CinemasController_GetAsync_ReturnCinemas()
        {
            //Arrange
            cinemasController = new CinemasController(_mockCinemasService.Object);
            var expectedResoultCount = 1;
            var expectedStatusCode = 200;
            var cinemasList = new List<CinemaDomainModel>() { _cinemaDomainModel };
            Task<IEnumerable<CinemaDomainModel>> cinemasCollection = Task.FromResult((IEnumerable<CinemaDomainModel>)cinemasList);

            //Act
            _mockCinemasService.Setup(x => x.GetAllAsync()).Returns(cinemasCollection);
            var resultAction = cinemasController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)resultAction).Value;
            var cinemasDomainModelsResult = (List<CinemaDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(cinemasDomainModelsResult);
            Assert.AreEqual(expectedResoultCount, cinemasDomainModelsResult.Count);
            Assert.AreEqual(_cinemaDomainModel.Id, cinemasDomainModelsResult[0].Id);
            Assert.AreEqual(_cinemaDomainModel.Name, cinemasDomainModelsResult[0].Name);
            Assert.IsInstanceOfType(cinemasDomainModelsResult[0], typeof(CinemaDomainModel));
            Assert.IsInstanceOfType(cinemasDomainModelsResult, typeof(List<CinemaDomainModel>));
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void CinemasController_GetAsync_ReturnEmptyList()
        {
            //Arrange
            cinemasController = new CinemasController(_mockCinemasService.Object);
            var expectedResoultCount = 0;
            var expectedStatusCode = 200;
            List<CinemaDomainModel> cinemasList = null;
            Task<IEnumerable<CinemaDomainModel>> cinemasCollection = Task.FromResult((IEnumerable<CinemaDomainModel>)cinemasList);

            //Act
            _mockCinemasService.Setup(x => x.GetAllAsync()).Returns(cinemasCollection);
            var resultAction = cinemasController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)resultAction).Value;
            var cinemasDomainModelsResult = (List<CinemaDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(cinemasDomainModelsResult);
            Assert.AreEqual(expectedResoultCount, cinemasDomainModelsResult.Count);
            Assert.IsInstanceOfType(cinemasDomainModelsResult, typeof(List<CinemaDomainModel>));
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void CinemasController_GetByIdAsync_ReturnCinema()
        {
            //Arrange
            cinemasController = new CinemasController(_mockCinemasService.Object);
            var expectedStatusCode = 200;
            Task<CinemaDomainModel> cinema = Task.FromResult((CinemaDomainModel)_cinemaDomainModel);

            //Act
            _mockCinemasService.Setup(x => x.GetCinemaByIdAsync(It.IsAny<int>())).Returns(cinema);
            var resultAction = cinemasController.GetByIdAsync(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)resultAction).Value;
            var cinemasDomainModelResult = (CinemaDomainModel)resultList;

            //Assert
            Assert.IsNotNull(cinemasDomainModelResult);
            Assert.AreEqual(_cinemaDomainModel.Id, cinemasDomainModelResult.Id);
            Assert.AreEqual(_cinemaDomainModel.Name, cinemasDomainModelResult.Name);
            Assert.IsInstanceOfType(cinemasDomainModelResult, typeof(CinemaDomainModel));
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void CinemasController_GetByIdAsync_ReturnErrorMessage()
        {
            //Arrange
            cinemasController = new CinemasController(_mockCinemasService.Object);
            var expectedStatusCode = 404;
            var expectedMessage = Messages.CINEMA_DOES_NOT_EXIST;
            Task<CinemaDomainModel> cinema = Task.FromResult((CinemaDomainModel)null);

            //Act
            _mockCinemasService.Setup(x => x.GetCinemaByIdAsync(It.IsAny<int>())).Returns(cinema);
            var resultAction = cinemasController.GetByIdAsync(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;
            var errorMessage = (string)result;

            //Assert
            Assert.AreEqual(expectedMessage, errorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void CinemasController_Delete_ReturnDeletedCinema()
        {
            //Arrange
            cinemasController = new CinemasController(_mockCinemasService.Object);
            var expectedStatusCode = 202;
            Task<CinemaDomainModel> cinema = Task.FromResult((CinemaDomainModel)_cinemaDomainModel);

            //Act
            _mockCinemasService.Setup(x => x.DeleteCinema(It.IsAny<int>())).Returns(cinema);
            var resultAction = cinemasController.Delete(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((AcceptedResult)resultAction).Value;
            var model = (CinemaDomainModel)result;

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_cinemaDomainModel.Id, model.Id);
            Assert.AreEqual(_cinemaDomainModel.Name, model.Name);
            Assert.IsInstanceOfType(resultAction, typeof(AcceptedResult));
            Assert.AreEqual(expectedStatusCode, ((AcceptedResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void CinemasController_Delete_ReturnErrorResponse()
        {
            //Arrange
            cinemasController = new CinemasController(_mockCinemasService.Object);
            var expectedStatusCode = 500;
            var expectedMessage = Messages.CINEMA_DOES_NOT_EXIST;
            Task<CinemaDomainModel> cinema = Task.FromResult((CinemaDomainModel)null);

            //Act
            _mockCinemasService.Setup(x => x.DeleteCinema(It.IsAny<int>())).Returns(cinema);
            var resultAction = cinemasController.Delete(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((ObjectResult)resultAction).Value;
            var errorMessage = (ErrorResponseModel)result;
            //Assert
            Assert.AreEqual(expectedMessage, errorMessage.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, (int)errorMessage.StatusCode);
        }

        [TestMethod]
        public void CinemasController_Delete_ReturnDbUpdateException()
        {
            //Arrange
            cinemasController = new CinemasController(_mockCinemasService.Object);
            var expectedStatusCode = 400;
            var expectedMessage = Messages.CINEMA_DOES_NOT_EXIST;
            Task<CinemaDomainModel> cinema = Task.FromResult((CinemaDomainModel)null);
            Exception exception = new Exception(Messages.CINEMA_DOES_NOT_EXIST);
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            //Act
            _mockCinemasService.Setup(x => x.DeleteCinema(It.IsAny<int>())).Throws(dbUpdateException);
            var resultAction = cinemasController.Delete(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((BadRequestObjectResult)resultAction).Value;
            var errorMessage = (ErrorResponseModel)result;
            //Assert
            Assert.AreEqual(expectedMessage, errorMessage.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, (int)errorMessage.StatusCode);
        }

        [TestMethod]
        public void CinemasController_Delete_ReturnArgumentNullException()
        {
            //Arrange
            cinemasController = new CinemasController(_mockCinemasService.Object);
            var expectedStatusCode = 400;
            var expectedMessage = Messages.CINEMA_DOES_NOT_EXIST;
            Task<CinemaDomainModel> cinema = Task.FromResult((CinemaDomainModel)null);
            Exception exception = new Exception(Messages.CINEMA_DOES_NOT_EXIST);
            ArgumentNullException dbUpdateException = new ArgumentNullException("Error.", exception);

            //Act
            _mockCinemasService.Setup(x => x.DeleteCinema(It.IsAny<int>())).Throws(dbUpdateException);
            var resultAction = cinemasController.Delete(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((BadRequestObjectResult)resultAction).Value;
            var errorMessage = (ErrorResponseModel)result;
            //Assert
            Assert.AreEqual(expectedMessage, errorMessage.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, (int)errorMessage.StatusCode);
        }

        [TestMethod]
        public void CinemasController_Post_ReturnModelStateError()
        {
            //Arrange
            cinemasController = new CinemasController(_mockCinemasService.Object);
            var expectedStatusCode = 400;
            cinemasController.ModelState.AddModelError("test", "test");

            //Act
            var resultAction = cinemasController.Post(new CinemaWithAuditoriumModel() { Name = "Cinemania" }).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((BadRequestObjectResult)resultAction).Value;

            //Assert
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, ((BadRequestObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void CinemasController_Post_ReturnCreatedCinema()
        {
            //Arrange
            cinemasController = new CinemasController(_mockCinemasService.Object);
            var expectedStatusCode = 201;
            var cinemaWithAuditoriumModel = new CinemaWithAuditoriumModel()
            {
                Name = "Cinemania"
            };

            var responseModel = new CreateCinemaResultModel()
            {
                ErrorMessage = null,
                IsSuccessful = true,
                Cinema = _cinemaDomainModel
            };
            Task<CreateCinemaResultModel> model = Task.FromResult(responseModel);

            //Act
            _mockCinemasService.Setup(x => x.AddCinema(It.IsAny<CinemaDomainModel>())).Returns(model);
            var resultAction = cinemasController.Post(cinemaWithAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((CreatedResult)resultAction).Value;
            var createdCinemaModel = (CinemaDomainModel)result;   

            //Assert
            Assert.IsInstanceOfType(resultAction, typeof(CreatedResult));
            Assert.AreEqual(_cinemaDomainModel.Id, createdCinemaModel.Id);
            Assert.AreEqual(_cinemaDomainModel.Name, createdCinemaModel.Name);
            Assert.AreEqual(expectedStatusCode, ((CreatedResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void CinemasController_Post_ReturnErrorResponseModel()
        {
            //Arrange
            cinemasController = new CinemasController(_mockCinemasService.Object);
            var expectedStatusCode = 400;
            var cinemaWithAuditoriumModel = new CinemaWithAuditoriumModel()
            {
                Name = "Cinemania"
            };

            var responseModel = new CreateCinemaResultModel()
            {
                ErrorMessage = Messages.CINEMA_CREATION_ERROR,
                IsSuccessful = false,
                Cinema = _cinemaDomainModel
            };
            Task<CreateCinemaResultModel> model = Task.FromResult(responseModel);

            //Act
            _mockCinemasService.Setup(x => x.AddCinema(It.IsAny<CinemaDomainModel>())).Returns(model);
            var resultAction = cinemasController.Post(cinemaWithAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((BadRequestObjectResult)resultAction).Value;
            var errorMessage = (ErrorResponseModel)result;

            //Assert
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.IsNotNull(errorMessage.ErrorMessage);
            Assert.AreEqual(Messages.CINEMA_CREATION_ERROR, errorMessage.ErrorMessage);
            Assert.AreEqual(expectedStatusCode, ((BadRequestObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void CinemasController_Post_ReturnDbUpdateException()
        {
            //Arrange
            cinemasController = new CinemasController(_mockCinemasService.Object);
            var cinemaWithAuditoriumModel = new CinemaWithAuditoriumModel()
            {
                Name = "Cinemania"
            };
            var expectedStatusCode = 400;
            var expectedMessage = Messages.CINEMA_CREATION_ERROR;
            Task<CinemaDomainModel> cinema = Task.FromResult((CinemaDomainModel)null);
            Exception exception = new Exception(Messages.CINEMA_CREATION_ERROR);
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            //Act
            _mockCinemasService.Setup(x => x.AddCinema(It.IsAny<CinemaDomainModel>())).Throws(dbUpdateException);
            var resultAction = cinemasController.Post(cinemaWithAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((BadRequestObjectResult)resultAction).Value;
            var errorMessage = (ErrorResponseModel)result;
            //Assert
            Assert.AreEqual(expectedMessage, errorMessage.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, (int)errorMessage.StatusCode);
        }

        [TestMethod]
        public void CinemasController_Put_ReturnModelStateError()
        {
            //Arrange
            cinemasController = new CinemasController(_mockCinemasService.Object);
            var expectedStatusCode = 400;
            cinemasController.ModelState.AddModelError("test", "test");

            //Act
            var resultAction = cinemasController.Put(It.IsAny<int>(), new CreateCinemaModel() { Name = "Cinemania" }).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((BadRequestObjectResult)resultAction).Value;

            //Assert
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, ((BadRequestObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void CinemasController_Put_ReturnUpdatedCinema()
        {
            //Arrange
            cinemasController = new CinemasController(_mockCinemasService.Object);
            var expectedStatusCode = 202;
            Task<CinemaDomainModel> cinema = Task.FromResult((CinemaDomainModel)_cinemaDomainModel);
            var updatedCinema = new CinemaDomainModel()
            {
                Id = _cinemaDomainModel.Id,
                Name = "SuperStar"
            };
            Task<CinemaDomainModel> updated = Task.FromResult((CinemaDomainModel)updatedCinema);

            //Act
            _mockCinemasService.Setup(x => x.GetCinemaByIdAsync(It.IsAny<int>())).Returns(cinema);
            _mockCinemasService.Setup(x => x.UpdateCinema(It.IsAny<CinemaDomainModel>())).Returns(updated);
            var resultAction = cinemasController.Put(It.IsAny<int>(), new CreateCinemaModel() { Name = "SuperStar" }).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((AcceptedResult)resultAction).Value;
            var cinemaDomainModelResult = (CinemaDomainModel)result;

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(updatedCinema.Id, cinemaDomainModelResult.Id);
            Assert.AreEqual(updatedCinema.Name, cinemaDomainModelResult.Name);
            Assert.IsInstanceOfType(resultAction, typeof(AcceptedResult));
            Assert.AreEqual(expectedStatusCode, ((AcceptedResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void CinemasController_Put_ReturnCinemaDoesntExistsError()
        {
            //Arrange
            cinemasController = new CinemasController(_mockCinemasService.Object);
            var expectedStatusCode = 400;
            var expectedMessage = Messages.CINEMA_DOES_NOT_EXIST;
            Task<CinemaDomainModel> cinema = Task.FromResult((CinemaDomainModel)null);

            //Act
            _mockCinemasService.Setup(x => x.GetCinemaByIdAsync(It.IsAny<int>())).Returns(cinema);
            var resultAction = cinemasController.Put(It.IsAny<int>(), new CreateCinemaModel() { Name = "SuperStar" }).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((BadRequestObjectResult)resultAction).Value;
            var errorResponseModel = (ErrorResponseModel)result;

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, errorResponseModel.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, ((BadRequestObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void CinemasController_Put_ReturnUpdateDbException()
        {
            //Arrange
            cinemasController = new CinemasController(_mockCinemasService.Object);
            var expectedStatusCode = 400;
            Task<CinemaDomainModel> cinema = Task.FromResult((CinemaDomainModel)_cinemaDomainModel);
            Exception exception = new Exception(Messages.CINEMA_DOES_NOT_EXIST);
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            //Act
            _mockCinemasService.Setup(x => x.GetCinemaByIdAsync(It.IsAny<int>())).Returns(cinema);
            _mockCinemasService.Setup(x => x.UpdateCinema(It.IsAny<CinemaDomainModel>())).Throws(dbUpdateException);
            var resultAction = cinemasController.Put(It.IsAny<int>(), new CreateCinemaModel() { Name = "SuperStar" }).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((BadRequestObjectResult)resultAction).Value;
            var errorMessage = (ErrorResponseModel)result;
            //Assert
            Assert.AreEqual(dbUpdateException.InnerException.Message.ToString(), errorMessage.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, (int)errorMessage.StatusCode);
        }
    }
}
