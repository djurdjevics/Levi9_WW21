using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class TicketServiceTests
    {
        private Mock<ITicketsRepository> _mockTicketsRepository;
        private Mock<IProjectionService> _mockProjectionService;
        private Mock<ISeatService> _mockSeatService;
        private Mock<IUserService> _mockUserSerivce;
        private Ticket _ticket;
        private Seat _seat;
        private SeatDomainModel _seatDomainModel;
        private TicketDomainModel _ticketDomainModel;
        private Projection _projection;
        private ProjectionDomainModel _projectionDomainModel;
        private TicketService ticketService;
        private User _user;
        private UserDomainModel _userDomainModel;


        [TestInitialize]
        public void TestInitialize()
        {
            _ticket = new Ticket()
            {
                Id = Guid.NewGuid(),
                Price = 10,
                ProjectionId = Guid.NewGuid(),
                SeatId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _ticketDomainModel = new TicketDomainModel()
            {
                UserId = _ticket.UserId,
                SeatId = _ticket.SeatId,
                ProjectionId = _ticket.ProjectionId,
                Id = _ticket.Id,
                Price = _ticket.Price
            };

            _seat = new Seat()
            {
                Id = Guid.NewGuid(),
                Number = 1,
                Row = 1,
                AuditoriumId = 1
            };

            _seatDomainModel = new SeatDomainModel()
            {
                Id = _seat.Id,
                Number = 1,
                Row = 1,
                AuditoriumId = 1
            };

            _projection = new Projection
            {
                Id = Guid.NewGuid(),
                Auditorium = new Auditorium { Name = "ImeSale" },
                Movie = new Movie { Title = "ImeFilma" },
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };

            _projectionDomainModel = new ProjectionDomainModel
            {
                Id = Guid.NewGuid(),
                AuditoriumName = "ImeSale",
                AuditoriumId = 1,
                MovieId = Guid.NewGuid(),
                MovieTitle = "ImeFilma",
                ProjectionTime = DateTime.Now.AddDays(1)
            };

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
            _mockTicketsRepository = new Mock<ITicketsRepository>();
            _mockProjectionService = new Mock<IProjectionService>();
            _mockSeatService = new Mock<ISeatService>();
            _mockUserSerivce = new Mock<IUserService>();
        }
        [TestMethod]
        public void TicketService_GetAllTickets_ReturnsListOfAllTickets()
        {
            //Arrange
            List<Ticket> ticketsList = new List<Ticket>();
            ticketsList.Add(_ticket);
            IEnumerable<Ticket> tickets = ticketsList;
            Task<IEnumerable<Ticket>> responseTask = Task.FromResult(tickets);

            int expectedResultCount = 1;
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            _mockTicketsRepository.Setup(x => x.GetAll()).Returns(responseTask);

            //Act
            var resultAction = ticketService.GetAllTickets().ConfigureAwait(false).GetAwaiter().GetResult().ToList();

            //Assert
            Assert.AreEqual(resultAction.Count, expectedResultCount);
            Assert.AreEqual(resultAction[0].Id, _ticket.Id);
            Assert.IsInstanceOfType(resultAction[0], typeof(TicketDomainModel));
            Assert.IsNotNull(resultAction);

        }

        [TestMethod]
        public void TicketService_GetAllAsync_RepositoryReturnsNull_ReturnsNull()
        {
            //Arrange
            IEnumerable<Ticket> tickets = null;
            Task<IEnumerable<Ticket>> responseTask = Task.FromResult(tickets);
            int expectedResultCount = 1;

            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            _mockTicketsRepository.Setup(x => x.GetAll()).Returns(responseTask);

            //Act
            var resultAction = ticketService.GetAllTickets().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }


        [TestMethod]
        public void TicketService_DeleteTicket_ReturnDeletedTicket()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            Task<Ticket> ticket = Task.FromResult(_ticket);
            var responseModel = new ResponseModel<ProjectionDomainModel>()
            {
                ErrorMessage = null,
                IsSuccessful = true,
                DomainModel = new ProjectionDomainModel()
                {
                    AuditoriumName = "Nikola Tesla",
                    Id = Guid.NewGuid(),
                    MovieId = Guid.NewGuid(),
                    MovieTitle = "Test Movie",
                    AuditoriumId = 1,
                    ProjectionTime = DateTime.Today.AddDays(1)
                }
            };
            Task<ResponseModel<ProjectionDomainModel>> response = Task.FromResult(responseModel);

            //Act
            _mockTicketsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(ticket);
            _mockProjectionService.Setup(x => x.GetProjectionById(It.IsAny<Guid>())).Returns(response);
            _mockTicketsRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(_ticket);
            var resultAction = ticketService.DeleteTicket(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_ticket.Id, resultAction.Id);
            Assert.AreEqual(_ticket.Price, resultAction.Price);
            Assert.AreEqual(_ticket.ProjectionId, resultAction.ProjectionId);
            Assert.AreEqual(_ticket.SeatId, resultAction.SeatId);
            Assert.AreEqual(_ticket.UserId, resultAction.UserId);
            Assert.IsInstanceOfType(resultAction, typeof(TicketDomainModel));
        }

        [TestMethod]
        public void TicketService_DeleteTicket_GetByIdReturnsNull()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            Ticket ticket = null;
            Task<Ticket> result = Task.FromResult(ticket);
            //Act
            _mockTicketsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(result);
            var resultAction = ticketService.DeleteTicket(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void TicketService_DeleteTicket_GetProjectionByIdReturnNull()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            Task<Ticket> result = Task.FromResult(_ticket);
            ResponseModel<ProjectionDomainModel> responseModel = null;
            Task<ResponseModel<ProjectionDomainModel>> response = Task.FromResult(responseModel);

            //Act
            _mockTicketsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(result);
            _mockProjectionService.Setup(x => x.GetProjectionById(It.IsAny<Guid>())).Returns(response);
            var resultAction = ticketService.DeleteTicket(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void TicketService_DeleteTicket_GetProjectionByIdWrongDateReturnNull()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            Task<Ticket> result = Task.FromResult(_ticket);
            ResponseModel<ProjectionDomainModel> responseModel = new ResponseModel<ProjectionDomainModel>()
            {
                ErrorMessage = null,
                IsSuccessful = true,
                DomainModel = new ProjectionDomainModel()
                {
                    AuditoriumName = "Nikola Tesla",
                    Id = Guid.NewGuid(),
                    MovieId = Guid.NewGuid(),
                    MovieTitle = "Test Movie",
                    AuditoriumId = 1,
                    ProjectionTime = DateTime.Now
                }
            };
            Task<ResponseModel<ProjectionDomainModel>> response = Task.FromResult(responseModel);

            //Act
            _mockTicketsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(result);
            _mockProjectionService.Setup(x => x.GetProjectionById(It.IsAny<Guid>())).Returns(response);
            var resultAction = ticketService.DeleteTicket(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void TicketService_DeleteTicket_DbDeleteReturnNull()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            Task<Ticket> ticket = Task.FromResult(_ticket);
            var responseModel = new ResponseModel<ProjectionDomainModel>()
            {
                ErrorMessage = null,
                IsSuccessful = true,
                DomainModel = new ProjectionDomainModel()
                {
                    AuditoriumName = "Nikola Tesla",
                    Id = Guid.NewGuid(),
                    MovieId = Guid.NewGuid(),
                    MovieTitle = "Test Movie",
                    AuditoriumId = 1,
                    ProjectionTime = DateTime.Today.AddDays(1)
                }
            };
            Task<ResponseModel<ProjectionDomainModel>> response = Task.FromResult(responseModel);
            Ticket tic = null;
            //Act
            _mockTicketsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(ticket);
            _mockProjectionService.Setup(x => x.GetProjectionById(It.IsAny<Guid>())).Returns(response);
            _mockTicketsRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(tic);
            var resultAction = ticketService.DeleteTicket(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void TicketService_GetTicketByIdAsync_ReturnTicket()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            Task<Ticket> ticket = Task.FromResult(_ticket);

            //Act
            _mockTicketsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(ticket);
            var resultAction =  ticketService.GetTicketByIdAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
            Assert.AreEqual(_ticket.Id, resultAction.DomainModel.Id);
            Assert.AreEqual(_ticket.Price, resultAction.DomainModel.Price);
            Assert.AreEqual(_ticket.ProjectionId, resultAction.DomainModel.ProjectionId);
            Assert.AreEqual(_ticket.SeatId, resultAction.DomainModel.SeatId);
            Assert.AreEqual(_ticket.UserId, resultAction.DomainModel.UserId);
            Assert.IsInstanceOfType(resultAction.DomainModel, typeof(TicketDomainModel));
        }

        [TestMethod]
        public void TicketService_GetTicketByIdAsync_DbReturnNullOnGetByIdAsync()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            Ticket tic = null;
            Task<Ticket> ticket = Task.FromResult(tic);
            var expectedMessage = Messages.TICKET_NOT_FOUND;

            //Act
            _mockTicketsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(ticket);
            var resultAction = ticketService.GetTicketByIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.DomainModel);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
        }

        [TestMethod]
        public void TicketService_GetTicketsByProjectionId_ReturnTickets()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            var tickets = new List<Ticket>() { _ticket };

            //Act
            _mockTicketsRepository.Setup(x => x.GetTicketsByProjectionId(It.IsAny<Guid>())).Returns(tickets);
            var resultAction = ticketService.GetTicketsByProjectionId(It.IsAny<Guid>());
            var result = (List<TicketDomainModel>)resultAction.DomainModel;

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsTrue(resultAction.IsSuccessful);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.AreEqual(_ticket.Id, result[0].Id);
            Assert.AreEqual(_ticket.Price, result[0].Price);
            Assert.AreEqual(_ticket.ProjectionId, result[0].ProjectionId);
            Assert.AreEqual(_ticket.SeatId, result[0].SeatId);
            Assert.AreEqual(_ticket.UserId, result[0].UserId);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<IEnumerable<TicketDomainModel>>));
        }

        [TestMethod]
        public void TicketService_GetTicketsByProjectionId_DbReturnNull()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            List<Ticket> tickets = null;
            var expectedErrorMessage = Messages.TICKET_PROJECTION_CAN_NOT_BE_FOUND;

            //Act
            _mockTicketsRepository.Setup(x => x.GetTicketsByProjectionId(It.IsAny<Guid>())).Returns(tickets);
            var resultAction = ticketService.GetTicketsByProjectionId(It.IsAny<Guid>());

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.DomainModel);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.AreEqual(expectedErrorMessage, resultAction.ErrorMessage);
        }

        [TestMethod]
        public void TicketService_AddTicket_ReturnTicket()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            var seatIdExists = new ResponseModel<SeatDomainModel>()
            {
                DomainModel = _seatDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<SeatDomainModel>> seatIdExist = Task.FromResult(seatIdExists);
            var projectionIdExists = new ResponseModel<ProjectionDomainModel>()
            {
                DomainModel = _projectionDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<ProjectionDomainModel>> projectionExist = Task.FromResult(projectionIdExists);
            List<Ticket> ticketsForProjection = new List<Ticket>();
            Task<UserDomainModel> user = Task.FromResult(_userDomainModel);
            Ticket insertedTicket = new Ticket()
            {
                Id = Guid.NewGuid(),
                Price = 200,
                ProjectionId = _projectionDomainModel.Id,
                SeatId = _seatDomainModel.Id,
                UserId = _userDomainModel.Id
            };
            //Act
            _mockSeatService.Setup(x => x.GetSeatByIdAsync(It.IsAny<Guid>())).Returns(seatIdExist);
            _mockProjectionService.Setup(x => x.GetProjectionById(It.IsAny<Guid>())).Returns(projectionExist);
            _mockTicketsRepository.Setup(x => x.GetTicketsByProjectionId(It.IsAny<Guid>())).Returns(ticketsForProjection);
            _mockUserSerivce.Setup(x => x.GetUserByUserName(It.IsAny<string>())).Returns(user);
            _mockTicketsRepository.Setup(x => x.Insert(It.IsAny<Ticket>())).Returns(insertedTicket);
            var resultAction = ticketService.AddTicket(_seatDomainModel.Id, _projectionDomainModel.Id, 200, _userDomainModel.UserName).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
            Assert.AreEqual(resultAction.DomainModel.Id, insertedTicket.Id);
            Assert.AreEqual(resultAction.DomainModel.Price, insertedTicket.Price);
            Assert.AreEqual(resultAction.DomainModel.SeatId, insertedTicket.SeatId);
            Assert.AreEqual(resultAction.DomainModel.UserId, insertedTicket.UserId);
            Assert.AreEqual(resultAction.DomainModel.ProjectionId, insertedTicket.ProjectionId);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<TicketDomainModel>));
        }

        [TestMethod]
        public void TicketService_AddTicket_GetSeatByIdReturnError()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            ResponseModel<SeatDomainModel> seatIdExists = new ResponseModel<SeatDomainModel>()
            {
                DomainModel = null,
                ErrorMessage = Messages.SEAT_GET_BY_ID_ERROR,
                IsSuccessful = false
            };
            Task<ResponseModel<SeatDomainModel>> seatIdExist = Task.FromResult(seatIdExists);
            var expectedErrorMessage = Messages.SEAT_GET_BY_ID_ERROR;
            //Act
            _mockSeatService.Setup(x => x.GetSeatByIdAsync(It.IsAny<Guid>())).Returns(seatIdExist);
            var resultAction = ticketService.AddTicket(_seatDomainModel.Id, _projectionDomainModel.Id, 200, _userDomainModel.UserName).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.DomainModel);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.AreEqual(expectedErrorMessage, resultAction.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<TicketDomainModel>));
        }

        [TestMethod]
        public void TicketService_AddTicket_GetProjectionByIdReturnError()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            var seatIdExists = new ResponseModel<SeatDomainModel>()
            {
                DomainModel = _seatDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<SeatDomainModel>> seatIdExist = Task.FromResult(seatIdExists);
            var projectionIdExists = new ResponseModel<ProjectionDomainModel>()
            {
                DomainModel = null,
                ErrorMessage = Messages.PROJECTION_GET_ALL_PROJECTIONS_ERROR,
                IsSuccessful = false
            };
            Task<ResponseModel<ProjectionDomainModel>> projectionExist = Task.FromResult(projectionIdExists);
            var expectedErrorMessage = Messages.PROJECTION_GET_ALL_PROJECTIONS_ERROR;
            
            //Act
            _mockSeatService.Setup(x => x.GetSeatByIdAsync(It.IsAny<Guid>())).Returns(seatIdExist);
            _mockProjectionService.Setup(x => x.GetProjectionById(It.IsAny<Guid>())).Returns(projectionExist);
            var resultAction = ticketService.AddTicket(_seatDomainModel.Id, _projectionDomainModel.Id, 200, _userDomainModel.UserName).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.DomainModel);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.AreEqual(expectedErrorMessage, resultAction.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<TicketDomainModel>));
        }

        [TestMethod]
        public void TicketService_AddTicket_AuditoriumIdDoesntMatchForSeatAndProjection()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            var seatIdExists = new ResponseModel<SeatDomainModel>()
            {
                DomainModel = _seatDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<SeatDomainModel>> seatIdExist = Task.FromResult(seatIdExists);
            var projectionModelWrongAuditoriumId = new ProjectionDomainModel()
            {
                Id = Guid.NewGuid(),
                AuditoriumName = "ImeSale",
                AuditoriumId = 5,
                MovieId = Guid.NewGuid(),
                MovieTitle = "ImeFilma",
                ProjectionTime = DateTime.Now.AddDays(1)
            };
            var projectionIdExists = new ResponseModel<ProjectionDomainModel>()
            {
                DomainModel = projectionModelWrongAuditoriumId,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<ProjectionDomainModel>> projectionExist = Task.FromResult(projectionIdExists);
            var expectedErrorMessage = Messages.TICKET_CREATION_ERROR_AUDITORIUM_ID_DOESNT_MATCH;
            //Act
            _mockSeatService.Setup(x => x.GetSeatByIdAsync(It.IsAny<Guid>())).Returns(seatIdExist);
            _mockProjectionService.Setup(x => x.GetProjectionById(It.IsAny<Guid>())).Returns(projectionExist);
            var resultAction = ticketService.AddTicket(_seatDomainModel.Id, _projectionDomainModel.Id, 200, _userDomainModel.UserName).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.DomainModel);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.AreEqual(expectedErrorMessage, resultAction.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<TicketDomainModel>));
        }


        [TestMethod]
        public void TicketService_AddTicket_GetTicketsByProjectionIdReturnError()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            var seatIdExists = new ResponseModel<SeatDomainModel>()
            {
                DomainModel = _seatDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<SeatDomainModel>> seatIdExist = Task.FromResult(seatIdExists);
            var projectionIdExists = new ResponseModel<ProjectionDomainModel>()
            {
                DomainModel = _projectionDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<ProjectionDomainModel>> projectionExist = Task.FromResult(projectionIdExists);
            List<Ticket> ticketsForProjection = null;
            var expectedErrorMessage = Messages.TICKET_PROJECTION_CAN_NOT_BE_FOUND;
            
            //Act
            _mockSeatService.Setup(x => x.GetSeatByIdAsync(It.IsAny<Guid>())).Returns(seatIdExist);
            _mockProjectionService.Setup(x => x.GetProjectionById(It.IsAny<Guid>())).Returns(projectionExist);
            _mockTicketsRepository.Setup(x => x.GetTicketsByProjectionId(It.IsAny<Guid>())).Returns(ticketsForProjection);
            var resultAction = ticketService.AddTicket(_seatDomainModel.Id, _projectionDomainModel.Id, 200, _userDomainModel.UserName).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.DomainModel);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.AreEqual(expectedErrorMessage, resultAction.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<TicketDomainModel>));
        }

        [TestMethod]
        public void TicketService_AddTicket_SeatIsAlreadyReserved()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            var seatIdExists = new ResponseModel<SeatDomainModel>()
            {
                DomainModel = _seatDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<SeatDomainModel>> seatIdExist = Task.FromResult(seatIdExists);
            var projectionIdExists = new ResponseModel<ProjectionDomainModel>()
            {
                DomainModel = _projectionDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<ProjectionDomainModel>> projectionExist = Task.FromResult(projectionIdExists);
            List<Ticket> ticketsForProjection = new List<Ticket>() { _ticket};
            var expectedErrorMessage = Messages.TICKET_SEAT_ALREADY_TAKEN;

            //Act
            _mockSeatService.Setup(x => x.GetSeatByIdAsync(It.IsAny<Guid>())).Returns(seatIdExist);
            _mockProjectionService.Setup(x => x.GetProjectionById(It.IsAny<Guid>())).Returns(projectionExist);
            _mockTicketsRepository.Setup(x => x.GetTicketsByProjectionId(It.IsAny<Guid>())).Returns(ticketsForProjection);
            var resultAction = ticketService.AddTicket(_ticket.SeatId, _projectionDomainModel.Id, 200, _userDomainModel.UserName).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.DomainModel);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.AreEqual(expectedErrorMessage, resultAction.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<TicketDomainModel>));
        }

        [TestMethod]
        public void TicketService_AddTicket_GetUserByUserNameReturnError()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            var seatIdExists = new ResponseModel<SeatDomainModel>()
            {
                DomainModel = _seatDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<SeatDomainModel>> seatIdExist = Task.FromResult(seatIdExists);
            var projectionIdExists = new ResponseModel<ProjectionDomainModel>()
            {
                DomainModel = _projectionDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<ProjectionDomainModel>> projectionExist = Task.FromResult(projectionIdExists);
            List<Ticket> ticketsForProjection = new List<Ticket>();
            UserDomainModel usersDomain = null;
            Task<UserDomainModel> user = Task.FromResult(usersDomain);
            var expectedErrorMessage = Messages.USER_NOT_FOUND;

            //Act
            _mockSeatService.Setup(x => x.GetSeatByIdAsync(It.IsAny<Guid>())).Returns(seatIdExist);
            _mockProjectionService.Setup(x => x.GetProjectionById(It.IsAny<Guid>())).Returns(projectionExist);
            _mockTicketsRepository.Setup(x => x.GetTicketsByProjectionId(It.IsAny<Guid>())).Returns(ticketsForProjection);
            _mockUserSerivce.Setup(x => x.GetUserByUserName(It.IsAny<string>())).Returns(user);
            var resultAction = ticketService.AddTicket(_seatDomainModel.Id, _projectionDomainModel.Id, 200, _userDomainModel.UserName).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.DomainModel);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.AreEqual(expectedErrorMessage, resultAction.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<TicketDomainModel>));
        }

        [TestMethod]
        public void TicketService_AddTicket_DbErrorOnInsert()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            var seatIdExists = new ResponseModel<SeatDomainModel>()
            {
                DomainModel = _seatDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<SeatDomainModel>> seatIdExist = Task.FromResult(seatIdExists);
            var projectionIdExists = new ResponseModel<ProjectionDomainModel>()
            {
                DomainModel = _projectionDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<ProjectionDomainModel>> projectionExist = Task.FromResult(projectionIdExists);
            List<Ticket> ticketsForProjection = new List<Ticket>();
            Task<UserDomainModel> user = Task.FromResult(_userDomainModel);
            Ticket insertedTicket = null;
            var expectedErrorMessage = Messages.TICKET_CREATION_ERROR;

            //Act
            _mockSeatService.Setup(x => x.GetSeatByIdAsync(It.IsAny<Guid>())).Returns(seatIdExist);
            _mockProjectionService.Setup(x => x.GetProjectionById(It.IsAny<Guid>())).Returns(projectionExist);
            _mockTicketsRepository.Setup(x => x.GetTicketsByProjectionId(It.IsAny<Guid>())).Returns(ticketsForProjection);
            _mockUserSerivce.Setup(x => x.GetUserByUserName(It.IsAny<string>())).Returns(user);
            _mockTicketsRepository.Setup(x => x.Insert(It.IsAny<Ticket>())).Returns(insertedTicket);
            var resultAction = ticketService.AddTicket(_seatDomainModel.Id, _projectionDomainModel.Id, 200, _userDomainModel.UserName).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.DomainModel);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.AreEqual(expectedErrorMessage, resultAction.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(ResponseModel<TicketDomainModel>));
        }

        [TestMethod]
        public void TicketService_GetBusySeats_ReturnSeats()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            IEnumerable<Ticket> tickets = new List<Ticket> { _ticket };
            ResponseModel<SeatDomainModel> seats = new ResponseModel<SeatDomainModel>()
            {
                DomainModel = _seatDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            Task<ResponseModel<SeatDomainModel>> seat = Task.FromResult(seats);
            //Act
            _mockTicketsRepository.Setup(x => x.GetTicketsByProjectionId(It.IsAny<Guid>())).Returns(tickets);
            _mockSeatService.Setup(x => x.GetSeatByIdAsync(It.IsAny<Guid>())).Returns(seat);
            var resultAction = ticketService.GetBusySeats(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<SeatDomainModel>)resultAction.DomainModel;

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
            Assert.AreEqual(result[0].Id, _seat.Id);
            Assert.AreEqual(result[0].AuditoriumId, _seat.AuditoriumId);
            Assert.AreEqual(result[0].Number, _seat.Number);
            Assert.AreEqual(result[0].Row, _seat.Row);
            Assert.IsInstanceOfType(result[0], typeof(SeatDomainModel));
        }

        [TestMethod]
        public void TicketService_GetBusySeats_GetTicketsByProjectionReturnNull()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            List<Ticket> tickets = null;
            var expectedErrorMessage = Messages.TICKET_PROJECTION_CAN_NOT_BE_FOUND;
           
            //Act
            _mockTicketsRepository.Setup(x => x.GetTicketsByProjectionId(It.IsAny<Guid>())).Returns(tickets);
            var resultAction = ticketService.GetBusySeats(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction.DomainModel);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.AreEqual(expectedErrorMessage, resultAction.ErrorMessage);
        }

        [TestMethod]
        public void TicketService_GetBusySeats_GetSeatByIdAsyncReturnNull()
        {
            //Arrange
            ticketService = new TicketService(_mockTicketsRepository.Object, _mockProjectionService.Object, _mockUserSerivce.Object, _mockSeatService.Object);
            IEnumerable<Ticket> tickets = new List<Ticket> { _ticket };
            ResponseModel<SeatDomainModel> seats = new ResponseModel<SeatDomainModel>()
            {
                DomainModel = null,
                IsSuccessful = false,
                ErrorMessage = Messages.SEAT_DOESNT_EXISTS
            };
            Task<ResponseModel<SeatDomainModel>> seat = Task.FromResult(seats);
            var expectedErrorMessage = Messages.SEAT_DOESNT_EXISTS;

            //Act
            _mockTicketsRepository.Setup(x => x.GetTicketsByProjectionId(It.IsAny<Guid>())).Returns(tickets);
            _mockSeatService.Setup(x => x.GetSeatByIdAsync(It.IsAny<Guid>())).Returns(seat);
            var resultAction = ticketService.GetBusySeats(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction.DomainModel);
            Assert.IsFalse(resultAction.IsSuccessful);
            Assert.AreEqual(expectedErrorMessage, resultAction.ErrorMessage);
        }
    }
}
