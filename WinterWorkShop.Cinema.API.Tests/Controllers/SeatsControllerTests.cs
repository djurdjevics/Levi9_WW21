using Microsoft.AspNetCore.Mvc;
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
    public class SeatsControllerTests
    {
        private Mock<ISeatService> _mockSeatsService;
        private Seat _seat;
        private SeatDomainModel _seatDomainModel;
        private SeatsController seatsController;

        [TestInitialize]
        public void TestInitialize()
        {
            _seat = new Seat()
            {
                Id = Guid.NewGuid(),
                Number = 1,
                Row = 1,
                AuditoriumId = 1,
                Auditorium = new Auditorium() { }
            };

            _seatDomainModel = new SeatDomainModel()
            {
                Id = _seat.Id,
                Number = 1,
                Row = 1,
                AuditoriumId = 1
            };

            _mockSeatsService = new Mock<ISeatService>();
        }


        [TestMethod]
        public void SeatsController_GetAsync_ReturnListOfSeats()
        {
            //Arrange
            seatsController = new SeatsController(_mockSeatsService.Object);
            var expectedResoultCount = 1;
            var expectedStatusCode = 200;
            var seatsList = new List<SeatDomainModel>() { _seatDomainModel };
            Task<IEnumerable<SeatDomainModel>> seatsCollection = Task.FromResult((IEnumerable<SeatDomainModel>)seatsList);

            //Act
            _mockSeatsService.Setup(x => x.GetAllAsync()).Returns(seatsCollection);
            var resultAction = seatsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)resultAction).Value;
            var seatsDomainModelResults = (List<SeatDomainModel>)resultList;
            //Assert
            Assert.IsNotNull(seatsDomainModelResults);
            Assert.AreEqual(expectedResoultCount, seatsDomainModelResults.Count);
            Assert.AreEqual(_seatDomainModel.Id, seatsDomainModelResults[0].Id);
            Assert.AreEqual(_seatDomainModel.Number, seatsDomainModelResults[0].Number);
            Assert.AreEqual(_seatDomainModel.Row, seatsDomainModelResults[0].Row);
            Assert.AreEqual(_seatDomainModel.AuditoriumId, seatsDomainModelResults[0].AuditoriumId);
            Assert.IsInstanceOfType(seatsDomainModelResults[0], typeof(SeatDomainModel));
            Assert.IsInstanceOfType(seatsDomainModelResults, typeof(List<SeatDomainModel>));
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void SeatsController_GetAsync_ReturnEmptyList()
        {
            //Arrange
            seatsController = new SeatsController(_mockSeatsService.Object);
            var expectedResoultCount = 0;
            var expectedStatusCode = 200;
            List<SeatDomainModel> seatsList = null;
            Task<IEnumerable<SeatDomainModel>> seatsCollection = Task.FromResult((IEnumerable<SeatDomainModel>)seatsList);

            //Act
            _mockSeatsService.Setup(x => x.GetAllAsync()).Returns(seatsCollection);
            var resultAction = seatsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)resultAction).Value;
            var seatsDomainModelResults = (List<SeatDomainModel>)resultList;
            //Assert
            Assert.IsNotNull(seatsDomainModelResults);
            Assert.AreEqual(expectedResoultCount, seatsDomainModelResults.Count);
            Assert.IsInstanceOfType(seatsDomainModelResults, typeof(List<SeatDomainModel>));
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void SeatsController_GetByIdAsync_ReturnSeat()
        {
            //Arrange
            seatsController = new SeatsController(_mockSeatsService.Object);
            var responseModel = new ResponseModel<SeatDomainModel>()
            {
                DomainModel = _seatDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            var expectedStatusCode = 200;
            Task<ResponseModel<SeatDomainModel>> responseModelTask = Task.FromResult(responseModel);
            //Act
            _mockSeatsService.Setup(x => x.GetSeatByIdAsync(It.IsAny<Guid>())).Returns(responseModelTask);
            var resultAction = seatsController.GetByIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var seatsDomainModelResult = (SeatDomainModel)result;
            //Assert
            Assert.IsNotNull(seatsDomainModelResult);
            Assert.AreEqual(_seatDomainModel.Id, seatsDomainModelResult.Id);
            Assert.AreEqual(_seatDomainModel.Row, seatsDomainModelResult.Row);
            Assert.AreEqual(_seatDomainModel.Number, seatsDomainModelResult.Number);
            Assert.AreEqual(_seatDomainModel.AuditoriumId, seatsDomainModelResult.AuditoriumId);
            Assert.IsInstanceOfType(seatsDomainModelResult, typeof(SeatDomainModel));
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void SeatsController_GetByIdAsync_Return_Not_Found()
        {
            //Arrange
            seatsController = new SeatsController(_mockSeatsService.Object);
            var responseModel = new ResponseModel<SeatDomainModel>()
            {
                DomainModel = null,
                ErrorMessage = Messages.SEAT_GET_BY_ID_ERROR,
                IsSuccessful = false
            };
            var expectedStatusCode = 404;
            var internalServerErrorCode = 500;
            Task<ResponseModel<SeatDomainModel>> responseModelTask = Task.FromResult(responseModel);
            //Act
            _mockSeatsService.Setup(x => x.GetSeatByIdAsync(It.IsAny<Guid>())).Returns(responseModelTask);
            var resultAction = seatsController.GetByIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;
            var errorMessage = (ErrorResponseModel)result;
            //Assert
            Assert.AreEqual(responseModel.ErrorMessage, errorMessage.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)resultAction).StatusCode);
            Assert.AreEqual(internalServerErrorCode, (int)errorMessage.StatusCode);
        }

        [TestMethod]
        public void SeatsController_GetSeatsByAuditoriumId_ReturnSeats()
        {
            //Arrange
            seatsController = new SeatsController(_mockSeatsService.Object);
            var listOfSeats = new List<SeatDomainModel>() { _seatDomainModel };
            _mockSeatsService.Setup(x => x.GetSeatsByAuditoriumId(It.IsAny<int>())).Returns(listOfSeats);

            //Act
            var resultAction = seatsController.GetSeatsByAuditoriumId(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var data = ((List<SeatDomainModel>)result);

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_seat.Id, data[0].Id);
            Assert.AreEqual(_seat.Row, data[0].Row);
            Assert.AreEqual(_seat.Number, data[0].Number);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
            Assert.AreEqual(200, ((OkObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void SeatsController_GetSeatsByAuditoriumId_ReturnErrorNotFound()
        {
            //Arrange
            seatsController = new SeatsController(_mockSeatsService.Object);
            List<SeatDomainModel> listOfSeats = null;
            _mockSeatsService.Setup(x => x.GetSeatsByAuditoriumId(It.IsAny<int>())).Returns(listOfSeats);

            //Act
            var resultAction = seatsController.GetSeatsByAuditoriumId(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(Messages.SEAT_GET_ALL_SEATS_ERROR, result);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
            Assert.AreEqual(404, ((NotFoundObjectResult)resultAction).StatusCode);
        }
    }
}
