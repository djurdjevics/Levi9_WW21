using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    public class TicketsControllerTest
    {
        private Mock<ITicketService> _mockTicketService;
        private TicketDomainModel _ticketDomainModel;
        private Mock<IUserService> _mockUserService;
        private CreateTicketModel createTicketModel;
        private SeatDomainModel _seatDomainModel;
        private ClaimsPrincipal user;

        [TestInitialize]
        public void TestInitialize()
        {
            _ticketDomainModel = new TicketDomainModel
            {
                ProjectionId = Guid.NewGuid(),
                SeatId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _seatDomainModel = new SeatDomainModel()
            {
                AuditoriumId = 1,
                Id = Guid.NewGuid(),
                Number = 2,
                Row = 2
            };

            user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("UserName", "m.tatic"),
            }, "mock"));

            List<TicketDomainModel> ticketList = new List<TicketDomainModel>();
            ticketList.Add(_ticketDomainModel);
            IEnumerable<TicketDomainModel> tickets = ticketList;
            Task<IEnumerable<TicketDomainModel>> responseTask = Task.FromResult(tickets);
            createTicketModel = new CreateTicketModel();

            _mockTicketService = new Mock<ITicketService>();
            _mockUserService = new Mock<IUserService>();
        }

        [TestMethod]
        public void TicketsController_GetAllTickets_ReturnsListOfTickets()
        {
            //Arrange
            List<TicketDomainModel> ticketList = new List<TicketDomainModel>();
            ticketList.Add(_ticketDomainModel);
            IEnumerable<TicketDomainModel> tickets = ticketList;
            Task<IEnumerable<TicketDomainModel>> responseTask = Task.FromResult(tickets);
            int expectedCount = 1;
            int expectedStatusCode = 200;

            TicketsController ticketsController = new TicketsController(_mockTicketService.Object, _mockUserService.Object);
            _mockTicketService.Setup(x => x.GetAllTickets()).Returns(responseTask);

            //Act
            var result = ticketsController.GetAllTickets().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var ticketDomainModelResultList = (List<TicketDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(resultList);
            Assert.AreEqual(ticketDomainModelResultList.Count, expectedCount);
            Assert.IsInstanceOfType(ticketDomainModelResultList[0], typeof(TicketDomainModel));
            Assert.AreEqual(ticketDomainModelResultList[0].ProjectionId, _ticketDomainModel.ProjectionId);
            Assert.AreEqual(ticketDomainModelResultList[0].SeatId, _ticketDomainModel.SeatId);
            Assert.AreEqual(ticketDomainModelResultList[0].UserId, _ticketDomainModel.UserId);
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void TicketsController_GetAllTickets_TicketsReturnsNull_ReturnError()
        {
            //Arrange
            IEnumerable<TicketDomainModel> nullDomainModel = null;
            Task<IEnumerable<TicketDomainModel>> nullTask = Task.FromResult(nullDomainModel);

            string expectedMessage = Messages.TICKET_GET_ALL_ERROR;
            int expectedStatusCode = 500;

            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            errorResponseModel.ErrorMessage = expectedMessage;
            errorResponseModel.StatusCode = System.Net.HttpStatusCode.InternalServerError;

            TicketsController ticketsController = new TicketsController(_mockTicketService.Object, _mockUserService.Object);
            _mockTicketService.Setup(x => x.GetAllTickets()).Returns(nullTask);

            //Act
            var result = ticketsController.GetAllTickets().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            ObjectResult viewResult = (ObjectResult)result;
            var viewMessage = (ErrorResponseModel)viewResult.Value;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(viewResult.Value.ToString(), errorResponseModel.ToString());
            Assert.AreEqual(errorResponseModel.ErrorMessage, viewMessage.ErrorMessage);
            Assert.AreEqual(viewResult.StatusCode, expectedStatusCode);
        }

        [TestMethod]
        public void TicketsController_GetByIdAsync_ReturnTicket()
        {
            //Arrange
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object, _mockUserService.Object);
            var resModel = new ResponseModel<TicketDomainModel>()
            {
                DomainModel = _ticketDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<TicketDomainModel>> responseModel = Task.FromResult((ResponseModel<TicketDomainModel>)resModel);
            _mockTicketService.Setup(x => x.GetTicketByIdAsync(It.IsAny<Guid>())).Returns(responseModel);

            //Act
            var resultAction = ticketsController.GetByIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var data = ((TicketDomainModel)result);

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_ticketDomainModel.Id, data.Id);
            Assert.AreEqual(_ticketDomainModel.Price, data.Price);
            Assert.AreEqual(_ticketDomainModel.ProjectionId, data.ProjectionId);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));

        }

        [TestMethod]
        public void TicketsController_GetByIdAsync_ReturnErrorModel()
        {
            //Arrange
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object, _mockUserService.Object);
            var resModel = new ResponseModel<TicketDomainModel>()
            {
                DomainModel = null,
                ErrorMessage = Messages.TICKET_GET_ALL_ERROR,
                IsSuccessful = false
            };
            Task<ResponseModel<TicketDomainModel>> responseModel = Task.FromResult((ResponseModel<TicketDomainModel>)resModel);
            _mockTicketService.Setup(x => x.GetTicketByIdAsync(It.IsAny<Guid>())).Returns(responseModel);

            //Act
            var resultAction = ticketsController.GetByIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;
            var errorModel = ((ErrorResponseModel)result);

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(resModel.ErrorMessage, errorModel.ErrorMessage);
            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, errorModel.StatusCode);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public void TicketsController_DeleteProjectionIfThereIsNoTickets_ReturnTickets()
        {
            //Arrange
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object, _mockUserService.Object);
            var listOfModels = new List<TicketDomainModel>() { _ticketDomainModel };
            var enumerables = (IEnumerable<TicketDomainModel>) listOfModels;
            var resModel = new ResponseModel<IEnumerable<TicketDomainModel>>()
            {
                DomainModel = enumerables,
                ErrorMessage = null,
                IsSuccessful = true
            };
            _mockTicketService.Setup(x => x.GetTicketsByProjectionId(It.IsAny<Guid>())).Returns(resModel);

            //Act
            var resultAction = ticketsController.DeleteProjectionIfThereIsNoTicket(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var data = ((List<TicketDomainModel>)result);

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_ticketDomainModel.Id, data[0].Id);
            Assert.AreEqual(_ticketDomainModel.Price, data[0].Price);
            Assert.AreEqual(_ticketDomainModel.ProjectionId, data[0].ProjectionId);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
        }

        [TestMethod]
        public void TicketsController_DeleteProjectionIfThereIsNoTickets_ReturnErrorModel()
        {
            //Arrange
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object, _mockUserService.Object);
            var listOfModels = new List<TicketDomainModel>() { _ticketDomainModel };
            var enumerables = (IEnumerable<TicketDomainModel>)listOfModels;
            var resModel = new ResponseModel<IEnumerable<TicketDomainModel>>()
            {
                DomainModel = null,
                ErrorMessage = Messages.TICKET_GET_ALL_ERROR,
                IsSuccessful = false
            };
            _mockTicketService.Setup(x => x.GetTicketsByProjectionId(It.IsAny<Guid>())).Returns(resModel);

            //Act
            var resultAction = ticketsController.DeleteProjectionIfThereIsNoTicket(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((BadRequestObjectResult)resultAction).Value;
            var data = ((ErrorResponseModel)result);

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(resModel.ErrorMessage, data.ErrorMessage);
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, data.StatusCode);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void TicketsController_GetByProjectionIdAsync_ReturnTickets()
        {
            //Arrange
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object, _mockUserService.Object);
            var listOfModels = new List<TicketDomainModel>() { _ticketDomainModel };
            var enumerables = (IEnumerable<TicketDomainModel>)listOfModels;
            var resModel = new ResponseModel<IEnumerable<TicketDomainModel>>()
            {
                DomainModel = enumerables,
                ErrorMessage = null,
                IsSuccessful = true
            };
            _mockTicketService.Setup(x => x.GetTicketsByProjectionId(It.IsAny<Guid>())).Returns(resModel);

            //Act
            var resultAction = ticketsController.GetByProjectionIdIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var data = ((ResponseModel<IEnumerable<TicketDomainModel>>)result);

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_ticketDomainModel.Id, data.DomainModel.ToList()[0].Id);
            Assert.AreEqual(_ticketDomainModel.Price, data.DomainModel.ToList()[0].Price);
            Assert.AreEqual(_ticketDomainModel.ProjectionId, data.DomainModel.ToList()[0].ProjectionId);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
        }

        [TestMethod]
        public void TicketsController_GetByProjectionIdAsync_ReturnErrorModel()
        {
            //Arrange
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object, _mockUserService.Object);
            var listOfModels = new List<TicketDomainModel>() { _ticketDomainModel };
            var enumerables = (IEnumerable<TicketDomainModel>)listOfModels;
            var resModel = new ResponseModel<IEnumerable<TicketDomainModel>>()
            {
                DomainModel = null,
                ErrorMessage = Messages.TICKET_NOT_FOUND,
                IsSuccessful = false
            };
            _mockTicketService.Setup(x => x.GetTicketsByProjectionId(It.IsAny<Guid>())).Returns(resModel);

            //Act
            var resultAction = ticketsController.GetByProjectionIdIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;
            var data = (ErrorResponseModel)result;

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(resModel.ErrorMessage, data.ErrorMessage);
            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, data.StatusCode);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public void TicketsController_GetBusySeatsByProjectionId_ReturnSeats()
        {
            //Arrange
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object, _mockUserService.Object);
            var listOfModels = new List<SeatDomainModel>() { _seatDomainModel };
            var enumerables = (IEnumerable<SeatDomainModel>)listOfModels;
            var resModel = new ResponseModel<IEnumerable<SeatDomainModel>>()
            {
                DomainModel = enumerables,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<IEnumerable<SeatDomainModel>>> responseModel = Task.FromResult(resModel);
            _mockTicketService.Setup(x => x.GetBusySeats(It.IsAny<Guid>())).Returns(responseModel);

            //Act
            var resultAction = ticketsController.GetBusySeatsByProjectionIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)resultAction).Value;
            var data = ((IEnumerable<SeatDomainModel>)result);

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_seatDomainModel.Id, data.ToList()[0].Id);
            Assert.IsInstanceOfType(resultAction, typeof(OkObjectResult));
        }

        [TestMethod]
        public void TicketsController_GetBusySeatsByProjectionId_ReturnErrorModel()
        {
            //Arrange
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object, _mockUserService.Object);
            var listOfModels = new List<SeatDomainModel>() { _seatDomainModel };
            var enumerables = (IEnumerable<SeatDomainModel>)listOfModels;
            var resModel = new ResponseModel<IEnumerable<SeatDomainModel>>()
            {
                DomainModel = null,
                ErrorMessage = Messages.TICKET_NOT_FOUND,
                IsSuccessful = false
            };
            Task<ResponseModel<IEnumerable<SeatDomainModel>>> responseModel = Task.FromResult(resModel);
            _mockTicketService.Setup(x => x.GetBusySeats(It.IsAny<Guid>())).Returns(responseModel);

            //Act
            var resultAction = ticketsController.GetBusySeatsByProjectionIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((NotFoundObjectResult)resultAction).Value;
            var data = ((ErrorResponseModel)result);

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, data.StatusCode);
            Assert.AreEqual(resModel.ErrorMessage, data.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public void TicketsController_Post_InvalidModelState()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;

            TicketsController ticketsController = new TicketsController(_mockTicketService.Object, _mockUserService.Object);
            ticketsController.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = ticketsController.PostAsync(createTicketModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultMessage = (BadRequestObjectResult)result;
            var resultMassageValue = ((BadRequestObjectResult)result).Value;
            var errorResult = ((SerializableError)resultMassageValue).GetValueOrDefault("key");
            var message = (string[])errorResult;


            //Assert
            Assert.IsNotNull(resultMessage);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultMessage.StatusCode);
        }

        [TestMethod]
        public void TicketsController_PostAsync_ReturnNewTicket()
        {
            //Arrange
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object, _mockUserService.Object);
            ticketsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            CreateTicketModel createTicketModel = new CreateTicketModel()
            {
                Price = 350,
                ProjectionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                SeatIds = new List<Seat>() { new Seat() { Id = new Guid(), Number = 1, Row = 1, AuditoriumId = 1 } }
            };

            var resModel = new ResponseModel<TicketDomainModel>()
            {
                DomainModel = _ticketDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<TicketDomainModel>> responseModel = Task.FromResult(resModel);

            //Act
            _mockTicketService.Setup(x => x.AddTicket(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<double>(), It.IsAny<string>())).Returns(responseModel);
            _mockUserService.Setup(x => x.AddBonusPoints(It.IsAny<Guid>(), It.IsAny<int>())).Returns(1);
            var resultAction = ticketsController.PostAsync(createTicketModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((CreatedResult)resultAction).Value;
            var data = ((List<TicketDomainModel>)result);

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(resModel.DomainModel.Id, data[0].Id);
            Assert.IsInstanceOfType(resultAction, typeof(CreatedResult));
            Assert.AreEqual(201, ((CreatedResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void TicketsController_PostAsync_ReturnErrorModelTicketNotCreated()
        {
            //Arrange
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object, _mockUserService.Object);
            ticketsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            CreateTicketModel createTicketModel = new CreateTicketModel()
            {
                Price = 350,
                ProjectionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                SeatIds = new List<Seat>() { new Seat() { Id = new Guid(), Number = 1, Row = 1, AuditoriumId = 1 } }
            };

            var resModel = new ResponseModel<TicketDomainModel>()
            {
                DomainModel = null,
                ErrorMessage = Messages.TICKET_CREATION_ERROR,
                IsSuccessful = false
            };
            Task<ResponseModel<TicketDomainModel>> responseModel = Task.FromResult(resModel);

            //Act
            _mockTicketService.Setup(x => x.AddTicket(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<double>(), It.IsAny<string>())).Returns(responseModel);
            _mockUserService.Setup(x => x.AddBonusPoints(It.IsAny<Guid>(), It.IsAny<int>())).Returns(1);
            var resultAction = ticketsController.PostAsync(createTicketModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((BadRequestObjectResult)resultAction).Value;
            var data = ((ErrorResponseModel)result);

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(resModel.ErrorMessage, data.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(400, ((BadRequestObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void TicketsController_PostAsync_ReturnErrorModelBonusPointsError()
        {
            //Arrange
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object, _mockUserService.Object);
            ticketsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            CreateTicketModel createTicketModel = new CreateTicketModel()
            {
                Price = 350,
                ProjectionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                SeatIds = new List<Seat>() { new Seat() { Id = new Guid(), Number = 1, Row = 1, AuditoriumId = 1 } }
            };

            var resModel = new ResponseModel<TicketDomainModel>()
            {
                DomainModel = _ticketDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<TicketDomainModel>> responseModel = Task.FromResult(resModel);

            //Act
            _mockTicketService.Setup(x => x.AddTicket(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<double>(), It.IsAny<string>())).Returns(responseModel);
            _mockUserService.Setup(x => x.AddBonusPoints(It.IsAny<Guid>(), It.IsAny<int>())).Returns(-1);
            var resultAction = ticketsController.PostAsync(createTicketModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((BadRequestObjectResult)resultAction).Value;
            var data = ((ErrorResponseModel)result);

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual("An error occured while assigning bonus points to the User", data.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(400, ((BadRequestObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void TicketsController_PostAsync_AddTicketReturnErrorModel()
        {
            //Arrange
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object, _mockUserService.Object);
            ticketsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            CreateTicketModel createTicketModel = new CreateTicketModel()
            {
                Price = 350,
                ProjectionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                SeatIds = new List<Seat>() { new Seat() { Id = new Guid(), Number = 1, Row = 1, AuditoriumId = 1 } }
            };

            var resModel = new ResponseModel<TicketDomainModel>()
            {
                DomainModel = _ticketDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<TicketDomainModel>> responseModel = Task.FromResult(resModel);
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            //Act
            _mockTicketService.Setup(x => x.AddTicket(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<double>(), It.IsAny<string>())).Throws(dbUpdateException);
            _mockUserService.Setup(x => x.AddBonusPoints(It.IsAny<Guid>(), It.IsAny<int>())).Returns(1);
            var resultAction = ticketsController.PostAsync(createTicketModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((BadRequestObjectResult)resultAction).Value;
            var data = ((ErrorResponseModel)result);

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(exception.Message, data.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(400, ((BadRequestObjectResult)resultAction).StatusCode);
        }

        [TestMethod]
        public void TicketsController_PostAsync_AddBonusPointsReturnErrorModel()
        {
            //Arrange
            TicketsController ticketsController = new TicketsController(_mockTicketService.Object, _mockUserService.Object);
            ticketsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            CreateTicketModel createTicketModel = new CreateTicketModel()
            {
                Price = 350,
                ProjectionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                SeatIds = new List<Seat>() { new Seat() { Id = new Guid(), Number = 1, Row = 1, AuditoriumId = 1 } }
            };

            var resModel = new ResponseModel<TicketDomainModel>()
            {
                DomainModel = _ticketDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<TicketDomainModel>> responseModel = Task.FromResult(resModel);
            Exception exception = new Exception("An error occured while assigning bonus points to the User");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            //Act
            _mockTicketService.Setup(x => x.AddTicket(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<double>(), It.IsAny<string>())).Returns(responseModel);
            _mockUserService.Setup(x => x.AddBonusPoints(It.IsAny<Guid>(), It.IsAny<int>())).Throws(dbUpdateException);
            var resultAction = ticketsController.PostAsync(createTicketModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((BadRequestObjectResult)resultAction).Value;
            var data = ((ErrorResponseModel)result);

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(exception.Message, data.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(400, ((BadRequestObjectResult)resultAction).StatusCode);
        }
    }
}
