using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class AuditoriumsControllerTest
    {
        private Mock<IAuditoriumService> _mockAuditoriumService;
        private AuditoriumDomainModel _auditoriumDomainModel;
        private CreateAuditoriumModel _createAuditoriumModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _auditoriumDomainModel = new AuditoriumDomainModel
            {
                Id = 1,
                CinemaId = 1,
                Name = "Auditorium"
            };

            _createAuditoriumModel = new CreateAuditoriumModel
            {
                cinemaId = 1,
                auditName = "Auditorium",
                seatRows = 1,
                numberOfSeats = 1
            };

            List<AuditoriumDomainModel> auditoriumList = new List<AuditoriumDomainModel>();
            auditoriumList.Add(_auditoriumDomainModel);
            IEnumerable<AuditoriumDomainModel> auditoriums = auditoriumList;
            Task<IEnumerable<AuditoriumDomainModel>> responseTask = Task.FromResult(auditoriums);

            _mockAuditoriumService = new Mock<IAuditoriumService>();

        }

        [TestMethod]
        public void AuditoriumsController_GetAllAsync_ReturnsListOfAuditoriums()
        {
            //Arrange
            List<AuditoriumDomainModel> auditoriumList = new List<AuditoriumDomainModel>();
            auditoriumList.Add(_auditoriumDomainModel);
            IEnumerable<AuditoriumDomainModel> auditoriums = auditoriumList;
            Task<IEnumerable<AuditoriumDomainModel>> responseTask = Task.FromResult(auditoriums);
            int expectedCount = 1;
            int expectedStatusCode = 200;

            AuditoriumsController auditoriumController = new AuditoriumsController(_mockAuditoriumService.Object);
            _mockAuditoriumService.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = auditoriumController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var auditoriumsDomainModelResultList = (List<AuditoriumDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(resultList);
            Assert.AreEqual(expectedCount, auditoriumsDomainModelResultList.Count);
            Assert.IsInstanceOfType(auditoriumsDomainModelResultList[0], typeof(AuditoriumDomainModel));
            Assert.AreEqual(auditoriumsDomainModelResultList[0].Id, _auditoriumDomainModel.Id);
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_GetAllAsync_ReturnNull()
        {
            //Arrange
            IEnumerable<AuditoriumDomainModel> auditoriums = null;
            Task<IEnumerable<AuditoriumDomainModel>> responseTask = Task.FromResult(auditoriums);
            int expectedCount = 0;
            int expectedStatusCode = 200;

            _mockAuditoriumService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            AuditoriumsController auditoriumController = new AuditoriumsController(_mockAuditoriumService.Object);

            //Act
            var result = auditoriumController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var auditoriumsDomainModelResultList = (List<AuditoriumDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, auditoriumsDomainModelResultList.Count);
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_GetByCinemaId_ReturnsListOfAuditoriums()
        {
            //Arrange
            List<AuditoriumDomainModel> auditoriumsList = new List<AuditoriumDomainModel>();
            auditoriumsList.Add(_auditoriumDomainModel);
            IEnumerable<AuditoriumDomainModel> auditoriums = auditoriumsList;
            Task<IEnumerable<AuditoriumDomainModel>> responseTask = Task.FromResult(auditoriums);
            int expectedCount = 1;
            int expectedStatusCode = 200;

            AuditoriumsController auditoriumController = new AuditoriumsController(_mockAuditoriumService.Object);
            _mockAuditoriumService.Setup(x => x.GetAuditoriumByCinemaId(It.IsAny<int>())).Returns(responseTask);

            //Act
            var result = auditoriumController.GetByCinemaId(_auditoriumDomainModel.CinemaId).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultModel = ((OkObjectResult)result);
            var resultList = ((OkObjectResult)result).Value;
            var auditoriumsDomainModelResultList = (List<AuditoriumDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, auditoriumsDomainModelResultList.Count);
            Assert.IsInstanceOfType(auditoriumsDomainModelResultList[0], typeof(AuditoriumDomainModel));
            Assert.AreEqual(((OkObjectResult)result).StatusCode, expectedStatusCode);
            Assert.AreEqual(auditoriumsDomainModelResultList[0].Id, _auditoriumDomainModel.Id);
            Assert.AreEqual(auditoriumsDomainModelResultList[0].CinemaId, _auditoriumDomainModel.CinemaId);
        }

        [TestMethod]
        public void AuditoriumsController_GetByCinemaId_ReturnNull()
        {
            //Arrange
            IEnumerable<AuditoriumDomainModel> auditoriums = null;
            Task<IEnumerable<AuditoriumDomainModel>> responseTask = Task.FromResult(auditoriums);
            int expectedCount = 0;
            int expectedStatusCode = 200;

            AuditoriumsController auditoriumController = new AuditoriumsController(_mockAuditoriumService.Object);
            _mockAuditoriumService.Setup(x => x.GetAuditoriumByCinemaId(It.IsAny<int>())).Returns(responseTask);

            //Act
            var result = auditoriumController.GetByCinemaId(_auditoriumDomainModel.CinemaId).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var auditoriumDomainModelResultList = (List<AuditoriumDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(resultList);
            Assert.AreEqual(auditoriumDomainModelResultList.Count, expectedCount);
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_PostAsync_ReturnsInsertedAuditorium()
        {
            int expectedStatusCode = 201;

            CreateAuditoriumModel createAuditoriumModel = new CreateAuditoriumModel()
            {
                cinemaId = 1,
                auditName = "Auditorium",
                seatRows = 1,
                numberOfSeats = 1
            };
            CreateAuditoriumResultModel createAuditoriumResultModel = new CreateAuditoriumResultModel
            {
                ErrorMessage = null,
                Auditorium = new AuditoriumDomainModel
                {
                    Id = 1,
                    CinemaId = createAuditoriumModel.cinemaId,
                    Name = "Auditorium"
                },
                IsSuccessful = true
            };
            Task<CreateAuditoriumResultModel> responseTask = Task.FromResult(createAuditoriumResultModel);

            _mockAuditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(responseTask);
            AuditoriumsController auditoriumController = new AuditoriumsController(_mockAuditoriumService.Object);

            //Act
            var result = auditoriumController.PostAsync(createAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var createdResult = ((CreatedResult)result).Value;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            Assert.AreEqual(expectedStatusCode, ((CreatedResult)result).StatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_PostAsync_ThrowsDbUpdateException()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            AuditoriumsController auditoriumController = new AuditoriumsController(_mockAuditoriumService.Object);

            _mockAuditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Throws(dbUpdateException);

            //Act
            var result = auditoriumController.PostAsync(_createAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = ((BadRequestObjectResult)result);
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_PostAsync_ReturnNull()
        {
            //Arrange
            CreateAuditoriumModel createAuditoriumModel = new CreateAuditoriumModel()
            {
                cinemaId = 1,
                auditName = "Auditorium",
                seatRows = 1,
                numberOfSeats = 1
            };
            CreateAuditoriumResultModel createAuditoriumResultModel = new CreateAuditoriumResultModel
            {
                Auditorium = new AuditoriumDomainModel
                {
                    Id = 1,
                    CinemaId = createAuditoriumModel.cinemaId,
                    Name = "Auditorium"
                },
                IsSuccessful = false,
                ErrorMessage = Messages.AUDITORIUM_CREATION_ERROR

            };
            Task<CreateAuditoriumResultModel> nullTask = Task.FromResult(createAuditoriumResultModel);
            int expectedStatusCode = 400;
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            errorResponseModel.StatusCode = System.Net.HttpStatusCode.InternalServerError;

            AuditoriumsController auditoriumController = new AuditoriumsController(_mockAuditoriumService.Object);
            _mockAuditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(nullTask);

            //Act
            var result = auditoriumController.PostAsync(_createAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            ObjectResult viewResult = (ObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(badObjectResult.ToString(), errorResponseModel.ToString());
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(viewResult.StatusCode, expectedStatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_Delete_ReturnsDeletedAuditorium()
        {
            //Arrange
            Task<AuditoriumDomainModel> auditorium = Task.FromResult(_auditoriumDomainModel);
            AuditoriumsController auditoriumController = new AuditoriumsController(_mockAuditoriumService.Object);
            int expectedStatusCode = 202;

            _mockAuditoriumService.Setup(x => x.DeleteAuditorium(It.IsAny<int>())).Returns(auditorium);

            //Act
            var result = auditoriumController.Delete(_auditoriumDomainModel.Id).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultModel = ((AcceptedResult)result);
            var resultList = ((AcceptedResult)result).Value;
            var auditoriumDomainModelResult = (AuditoriumDomainModel)resultList;

            //Assert
            Assert.IsNotNull(auditoriumDomainModelResult);
            Assert.IsInstanceOfType(auditoriumDomainModelResult, typeof(AuditoriumDomainModel));
            Assert.IsInstanceOfType(resultModel, typeof(AcceptedResult));
            Assert.AreEqual(((AcceptedResult)result).StatusCode, expectedStatusCode);
            Assert.AreEqual(auditoriumDomainModelResult.Id, _auditoriumDomainModel.Id);
        }

        [TestMethod]
        public void AuditoriumsController_Delete_ThrowsDbUpdateException()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            AuditoriumsController auditoriumController = new AuditoriumsController(_mockAuditoriumService.Object);
            _mockAuditoriumService.Setup(x => x.DeleteAuditorium(It.IsAny<int>())).Throws(dbUpdateException);

            //Act
            var result = auditoriumController.Delete(_auditoriumDomainModel.Id).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = ((BadRequestObjectResult)result);
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_Delete_ReturnsNull()
        {
            AuditoriumDomainModel nullDomainModel = null;
            Task<AuditoriumDomainModel> nullTask = Task.FromResult(nullDomainModel);

            string expectedMessage = Messages.AUDITORIUM_DOES_NOT_EXIST;
            int expectedStatusCode = 500;

            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            errorResponseModel.ErrorMessage = expectedMessage;
            errorResponseModel.StatusCode = System.Net.HttpStatusCode.InternalServerError;

            AuditoriumsController auditoriumController = new AuditoriumsController(_mockAuditoriumService.Object);
            _mockAuditoriumService.Setup(x => x.DeleteAuditorium(It.IsAny<int>())).Returns(nullTask);

            //Act
            var result = auditoriumController.Delete(_auditoriumDomainModel.Id).ConfigureAwait(false).GetAwaiter().GetResult();
            ObjectResult viewResult = (ObjectResult)result;
            var messageResult = (ErrorResponseModel)viewResult.Value;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, messageResult.ErrorMessage);
            Assert.AreEqual(expectedStatusCode, (int)messageResult.StatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_Put_ReturnsUpdatedAuditorium()
        {
            //Arrange
            AuditoriumsController auditoriumController = new AuditoriumsController(_mockAuditoriumService.Object);
            int expectedStatusCode = 202;
            Task<AuditoriumDomainModel> auditorium = Task.FromResult((AuditoriumDomainModel)_auditoriumDomainModel);
            CreateAuditoriumModel createAuditoriumModel = new CreateAuditoriumModel()
            {
                cinemaId = 1,
                auditName = "Auditorium",
                seatRows = 1,
                numberOfSeats = 1
            };

            var updatedAudit = new AuditoriumDomainModel()
            {
                CinemaId = createAuditoriumModel.cinemaId,
                Name = createAuditoriumModel.auditName
            };


            Task<AuditoriumDomainModel> updated = Task.FromResult((AuditoriumDomainModel)updatedAudit);
            _mockAuditoriumService.Setup(x => x.GetAuditoriumByIdAsync(It.IsAny<int>())).Returns(auditorium);
            _mockAuditoriumService.Setup(x => x.UpdateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(updated);

            //Act
            var resultAction = auditoriumController.Put(It.IsAny<int>(),createAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((AcceptedResult)resultAction).Value;
            var auditoriumDomainModelResult = (AuditoriumDomainModel)result;

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(updatedAudit.Id, auditoriumDomainModelResult.Id);
            Assert.AreEqual(updatedAudit.Name, auditoriumDomainModelResult.Name);
            Assert.IsInstanceOfType(resultAction, typeof(AcceptedResult));
            Assert.AreEqual(expectedStatusCode, ((AcceptedResult)resultAction).StatusCode);
        
        }

        [TestMethod]
        public void AuditoriumsController_Put_ThrowsDbUpdateException()
        {
            //Arrange
            AuditoriumsController auditoriumController = new AuditoriumsController(_mockAuditoriumService.Object);
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            Task<AuditoriumDomainModel> auditorium = Task.FromResult((AuditoriumDomainModel)_auditoriumDomainModel);
            CreateAuditoriumModel createAuditoriumModel = new CreateAuditoriumModel()
            {
                cinemaId = 1,
                auditName = "Auditorium",
                seatRows = 1,
                numberOfSeats = 1
            };

            var updatedAudit = new AuditoriumDomainModel()
            {
                CinemaId = createAuditoriumModel.cinemaId,
                Name = createAuditoriumModel.auditName
            };


            Task<AuditoriumDomainModel> updated = Task.FromResult((AuditoriumDomainModel)updatedAudit);
            _mockAuditoriumService.Setup(x => x.GetAuditoriumByIdAsync(It.IsAny<int>())).Returns(auditorium);
            _mockAuditoriumService.Setup(x => x.UpdateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Throws(dbUpdateException);

            //Act
            var resultAction = auditoriumController.Put(It.IsAny<int>(), createAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = ((BadRequestObjectResult)resultAction);
            var badObjectResult = ((BadRequestObjectResult)resultAction).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);

        }
        [TestMethod]
        public void AuditoriumsController_Put_ReturnNull()
        {
            //Arrange
            AuditoriumsController auditoriumController = new AuditoriumsController(_mockAuditoriumService.Object);
            AuditoriumDomainModel nullDomainModel = null;
            Task<AuditoriumDomainModel> nullTask = Task.FromResult(nullDomainModel);

            string expectedMessage = Messages.AUDITORIUM_DOES_NOT_EXIST;
            int expectedStatusCode = 400;

            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            errorResponseModel.ErrorMessage = expectedMessage;
            errorResponseModel.StatusCode = System.Net.HttpStatusCode.InternalServerError;

            _mockAuditoriumService.Setup(x => x.GetAuditoriumByIdAsync(It.IsAny<int>())).Returns(nullTask);
            _mockAuditoriumService.Setup(x => x.UpdateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(nullTask);

            //Act
            var resultAction = auditoriumController.Put(It.IsAny<int>(), It.IsAny<CreateAuditoriumModel>()).ConfigureAwait(false).GetAwaiter().GetResult();
            ObjectResult viewResult = (ObjectResult)resultAction;
            var messageResult = (ErrorResponseModel)viewResult.Value;

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, messageResult.ErrorMessage);
            Assert.AreEqual(expectedStatusCode, (int)messageResult.StatusCode);

        }

    }
}
